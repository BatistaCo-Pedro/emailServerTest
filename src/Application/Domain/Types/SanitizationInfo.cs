namespace App.Server.Notification.Application.Domain.Types;

/// <summary>
/// Represents information about a removed element.
/// </summary>
/// <param name="TagName">The name of the removed tag.</param>
/// <param name="ElementName">The name of the removed element.</param>
/// <param name="Reason">The reason for removal.</param>
/// <param name="Position">The position of the removed tag.</param>
public readonly record struct RemovedElementInfo(
    string TagName,
    string? ElementName,
    string Reason,
    string? Position
);

/// <summary>
/// Represents information about the sanitization process.
/// </summary>
/// <param name="RemovedElements">A list of removed elements.</param>
public readonly record struct SanitizationInfo(ImmutableArray<RemovedElementInfo> RemovedElements)
{
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <param name="builder">The string builder to build the string.</param>
    /// <returns>A string representing the object.</returns>
    /// <remarks>
    /// This method is used by the <see cref="object.ToString"/> method.
    /// </remarks>
    private bool PrintMembers(StringBuilder builder)
    {
        foreach (var removedElementInfo in RemovedElements)
        {
            builder.Append(removedElementInfo.ToString());
            builder.Append("; ");
        }

        return true;
    }
}

/// <summary>
/// A builder for <see cref="App.Server.Notification.Application.Domain.Types.SanitizationInfo"/>.
/// </summary>
public sealed class SanitizationInfoBuilder
{
    private readonly List<RemovingAttributeEventArgs> _removedAttributesArgs = [];
    private readonly List<RemovingStyleEventArgs> _removedStyleArgs = [];
    private readonly List<RemovingTagEventArgs> _removedTagsArgs = [];
    private readonly List<RemovingCssClassEventArgs> _removedCssClassArgs = [];

    private readonly List<RemovedElementInfo> _removedElementsInfoList = [];

    /// <summary>
    /// Adds a removed attribute to the builder.
    /// </summary>
    /// <param name="htmlSanitizer">The html sanitizer object.</param>
    /// <param name="args">The event arguments.</param>
    public void AddRemovedAttribute(object? htmlSanitizer, RemovingAttributeEventArgs args) =>
        _removedAttributesArgs.Add(args);

    /// <summary>
    /// Adds a removed style to the builder.
    /// </summary>
    /// <param name="htmlSanitizer">The html sanitizer object.</param>
    /// <param name="args">The event arguments.</param>
    public void AddRemovedStyle(object? htmlSanitizer, RemovingStyleEventArgs args) =>
        _removedStyleArgs.Add(args);

    /// <summary>
    /// Adds a removed tag to the builder.
    /// </summary>
    /// <param name="htmlSanitizer">The html sanitizer object.</param>
    /// <param name="args">The event arguments.</param>
    public void AddRemovedTag(object? htmlSanitizer, RemovingTagEventArgs args) =>
        _removedTagsArgs.Add(args);

    /// <summary>
    /// Adds a removed css class to the builder.
    /// </summary>
    /// <param name="htmlSanitizer">The html sanitizer object.</param>
    /// <param name="args">The event arguments.</param>
    public void AddRemovedCssClass(object? htmlSanitizer, RemovingCssClassEventArgs args) =>
        _removedCssClassArgs.Add(args);

    /// <summary>
    /// Builds the <see cref="App.Server.Notification.Application.Domain.Types.SanitizationInfo"/>.
    /// </summary>
    /// <returns>A <see cref="App.Server.Notification.Application.Domain.Types.SanitizationInfo"/> object will all the information related to the sanitization.</returns>
    public SanitizationInfo Build()
    {
        AddAllRemovedToRemovedElementsInfoList();
        return new SanitizationInfo(_removedElementsInfoList.ToImmutableArray());
    }

    #region Private Methods

    private void AddAllRemovedToRemovedElementsInfoList()
    {
        AddRemovedAttributesToRemovedElementsInfoList();
        AddRemovedTagsToRemovedElementsInfoList();
        AddRemovedCssClassesToRemovedElementsInfoList();
        AddRemovedStylesToRemovedElementsInfoList();
    }

    private void AddRemovedAttributesToRemovedElementsInfoList()
    {
        foreach (var args in _removedAttributesArgs)
        {
            _removedElementsInfoList.Add(
                new RemovedElementInfo(
                    args.Tag.TagName,
                    args.Attribute.Name,
                    args.Reason.ToString(),
                    args.Tag.SourceReference?.Position.ToString()
                )
            );
        }
    }

    private void AddRemovedTagsToRemovedElementsInfoList()
    {
        foreach (var args in _removedTagsArgs)
        {
            _removedElementsInfoList.Add(
                new RemovedElementInfo(
                    args.Tag.TagName,
                    args.Tag.TagName,
                    args.Reason.ToString(),
                    args.Tag.SourceReference?.Position.ToString()
                )
            );
        }
    }

    private void AddRemovedCssClassesToRemovedElementsInfoList()
    {
        foreach (var args in _removedCssClassArgs)
        {
            _removedElementsInfoList.Add(
                new RemovedElementInfo(
                    args.Tag.TagName,
                    args.CssClass,
                    args.Reason.ToString(),
                    args.Tag.SourceReference?.Position.ToString()
                )
            );
        }
    }

    private void AddRemovedStylesToRemovedElementsInfoList()
    {
        foreach (var args in _removedStyleArgs)
        {
            _removedElementsInfoList.Add(
                new RemovedElementInfo(
                    args.Tag.TagName,
                    args.Style.Name,
                    args.Reason.ToString(),
                    args.Tag.SourceReference?.Position.ToString()
                )
            );
        }
    }

    #endregion
}
