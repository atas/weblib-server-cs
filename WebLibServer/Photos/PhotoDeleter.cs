using JetBrains.Annotations;
using WebLibServer.Exceptions;
using WebLibServer.Uploaders;
using WebLibServer.WebSys.DI;

namespace WebLibServer.Photos;

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