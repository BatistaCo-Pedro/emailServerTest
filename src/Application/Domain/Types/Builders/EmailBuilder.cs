namespace App.Server.Notification.Application.Domain.Types.Builders;

/// <summary>
/// Builder class for emails.
/// </summary>
public sealed class EmailBuilder : IDisposable
{
    /// <summary>
    /// Underlying mail message. Set in the constructor.
    /// </summary>
    private readonly MailMessage _mailMessage;

    /// <summary>
    /// Attachments container.
    /// </summary>
    private readonly List<Attachment> _attachments = [];

    /// <summary>
    /// Linked resources' container.
    /// </summary>
    private readonly List<LinkedResource> _linkedResources = [];

    /// <summary>
    /// Reply to list.
    /// </summary>
    private readonly List<MailAddress> _replyToList = [];

    /// <summary>
    /// The plain text body of the email.
    /// </summary>
    private string? _plainBody;

    /// <summary>
    /// The html text of the email.
    /// </summary>
    /// <remarks>
    /// Could be 'HtmlDocument' from HtmlAgilityPack in the future.
    /// </remarks>
    private string? _htmlBody;

    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="from">The sender of the email.</param>
    /// <param name="to">The recipient of the email.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="priority">The priority of the email.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="from" /> is <see langword="null" />. -or- <paramref name="to" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    /// <paramref name="from" /> or <paramref name="to" /> is malformed.</exception>
    public EmailBuilder(
        MailAddress from,
        MailAddress to,
        string subject,
        MailPriority priority = MailPriority.Normal
    )
    {
        _mailMessage = new MailMessage(from, to) { Subject = subject, Priority = priority };
    }

    /// <summary>
    /// Adds an html body to the email.
    /// </summary>
    /// <param name="body">The html body to add.</param>
    /// <returns>A reference to the builder.</returns>
    /// <exception cref="InvalidOperationException">Multiple html bodies in one email.</exception>
    /// <exception cref="ObjectDisposedException">If the builder has been disposed.</exception>
    public EmailBuilder AddHtml(string body)
    {
        CheckDisposed();

        Guard.Null(_htmlBody, "Cannot have multiple html bodies in one email.", nameof(_htmlBody));

        _htmlBody = body;

        return this;
    }

    /// <summary>
    /// Adds a plain text body to the email.
    /// </summary>
    /// <param name="body">The plain text body to add.</param>
    /// <returns>A reference to the builder.</returns>
    /// <exception cref="InvalidOperationException">Cannot have multiple plain bodies in one email.</exception>
    /// <exception cref="ObjectDisposedException">If the builder has been disposed.</exception>
    public EmailBuilder AddPlain(string body)
    {
        CheckDisposed();

        Guard.Null(
            _plainBody,
            "Cannot have multiple plain text bodies in one email.",
            nameof(_plainBody)
        );

        _plainBody = body;

        return this;
    }

    /// <summary>
    /// Add linked resources to the html view of the email.
    /// </summary>
    /// <param name="linkedResources">The list of linked resources to add.</param>
    /// <returns>A reference to the builder.</returns>
    /// <exception cref="ArgumentNullException">If either linked resources or html view is null.</exception>
    /// <exception cref="InvalidOperationException">If attachments have already been added.</exception>
    /// <exception cref="ObjectDisposedException">If the builder has been disposed.</exception>
    public EmailBuilder AddLinkedResources(ICollection<LinkedResource> linkedResources)
    {
        CheckDisposed();

        Guard.NotNullOrEmpty(linkedResources);
        Guard.NotNull(
            _htmlBody,
            "Add an html body before adding linked resources.",
            nameof(_htmlBody)
        );

        _linkedResources.AddRange(linkedResources);
        return this;
    }

    /// <summary>
    /// Add attachments to the email.
    /// </summary>
    /// <param name="attachments">The attachment list to add.</param>
    /// <returns>A reference to the builder.</returns>
    /// <exception cref="ArgumentException">If either attachments or any view is null.</exception>
    /// <exception cref="InvalidOperationException">If linked resources have already been added.</exception>
    /// <exception cref="ObjectDisposedException">If the builder has been disposed.</exception>
    public EmailBuilder AddAttachments(ICollection<Attachment> attachments)
    {
        CheckDisposed();

        Guard.NotNullOrEmpty(attachments);
        Guard.NotNull([_htmlBody, _plainBody], "Add an email body before adding attachments.");

        _attachments.AddRange(attachments);
        return this;
    }

