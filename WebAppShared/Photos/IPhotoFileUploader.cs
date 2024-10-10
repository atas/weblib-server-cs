using System;
using System.IO;
using System.Threading.Tasks;

namespace WebAppShared.Photos;

public interface IPhotoFileUploader : IDisposable
{
	IPhotoFileUploader SetExtension(string ext);
	IPhotoFileUploader SetExtensionByFileName(string fileName);
	IPhotoFileUploader SetMaxWidthHeight(int maxWidthHeight);
	IPhotoFileUploader SetSource(Stream stream);
	IPhotoFileUploader SetSource(string filePath);

	Task<PhotoFileUploaderResult> Upload();
}
