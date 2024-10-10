using WebAppShared.Uploaders;
using WebAppShared.WebSys.DI;

namespace WebAppShared.Photos;

[Service(ServiceAttribute.Scopes.Scoped)]
public class PhotoUrlBuilderSvc(IStorageBucket storageBucket)
{
    private string GetFullUrl(Guid photoId, IEnumerable<int> availablePhotoSizes, string ext, int? size = null)
    {
        // If the requested size has not been generated
        if (size != null && !availablePhotoSizes.Contains(size.Value)) return $"/api/photos/{photoId}/sizes/{size}";
        if (size != null)
            return $"https://{storageBucket.MediaHost}/{storageBucket.Photos}/{photoId.ToString()}-{size}.{ext}";

        return $"https://{storageBucket.MediaHost}/{storageBucket.Photos}/{photoId.ToString()}.{ext}";
    }

    public string GetFullUrl(ISharedPhoto photo, int? size = null)
    {
        if (photo == null) return null;
        return GetFullUrl(photo.Id, photo.Sizes, photo.Ext, size);
    }
}