    /// <summary>
    /// Adds reply to addresses to the email.
    /// </summary>
    /// <param name="replyToArray">The array of reply to addresses.</param>
    /// <returns>A reference to the builder.</returns>
    /// <exception cref="ObjectDisposedException">If the builder has been disposed.</exception>
    public EmailBuilder AddReplyTo(params MailAddress[] replyToArray)
    {
        CheckDisposed();

        _replyToList.AddRange(replyToArray);

        return this;
    }

    /// <summary>
    /// Decision-making method. Decides what email type to build and builds it.
    /// </summary>
    /// <returns>Email of type <see cref="MailMessage"/>.</returns>
    /// <exception cref="ObjectDisposedException">If the builder has been disposed.</exception>
    public MailMessage Build()
    {
        CheckDisposed();

        // Create new instance to dispose of the _mailMessage object.
        var mailMessage = new MailMessage(_mailMessage.From!, _mailMessage.To.First())
        {
            Subject = _mailMessage.Subject,
            Priority = _mailMessage.Priority,
        };

        AddReplyToEmails(mailMessage, _replyToList);

        if (_htmlBody != null)
        {
            AddHtmlViewWithLinkedResources(mailMessage, _htmlBody);
        }

        if (_plainBody != null)
        {
            AddPlainTextView(mailMessage, _plainBody);
        }

        foreach (var attachment in _attachments)
        {
            // Create new instance to dispose of the _attachments list.
            mailMessage.Attachments.Add(
                new Attachment(
                    attachment.ContentStream,
                    attachment.Name,
                    attachment.ContentType.MediaType
                )
            );
        }

        mailMessage.Body = _htmlBody ?? _plainBody;
        return mailMessage;
    }

    /// <summary>
    /// Adds an html view with linked resources to the mail message.
    /// </summary>
    /// <param name="mailMessage">The <see cref="MailMessage"/> to add the html view to.</param>
    /// <param name="htmlBody">The body to create the view from.</param>
    private void AddHtmlViewWithLinkedResources(MailMessage mailMessage, string htmlBody)
    {
        var htmlView = AlternateView.CreateAlternateViewFromString(
            htmlBody,
            Encoding.UTF8,
            MediaTypeNames.Text.Html
        );

        foreach (var linkedResource in _linkedResources)
        {
            // Create new instance to dispose of the _linkedResources list.
            htmlView.LinkedResources.Add(
                new LinkedResource(linkedResource.ContentStream, linkedResource.ContentType)
                {
                    ContentId = linkedResource.ContentId,
                }
            );
        }

        mailMessage.AlternateViews.Add(htmlView);
    }

    /// <summary>
    /// Adds an html view with linked resources to the mail message.
    /// </summary>
    /// <param name="mailMessage">The <see cref="MailMessage"/> to add the html view to.</param>
    /// <param name="plainBody">The body to create the view from.</param>
    private static void AddPlainTextView(MailMessage mailMessage, string plainBody)
    {
        var plainView = AlternateView.CreateAlternateViewFromString(
            plainBody,
            Encoding.UTF8,
            MediaTypeNames.Text.Plain
        );

        mailMessage.AlternateViews.Add(plainView);
    }

    /// <summary>
    /// Adds reply to addresses to the mail message.
    /// </summary>
    /// <param name="mailMessage">The <see cref="MailMessage"/> to add the reply to addresses to.</param>
    /// <param name="replyToList">The <see cref="ICollection{T}"/> of <see cref="MailAddress"/> containing the reply to addresses.</param>
    private static void AddReplyToEmails(
        MailMessage mailMessage,
        ICollection<MailAddress> replyToList
    )
    {
        foreach (var replyTo in replyToList)
        {
            mailMessage.ReplyToList.Add(new MailAddress(replyTo.Address, replyTo.DisplayName));
        }
    }

    /// <summary>
    /// Checks if this builder has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the builder has been disposed.</exception>
    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(EmailBuilder));
        }
    }

    /// <summary>
    /// Flag to check if the object has been disposed.
    /// </summary>
    private bool _disposed;

    public void Dispose()
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
            // free managed resources
            _mailMessage.Dispose();

            _attachments.ForEach(x => x.Dispose());
            _attachments.Clear();

            _linkedResources.ForEach(x => x.Dispose());
            _linkedResources.Clear();

            _replyToList.Clear();

            _htmlBody = null;
            _plainBody = null;
        }

        // free unmanaged resources

        _disposed = true;
    }

    ~EmailBuilder() => Dispose(false);
}
