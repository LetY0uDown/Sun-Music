using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
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

    public async Task<Stream> DownloadStream(Guid fileID, string path)
    {
        var fullPath = Path.Combine(path, fileID.ToString());

        Stream stream = null!;

        using (RestClient client = new(_hostURL)) {
            client.Authenticator = new JwtAuthenticator(App.AuthorizeData.Token);
            RestRequest request = new(fullPath, Method.Get);

            try {
                stream = await client.DownloadStreamAsync(request);
            } catch (Exception e) {
                InfoBox.Show(e.Message, "Error");
            }
        }

        return stream;
    }

    public async Task UploadFileAsync(string filePath, string path)
    {
        using (RestClient client = new(_hostURL)) {
            client.Authenticator = new JwtAuthenticator(App.AuthorizeData.Token);
            RestRequest request = new RestRequest(path, Method.Post).AddFile("file", filePath, "multipart/form-data");

            try {
                var response = await client.PostAsync(request);
            } catch (Exception e) {
                InfoBox.Show(e.Message, "Error");
            }
        }
    }
}