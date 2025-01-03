#pragma warning disable CS8618, CS9264
namespace App.Server.Notification.Application.Domain.DataModels.Emailing;

/// <summary>
/// This class will be serialized and used by the mail queue to save mail information.
/// </summary>
/// <remarks>
/// This record does not inherit from <see cref="IJsonEntity"/> as it does not directly interact with entities.
/// <br/> <br/>
/// It uses the system types e.g. `<see cref="String"/> instead of the domain types e.g. <see cref="NonEmptyString"/>
/// to make sure serialization works properly across implementations - Text.Json, Newtonsoft etc...
/// </remarks>
public record EmailInfoDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailInfoDto"/> record.
    /// </summary>
    /// <remarks>
    /// This constructor is required by json serializers.
    /// </remarks>
    [Obsolete("Required by other json serializers - use other constructors.")]
    public EmailInfoDto() { }

    /// <summary>
    /// Creates a new instance of the <see cref="EmailInfoDto"/> record.
    /// This is the recommended way to create a new instance of <see cref="EmailInfoDto"/>
    /// </summary>
    /// <param name="dataOwnerId">The ID of the data owner.</param>
    /// <param name="templateTypeId">The ID of the template type.</param>
    /// <param name="sender">The sender name.</param>
    /// <param name="senderAddress">The sender address.</param>
    /// <param name="recipient">The recipient name.</param>
    /// <param name="recipientAddress">The recipient address.</param>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="mergeTagArguments">The arguments to be used within the merge tags.</param>
    /// <param name="attachmentDtos">The attachment dtos.</param>
    /// <param name="emailTemplateId">The ID of the email template
    /// - if provided will be used, otherwise the default email template for the data owner will be used.</param>
    /// <param name="customSubject">A custom subject sent by the event caller.</param>
    /// <returns>A new <see cref="EmailInfoDto"/> object with the provided data.</returns>
    [JsonConstructor]
    [SetsRequiredMembers]
    public EmailInfoDto(
        Guid dataOwnerId,
        Guid templateTypeId,
        string sender,
        string senderAddress,
        string recipient,
        string recipientAddress,
        string cultureCode,
        ImmutableDictionary<string, string> mergeTagArguments,
        ImmutableHashSet<AttachmentDto> attachmentDtos,
        Guid? emailTemplateId = null,
        string? customSubject = null
    )
    {
        DataOwnerId = dataOwnerId;
        TemplateTypeId = templateTypeId;
        Sender = (NonEmptyString)sender;
        SenderAddress = (NonEmptyString)senderAddress;
        Recipient = (NonEmptyString)recipient;
        RecipientAddress = (NonEmptyString)recipientAddress;
        CultureCode = (CultureCode)cultureCode;
        MergeTagArguments = mergeTagArguments;
        AttachmentDtos = attachmentDtos;
        EmailTemplateId = emailTemplateId;
        CustomSubject = customSubject;
    }

    /// <summary>
    /// The ID of the data owner.
    /// </summary>
    [JsonPropertyName("dataOwnerId")]
    public required Guid DataOwnerId { get; init; }

    /// <summary>
    /// The ID of the template type.
    /// </summary>
    [JsonPropertyName("templateTypeId")]
    public required Guid TemplateTypeId { get; init; }

    /// <summary>
    /// The sender of the email.
    /// </summary>
    [JsonPropertyName("sender")]
    public required string Sender { get; init; }

    /// <summary>
    /// The sender address of the email.
    /// </summary>
    [JsonPropertyName("senderAddress")]
    public required string SenderAddress { get; init; }

    /// <summary>
    /// The sender of the email as <see cref="MailAddress"/>.
    /// </summary>
    [JsonIgnore]
    [NotMapped]
    public MailAddress SenderMailAddress => new(SenderAddress, Sender);

    /// <summary>
    /// The recipient of the email.
    /// </summary>
    [JsonPropertyName("recipient")]
    public required string Recipient { get; init; }

    /// <summary>
    /// The recipient address of the email.
    /// </summary>
    [JsonPropertyName("recipientAddress")]
    public required string RecipientAddress { get; init; }

    /// <summary>
    /// The recipient of the email as <see cref="MailAddress"/>.
    /// </summary>
    [JsonIgnore]
    [NotMapped]
    public MailAddress RecipientMailAddress => new(RecipientAddress, Recipient);

    /// <summary>
    /// The culture code.
    /// </summary>
    [JsonPropertyName("cultureCode")]
    public required string CultureCode { get; init; }

    /// <summary>
    /// The arguments to be used within the merge tags.
    /// </summary>
    [JsonPropertyName("mergeTagArguments")]
    public ImmutableDictionary<string, string> MergeTagArguments { get; init; }

    /// <summary>
    /// The resources of the email.
    /// </summary>
    [JsonPropertyName("attachmentDtos")]
    public ImmutableHashSet<AttachmentDto> AttachmentDtos { get; init; }

    /// <summary>
    /// The attachments of the email.
    /// </summary>
    [JsonIgnore]
    [NotMapped]
    public ImmutableHashSet<Attachment> Attachments =>
        AttachmentDtos.Select(x => x.ToAttachment()).ToImmutableHashSet();

    /// <summary>
    /// The ID of the email template.
    /// </summary>
    [JsonPropertyName("emailTemplateId")]
    public Guid? EmailTemplateId { get; init; }

    /// <summary>
    /// A custom subject sent by the event caller.
    /// </summary>
    [JsonPropertyName("customSubject")]
    public string? CustomSubject { get; init; }
}
