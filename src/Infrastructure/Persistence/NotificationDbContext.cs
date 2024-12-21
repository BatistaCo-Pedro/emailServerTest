namespace App.Server.Notification.Infrastructure.Persistence;

// Learn more about disposal pattern: https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
/// <summary>
/// The EF Core database context for the notification service.
/// Implements <see cref="IUnitOfWork"/>.
/// </summary>
internal sealed class NotificationDbContext : DbContext, IUnitOfWork
{
    private const string DefaultLoggingContext = "Transaction";

    private readonly ConcurrentDictionary<string, IRepository> _repositories = [];

    private readonly IDomainEventDispatcher _domainEventDispatcher;

    /// <summary>
    /// Ctor for the <see cref="NotificationDbContext"/>.
    /// </summary>
    /// <param name="options">Options to create the database.</param>
    /// <param name="domainEventDispatcher">The domain event to use - must be registered in the dependency container.</param>
    public NotificationDbContext(
        DbContextOptions<NotificationDbContext> options,
        IDomainEventDispatcher domainEventDispatcher
    )
        : base(options)
    {
        Log.Debug("Creating {DbContext}", nameof(NotificationDbContext));

        SavingChanges += OnSavingChanges;

        _domainEventDispatcher = domainEventDispatcher;

        // Register custom repositories
        RegisterRepository<ITemplateTypeRepository, TemplateType>(new TemplateTypeRepository(this));
        RegisterRepository<IDataOwnerRepository, DataOwner>(new DataOwnerRepository(this));
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureJsonEntities(modelBuilder);

        modelBuilder.Entity<TemplateType>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<DataOwner>().HasIndex(x => new { x.Name, x.Source }).IsUnique();
        modelBuilder
            .Entity<EmailBodyContent>()
            .HasIndex(x => new { x.CultureCode, x.EmailTemplateId })
            .IsUnique();
        modelBuilder
            .Entity<EmailTemplate>()
            .HasIndex(x => new { x.Name, x.DataOwnerId })
            .IsUnique()
            .AreNullsDistinct(false);

        modelBuilder
            .Entity<EmailBodyContent>()
            .Property(x => x.Body)
            .HasConversion<CompressedStringConverter>();

        modelBuilder
            .Entity<EmailBodyContent>()
            .Property(x => x.JsonStructure)
            .HasConversion<CompressedJsonConverter>();

        modelBuilder
            .Entity<TemplateType>()
            .HasMany(x => x.EmailTemplates)
            .WithOne(x => x.TemplateType)
            .HasForeignKey(x => x.TemplateTypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmailSettings>(c =>
        {
            c.HasOne<TemplateType>()
                .WithMany()
                .HasForeignKey(x => x.TemplateTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            c.HasOne<EmailTemplate>()
                .WithMany()
                .HasForeignKey(x => x.DefaultEmailTemplateId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            c.HasOne<DataOwner>()
                .WithMany(x => x.EmailSettings)
                .HasForeignKey(x => x.DataOwnerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            c.HasIndex(x => new
                {
                    x.TemplateTypeId,
                    x.DataOwnerId,
                    x.DefaultEmailTemplateId,
                })
                .IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<NonEmptyString>().HaveConversion<NonEmptyStringConverter>();
        configurationBuilder.Properties<CultureCode>().HaveConversion<CultureCodeConverter>();
        configurationBuilder.Properties<HtmlString>().HaveConversion<HtmlStringConverter>();
        configurationBuilder
            .Properties<MergeTagShortCode>()
            .HaveConversion<MergeTagShortCodeConverter>();
        configurationBuilder
            .Properties<SmtpSettings>()
            .HaveConversion<EncryptedSmtpSettingsConverter>();
    }

    /// <inheritdoc />
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var entities = ChangeTracker
            .Entries<Entity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .ToList();

        _domainEventDispatcher.DispatchAndClear(entities).Wait();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        var entities = ChangeTracker
            .Entries<Entity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .AsEnumerable();

        await _domainEventDispatcher.DispatchAndClear(entities);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <inheritdoc />
    public bool RegisterRepository<TRepo, TEntity>(TRepo repository)
        where TEntity : AggregateRoot
        where TRepo : IRepository<TEntity>
    {
        return _repositories.TryAdd(typeof(TRepo).Name, repository);
    }

    /// <inheritdoc />
    public TRepo GetRepository<TRepo>()
        where TRepo : IRepository
    {
        var result = _repositories.TryGetValue(typeof(TRepo).Name, out var repository);

        if (result)
        {
            return (TRepo)repository!;
        }

        var entityType = typeof(TRepo).GetGenericArguments().Single();
        return (TRepo)
            Activator.CreateInstance(
                typeof(BaseCrudRepository<>).MakeGenericType(entityType),
                this
            )!;
    }

    /// <inheritdoc />
    public Result UseTransaction(
        Action action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return Database
            .CreateExecutionStrategy()
            .Execute(() =>
            {
                using var transaction = Database.BeginTransaction();

                try
                {
                    action();
                    transaction.Commit();
                    return Result.Ok().Log(logLevel: LogEventLevel.Debug, context: loggingContext);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result
                        .Fail(ex.Message)
                        .Log(logLevel: LogEventLevel.Error, context: loggingContext);
                }
            });
    }

    /// <inheritdoc />
    public Result<T> UseTransaction<T>(
        Func<T> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return Database
            .CreateExecutionStrategy()
            .Execute(() =>
            {
                using var transaction = Database.BeginTransaction();

                try
                {
                    var result = action();
                    transaction.Commit();
                    return Result<T>
                        .Ok(result)
                        .Log(logLevel: LogEventLevel.Debug, context: loggingContext);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<T>
                        .Fail(ex.Message)
                        .Log(logLevel: LogEventLevel.Error, context: loggingContext);
                }
            });
    }

    /// <inheritdoc />
    public Result<T> UseTransaction<T>(
        Func<Result<T>> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return Database
            .CreateExecutionStrategy()
            .Execute(() =>
            {
                using var transaction = Database.BeginTransaction();

                var result = action();
                if (!result.IsSuccess)
                {
                    transaction.Rollback();
                    return result.Log(logLevel: LogEventLevel.Debug, context: loggingContext);
                }

                transaction.Commit();
                return result.Log(logLevel: LogEventLevel.Debug, context: loggingContext);
            });
    }

    /// <inheritdoc />
    public async Task<Result> UseTransactionAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return await Database
            .CreateExecutionStrategy()
            .ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(
                    cancellationToken
                );

                try
                {
                    await action();
                    await transaction.CommitAsync(cancellationToken);
                    return Result.Ok().Log(logLevel: LogEventLevel.Debug, context: loggingContext);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result
                        .Fail(ex.Message)
                        .Log(logLevel: LogEventLevel.Error, context: loggingContext);
                }
            });
    }

    /// <inheritdoc />
    public async Task<Result<T>> UseTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return await Database
            .CreateExecutionStrategy()
            .ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(
                    cancellationToken
                );

                try
                {
                    var result = await action();
                    await transaction.CommitAsync(cancellationToken);
                    return Result<T>
                        .Ok(result)
                        .Log(logLevel: LogEventLevel.Debug, context: loggingContext);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<T>
                        .Fail(ex.Message)
                        .Log(logLevel: LogEventLevel.Error, context: loggingContext);
                }
            });
    }

