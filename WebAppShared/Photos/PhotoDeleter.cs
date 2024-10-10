using JetBrains.Annotations;
using WebAppShared.Exceptions;
using WebAppShared.Uploaders;
using WebAppShared.WebSys.DI;

namespace WebAppShared.Photos;

[Service, UsedImplicitly]
public class PhotoAndSizesDeleterBase(IFileDeleter fileDeleter, IStorageBucket bucket)
{
    public async Task Delete(ISharedPhoto photo)
    {
        if (photo == null) throw new HttpJsonError("Photo not found");

        var filesToDelete = new List<string> { photo.GetFilename() };
        filesToDelete.AddRange(photo.Sizes.Select(s => photo.GetFilename(s)));

        await fileDeleter.Delete(bucket.Photos, filesToDelete);
    }
}