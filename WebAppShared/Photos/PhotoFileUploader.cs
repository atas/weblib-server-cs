using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using WebAppShared.Exceptions;
using WebAppShared.MVC;
using WebAppShared.Uploaders;
using WebAppShared.WebSys.DI;

namespace WebAppShared.Photos;

/// <summary>
/// Main class responsible of uploading photos to the storage bucket.
/// Resizes to the max allowed type. Checks if the extension is allowed.
/// Converts to the correct MIME type if photo is not
/// </summary>
/// <param name="fileUploader"></param>
/// <param name="bucket"></param>
/// <param name="logger"></param>
[Service(InterfaceToBind = typeof(IPhotoFileUploader)), UsedImplicitly]
public class PhotoFileUploader(IFileUploader fileUploader, IStorageBucket bucket, ILogger<PhotoFileUploader> logger)
    : IPhotoFileUploader
{
    private Stream _stream;

    private IImageFormat _imageFormat;
    private Guid? _photoId;

    /// <summary>
    /// Because Facebook OpenGraph does not support WebP, yet, we use JPG as default.
    /// </summary>
    private string _fileExt = "jpg";

    private int _maxWidthHeight = 2048;

    private bool _disposeStream;

    /// <summary>
    /// Gets the uploaded photo's width
    /// </summary>
    public int PhotoWidth { get; set; }

    /// <summary>
    /// Gets the uploaded photo's height
    /// </summary>
    public int PhotoHeight { get; set; }

    /// <summary>
    /// Gets the file extension used to upload the file
    /// </summary>
    public string Ext => _fileExt ?? throw new HttpJsonError("Photo extension is not set.");

    /// <summary>
    /// Overrides the default photo extension JPG.
    /// </summary>
    /// <param name="ext"></param>
    /// <returns></returns>
    /// <exception cref="HttpJsonError"></exception>
    public IPhotoFileUploader SetExtension(string ext)
    {
        if (!CommonPhotoUtils.IsExtensionAllowed(ext)) throw new HttpJsonError("This photo extension is not allowed.");

        _fileExt = ext;
        return this;
    }

    /// <summary>
    /// Sets the extension to the original file extension.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public IPhotoFileUploader SetExtensionByFileName(string fileName)
    {
        SetExtension(Path.GetExtension(fileName).Replace(".", ""));
        return this;
    }

    public IPhotoFileUploader SetMaxWidthHeight(int maxWidthHeight)
    {
        _maxWidthHeight = maxWidthHeight;
        return this;
    }

    private string GetFilename() => _photoId!.Value + "." + Ext;

    public IPhotoFileUploader SetSource(Stream stream)
    {
        _imageFormat = Image.DetectFormat(stream);
        _stream = stream;

        using (var img = Image.Load(stream))
        {
            PhotoWidth = img.Width;
            PhotoHeight = img.Height;
        }

        return this;
    }

    public IPhotoFileUploader SetSource(string filePath)
    {
        _stream?.Dispose();
        _disposeStream = true;
        _stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        SetSource(_stream);
        return this;
    }

    public async Task<PhotoFileUploaderResult> Upload()
    {
        _photoId = Guid.NewGuid();

        if (PhotoWidth > _maxWidthHeight || PhotoHeight > _maxWidthHeight)
            await ResizeToMaxAllowedSize();
        // If we have not resized the image, check it's in the correct type. Generally, we will convert to jpg.
        else if (_imageFormat.DefaultMimeType != CommonPhotoUtils.GetMimeTypeByExt(Ext))
            ConvertPhotoToExtensionType();

        await fileUploader.Upload(_stream, GetFilename(), _imageFormat.DefaultMimeType, bucket.Photos);

        return new PhotoFileUploaderResult
        {
            Ext = Ext,
            Width = PhotoWidth,
            Height = PhotoHeight,
            FullFileName = GetFilename(),
            Id = _photoId!.Value
        };
    }

    private void ConvertPhotoToExtensionType()
    {
        logger.LogInformation("Converting photo {PhotoId} format to {Format}", _photoId, Ext);

        _stream.Seek(0, SeekOrigin.Begin);
        using var img = Image.Load(_stream);

        if (_disposeStream) _stream?.Dispose();

        _stream = new MemoryStream();
        _disposeStream = true;

        img.Save(_stream, CommonPhotoUtils.GetEncoder(Ext));
    }

    private async Task ResizeToMaxAllowedSize()
    {
        logger.LogInformation("Resizing photo {Id} before uploading", _photoId);

        _stream.Seek(0, SeekOrigin.Begin);
        using var img = await Image.LoadAsync(_stream);
        img.Mutate(i => i.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Position = AnchorPositionMode.Center,
            Size = new Size(_maxWidthHeight)
        }));

        // Dispose the old stream if we should.
        if (_disposeStream) _stream?.Dispose();

        // Now we have a new stream, definitely dispose it later.
        _disposeStream = true;

        _stream = new MemoryStream();

        await img.SaveAsync(_stream, CommonPhotoUtils.GetEncoder(Ext));

        PhotoWidth = img.Width;
        PhotoHeight = img.Height;
    }

    public void Dispose()
    {
        if (_disposeStream) _stream?.Dispose();
    }
}