using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebLibServer.Uploaders;

public interface IFileDeleter
{
	Task Delete(string bucket, string fileName);
	Task Delete(string bucket, List<string> fileNames);
}
