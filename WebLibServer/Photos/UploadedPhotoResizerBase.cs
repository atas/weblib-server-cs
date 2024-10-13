using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using WebLibServer.Http;
using WebLibServer.Uploaders;
using WebLibServer.WebSys.DI;

namespace WebLibServer.Photos;

[UsedImplicitly]
public abstract class UploadedPhotoResizerBase(
    string mediaHost,
    IFileUploader uploader,
    ILogger<UploadedPhotoResizerBase> logger,
    IStorageBucket bucket)
{
    /// <summary>
    ///     Resize a given photo uploaded to cloud with the given max size by width and/or height
    /// </summary>
    /// <param name="photoId"></param>
    /// <param name="maxSize"></param>
    public async Task<ISharedPhoto> Resize(Guid photoId, int maxSize)
    {
        var photo = GetPhoto(photoId);

        await using var stream = await uploader.Download($"{photo.Id}.{photo.Ext}", bucket.Photos);
        byte[] imgBytes;
        using (var s = new MemoryStream())
        {
            await stream.CopyToAsync(s);
            imgBytes = s.ToArray();
        }

        using var img = Image.Load(imgBytes);
        img.Mutate(i => i.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Crop,
            Position = AnchorPositionMode.Center,
            Size = new Size(maxSize)
        }));

        var ms = new MemoryStream();

        await img.SaveAsync(ms, CommonPhotoUtils.GetEncoder(photo.Ext));


        var resizedPhotoUploadName = $"{photo.Id}-{maxSize}.{photo.Ext}";
        logger.LogInformation("Uploading resized photo to {Url}", resizedPhotoUploadName);

        await uploader.Upload(ms, resizedPhotoUploadName, CommonPhotoUtils.GetMimeTypeByExt(photo.Ext),
            bucket.Photos);

        if (photo.Sizes == null)
            photo.Sizes = new List<int>(new[] { maxSize });
        else if (!photo.Sizes.Contains(maxSize))
            photo.Sizes.Add(maxSize);

        SaveChangesToPhoto(photo);

        return photo;
    }

    protected abstract ISharedPhoto GetPhoto(Guid photoId);
    protected abstract void SaveChangesToPhoto(ISharedPhoto photo);
}