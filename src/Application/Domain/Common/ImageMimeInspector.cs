using MimeDetective;
using MimeDetective.Definitions;
using MimeDetective.Definitions.Licensing;
using MimeDetective.Storage;

namespace App.Server.Notification.Application.Domain.Common;

/// <summary>
/// Class to inspect the MIME type of images.
/// </summary>
/// <remarks>
/// Accepts only the supported image extensions.
/// </remarks>
public static class ImageMimeInspector
{
    /// <summary>
    /// The supported image extensions.
    /// </summary>
    private static readonly ImmutableHashSet<string> SupportedExtensions = new[]
    {
        "jpg",
        "jpeg",
        "png",
        "gif",
        "bmp",
        "tif",
        "tiff",
        "svg", // should be validated
        "webp"
    }.ToImmutableHashSet(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Blank Method which will force constructor of static class
    /// </summary>
    public static void Init() { }

    /// <summary>
    /// Initializes static members of the <see cref="ImageMimeInspector"/> class.
    /// </summary>
    static ImageMimeInspector()
    {
        Inspector = new ContentInspectorBuilder
        {
            Definitions = new CondensedBuilder { UsageType = UsageType.PersonalNonCommercial, }
                .Build()
                .ScopeExtensions(SupportedExtensions)
                .TrimDescription()
                .ToImmutableArray()
        }.Build();
    }

    /// <summary>
    /// The inspector to use for image MIME type inspection.
    /// </summary>
    public static IContentInspector Inspector { get; }
}
