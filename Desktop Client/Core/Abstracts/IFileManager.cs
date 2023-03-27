using System.IO;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

public interface IFileManager : IService
{
    Task<Stream> DownloadStream();

    Task SendFileAsync(string filePath, string url);
}