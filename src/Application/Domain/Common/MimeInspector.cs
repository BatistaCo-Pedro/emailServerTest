using MimeDetective.Definitions;
using MimeDetective.Definitions.Licensing;
using MimeDetective.Storage;

namespace App.Server.Notification.Application.Domain.Common;

/// <summary>
/// Class used to inspect MIME types.
/// </summary>
/// <remarks>
/// Accepts all MIME types defined in the <see cref="CondensedBuilder"/> with the <see cref="UsageType.PersonalNonCommercial"/> usage type.
/// </remarks>
public static class MimeInspector
{
    /// <summary>
    /// Blank Method which will force constructor of static class
    /// </summary>
    public static void Init() { }
    
    /// <summary>
    /// Initializes static members of the <see cref="MimeInspector"/> class.
    /// </summary>
    static MimeInspector()
    {
        Inspector = new ContentInspectorBuilder()
        {
            Definitions = new CondensedBuilder { UsageType = UsageType.PersonalNonCommercial, }
                .Build()
                .TrimDescription()
                .ToImmutableArray()
        }.Build();
    }
    
    /// <summary>
    /// The inspector used to inspect MIME types.
    /// </summary>
    public static IContentInspector Inspector { get; }
}
