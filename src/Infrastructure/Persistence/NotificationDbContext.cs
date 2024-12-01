using System.Collections.Concurrent;
using App.Server.Notification.Infrastructure.Messaging.DomainEvents;

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
    /// <param name="domainEventDispatcher">The dispatcher for the domain events.</param>
    public NotificationDbContext(
        DbContextOptions<NotificationDbContext> options,
        IDomainEventDispatcher domainEventDispatcher
    )
        : this(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    /// <summary>
    /// Ctor for the <see cref="NotificationDbContext"/>.
    /// </summary>
    /// <param name="options">Options to create the database.</param>
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
        Log.Debug("Creating {DbContext}", nameof(NotificationDbContext));

        _domainEventDispatcher = new DefaultDomainEventDispatcher();

        SavingChanges += (_, _) => SavingChangesEvent();

        RegisterRepository<ITemplateTypeRepository, TemplateType>(new TemplateTypeRepository(this));
    }

    public override int SaveChanges()
    {
        var entities = ChangeTracker
            .Entries<Entity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .AsEnumerable();

        _domainEventDispatcher.DispatchAndClear(entities);

        return base.SaveChanges();
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
    }

    /// <inheritdoc />
    public bool RegisterRepository<TRepo, TEntity>(TRepo repository)
        where TEntity : IAggregateRoot
        where TRepo : IRepository<TEntity>
    {
        return _repositories.TryAdd(typeof(TEntity).Name, repository);
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
                    return Result
                        .Ok()
                        .Log(LogEventLevel.Debug, loggingContext, "Transaction succeeded");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result
                        .Fail(ex.Message)
                        .Log(
                            LogEventLevel.Error,
                            loggingContext,
                            "Exception: {Message}",
                            ex.Message
                        );
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
                        .Log(LogEventLevel.Debug, loggingContext, "Transaction succeeded");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<T>
                        .Fail(ex.Message)
                        .Log(
                            LogEventLevel.Error,
                            loggingContext,
                            "Exception: {Message}",
                            ex.Message
                        );
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
                    return result.Log(LogEventLevel.Error, loggingContext, "Transaction failed");
                }

                transaction.Commit();
                return result.Log(LogEventLevel.Debug, loggingContext, "Transaction succeeded");
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
                    return Result
                        .Ok()
                        .Log(LogEventLevel.Debug, loggingContext, "Transaction succeeded");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result
                        .Fail(ex.Message)
                        .Log(
                            LogEventLevel.Error,
                            loggingContext,
                            "Exception: {Message}",
                            ex.Message
                        );
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
                        .Log(LogEventLevel.Debug, loggingContext, "Transaction succeeded");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<T>
                        .Fail(ex.Message)
                        .Log(
                            LogEventLevel.Error,
                            loggingContext,
                            "Exception: {Message}",
                            ex.Message
                        );
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
                    return result.Log(LogEventLevel.Error, loggingContext, "Transaction failed");
                }

                await transaction.CommitAsync();
                return result.Log(LogEventLevel.Debug, loggingContext, "Transaction succeeded");
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

    private void SavingChangesEvent()
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
