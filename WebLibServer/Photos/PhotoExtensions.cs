using System.Text;

namespace WebLibServer.Photos;

public static class PhotoExtensions
{
    private static string _getFilename(Guid photoId, string photoExt, int? photoSize)
    {
        var sb = new StringBuilder(photoId.ToString());

        if (photoSize != null)
            sb.Append('-').Append(photoSize);

        sb.Append('.').Append(photoExt);

        return sb.ToString();
    }

    public static string GetFilename(this ISharedPhoto photo, int? size = null)
    {
        if (photo == null) return null;
        return _getFilename(photo.Id, photo.Ext, size);
    }
}