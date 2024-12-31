using Microsoft.AspNetCore.StaticFiles;

namespace App.Server.Notification.Application.Domain.Common.Helpers;

public static class MimeTypeHelper
{
    public static NonEmptyString GetMimeType(byte[] data) =>
        MimeInspector.Inspector.Inspect(data).ByMimeType().Single().MimeType;

    public static NonEmptyString GetMimeType(Uri uri) =>
        new FileExtensionContentTypeProvider().TryGetContentType(uri.AbsolutePath, out var mimeType)
            ? mimeType
            : throw new ArgumentException("The media type could not be determined.");
}
