using System.IO;
using System.Threading.Tasks;

namespace WebLibServer.Uploaders;

public interface IFileUploader
{
	Task Upload(Stream file, string name, string contentType, string container);

	Task Upload(string file, string name, string contentType, string container);

	Task<Stream> Download(string name, string bucket);
}
