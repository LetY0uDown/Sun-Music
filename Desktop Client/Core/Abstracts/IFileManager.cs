using System.IO;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

public interface IFileManager : IService
{
    Task<Stream> DownloadStream(string fileID, string path);

    Task UploadFileAsync(string filePath, string path);
}