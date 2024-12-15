#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace App.Server.Notification.Application.Domain.Entities;

/// <summary>
/// Template type entity.
/// </summary>
public class TemplateType : Entity, IAggregateRoot
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
    public IReadOnlyCollection<EmailTemplate> EmailTemplates
    {
        get => _emailTemplates;
        private init => _emailTemplates = value.ToList();
    }

    /// <summary>
    /// Adds accepted merge tags to the template type.
    /// </summary>
    /// <param name="newAcceptedMergeTags">The accepted merge tags to add.</param>
    /// <returns>A <see cref="Result"/> defining the success of the operation.</returns>
    public Result AddAcceptedVariables(ImmutableHashSet<MergeTag> newAcceptedMergeTags)
    {
        return Result.Try(() =>
        {
            _acceptedMergeTags.UnionWith(newAcceptedMergeTags);
        });
    }

    /// <summary>
    /// Adds an email template to the template type.
    /// </summary>
    /// <param name="emailTemplate">The email template to add.</param>
    /// <returns></returns>
    public void AddEmailTemplate(EmailTemplate emailTemplate) => _emailTemplates.Add(emailTemplate);

    /// <summary>
    /// Gets an email template by id.
    /// </summary>
    /// <param name="emailTemplateId">The ID of the email template to get.</param>
    /// <returns>A <see cref="Result{TValue}"/> object representing the result of the operation.</returns>
    public Result<EmailTemplate> GetEmailTemplate(Guid emailTemplateId) =>
        _emailTemplates
            .FirstOrDefault(x => x.Id == emailTemplateId)
            .ToResult($"Email template with id {emailTemplateId} not found");
}
