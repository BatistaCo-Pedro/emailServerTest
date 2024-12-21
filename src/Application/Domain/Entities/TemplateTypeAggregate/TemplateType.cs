#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace App.Server.Notification.Application.Domain.Entities.TemplateTypeAggregate;

/// <summary>
/// Template type entity.
/// </summary>
public class TemplateType : AggregateRoot
{
    [Obsolete("Required by DI and EF Core")]
    protected TemplateType() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateType"/> class.
    /// </summary>
    /// <param name="name">The name of the template type.</param>
    /// <param name="acceptedMergeTags">A list of accepted merge tags for the template type.</param>
    public TemplateType(NonEmptyString name, ImmutableHashSet<MergeTag> acceptedMergeTags)
    {
        Name = name;
        AcceptedMergeTags = acceptedMergeTags;
    }

    /// <summary>
    /// Private list of available merge tags for encapsulation.
    /// </summary>
    private readonly HashSet<MergeTag> _acceptedMergeTags = [];

    /// <summary>
    /// Private list of email body contents for encapsulation.
    /// </summary>
    private readonly List<EmailTemplate> _emailTemplates = [];

    /// <summary>
    /// The name of the template type.
    /// </summary>
    public NonEmptyString Name { get; private set; }

    /// <summary>
    /// Available merge tags for this template type.
    /// </summary>
    public IReadOnlySet<MergeTag> AcceptedMergeTags
    {
        get => _acceptedMergeTags;
        private init => _acceptedMergeTags = value.ToHashSet();
    }

    /// <summary>
    /// The email templates of this template type as <see cref="IReadOnlyCollection{T}"/> for encapsulation.
    /// </summary>
    public virtual IReadOnlyCollection<EmailTemplate> EmailTemplates
    {
        get => _emailTemplates;
        private init => _emailTemplates = value.ToList();
    }

    /// <summary>
    /// Adds accepted merge tags to the template type.
    /// </summary>
    /// <param name="newAcceptedMergeTags">The accepted merge tags to add.</param>
    /// <returns>A <see cref="Result"/> defining the success of the operation.</returns>
    public void AddAcceptedVariables(ImmutableHashSet<MergeTag> newAcceptedMergeTags) =>
        _acceptedMergeTags.UnionWith(newAcceptedMergeTags);

    /// <summary>
    /// Gets a fallback email template.
    /// This will simply be the first email template that does not match the provided ID.
    /// </summary>
    /// <param name="emailTemplateIdToNotMatch">The ID to not match.</param>
    /// <returns>A <see cref="Result"/> representing the result of the operation.</returns>
    public Result<EmailTemplate> GetFallBackEmailTemplate(Guid emailTemplateIdToNotMatch) =>
        _emailTemplates
            .FirstOrDefault(x => x.Id != emailTemplateIdToNotMatch)
            .ToResult(
                $"Template type with ID {Id} does not contain a second email template to use as fallback"
            );

    /// <summary>
    /// Adds an email template to the template type.
    /// </summary>
    /// <param name="emailTemplate">The email template to add.</param>
    public void AddEmailTemplate(EmailTemplate emailTemplate)
    {
        _emailTemplates.Add(emailTemplate);

        RaiseDomainEvent(
            new EmailTemplateCreated(
                emailTemplate.Id,
                Id,
                _emailTemplates.Count == 1,
                emailTemplate.DataOwnerId
            )
        );
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="emailTemplate"></param>
    public void RemoveEmailTemplate(EmailTemplate emailTemplate)
    {
        _emailTemplates.Remove(emailTemplate);
    }
}