    /// <inheritdoc />
    public async Task<Result<T>> UseTransaction<T>(
        Func<Task<Result<T>>> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return await Database
            .CreateExecutionStrategy()
            .ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync();

                var result = await action();
                if (!result.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return result.Log(logLevel: LogEventLevel.Error, context: loggingContext);
                }

                await transaction.CommitAsync();
                return result.Log(logLevel: LogEventLevel.Debug, context: loggingContext);
            });
    }

    private bool _disposed;

    // <inheritdoc />
    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Log.Debug("Disposing {DbContext}.", nameof(NotificationDbContext));

            // free managed resources
            _repositories.Clear();

            base.Dispose();
        }

        // free unmanaged resources

        _disposed = true;
    }

    ~NotificationDbContext() => Dispose(false);

    /// <summary>
    /// Configures all JSON entities implementing <see cref="IJsonEntity"/> to be mapped to the database as JSON.
    /// </summary>
    /// <param name="modelBuilder">The database model builder.</param>
    private static void ConfigureJsonEntities(ModelBuilder modelBuilder)
    {
        foreach (
            var type in Assembly
                .GetAssembly(typeof(Entity))!
                .GetExportedTypes()
                .Where(x => x.IsAssignableTo(typeof(Entity)) && x.IsAbstract == false)
        )
        {
            ConfigureJsonEntity(modelBuilder, type);
        }
    }

    /// <summary>
    /// Configure a single JSON entity implementing <see cref="IJsonEntity"/> to be mapped to the database as JSON.
    /// </summary>
    /// <param name="modelBuilder">The database model builder.</param>
    /// <param name="type">The entity type.</param>
    private static void ConfigureJsonEntity(ModelBuilder modelBuilder, Type type)
    {
        var entityTypeBuilder = modelBuilder.Entity(type);

        var entityProperties = type.GetProperties()
            .Select(x =>
            {
                var propertyType = x.PropertyType;
                // Handle collections
                if (propertyType.IsGenericType && propertyType.IsAssignableTo(typeof(IEnumerable)))
                {
                    return new
                    {
                        Type = propertyType.GetGenericArguments().Single(),
                        IsCollection = true,
                        Name = x.Name,
                    };
                }

                // Handle single entities
                return new
                {
                    Type = propertyType,
                    IsCollection = false,
                    Name = x.Name,
                };
            });

        var jsonProperties = entityProperties
            .Where(x => x.Type.IsAssignableTo(typeof(IJsonEntity)))
            .ToHashSet();

        foreach (var property in jsonProperties)
        {
            if (property.IsCollection)
            {
                entityTypeBuilder.OwnsMany(
                    property.Type,
                    property.Name,
                    builder => builder.ToJson()
                );
                continue;
            }

            entityTypeBuilder.OwnsOne(property.Type, property.Name, builder => builder.ToJson());
        }

        Log.Debug(
            "Mapped {Count} JSON entities in entity {Entity}.",
            jsonProperties.Count,
            type.Name
        );
    }

    /// <summary>
    /// Exception representing a custom repository not being registered withing the unit of work.
    /// </summary>
    private class CustomRepositoryNotRegisteredException : Exception
    {
        /// <summary>
        /// Ctor for the <see cref="CustomRepositoryNotRegisteredException"/>.
        /// </summary>
        /// <param name="type">The type of the repository.</param>
        public CustomRepositoryNotRegisteredException(Type type)
            : base($"Custom repository of type {type.Name} not registered.") { }
    }

    private void OnSavingChanges(object? sender, SavingChangesEventArgs args)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not Entity entity)
                continue;

            Log.Debug(
                "Entity {EntityName} with id {Id} is {State}",
                entity.GetType().Name,
                entity.Id,
                entry.State
            );

            if (entry.Entity is not AuditableEntity auditableEntity)
                continue;

            if (entry.State == EntityState.Added)
            {
                auditableEntity.CreatedAt = DateTime.UtcNow;
                auditableEntity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                auditableEntity.Stamp++;
                auditableEntity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
