using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Services;

[Lifetime(Lifetime.Transient)]
internal class FileManager : IFileManager
{
    private readonly IConfiguration _config;
    private readonly string _hostURL;

    public FileManager(IConfiguration config)
    {
        _config = config;

        _hostURL = _config["HostURL:HTTP"];
    }

    public Task<Stream> DownloadStream()
    {
        throw new NotImplementedException();
    }

    public async Task SendFileAsync(string filePath, string uri)
    {
        using (RestClient client = new(_hostURL))
        {
            RestRequest request = new RestRequest(uri, Method.Post).AddFile("file", filePath, "multipart/form-data");

            try {
                var response = await client.PostAsync(request);
            }
            catch (Exception e) {
                InfoBox.Show(e.Message, "Error");
            }
        }
    }
}