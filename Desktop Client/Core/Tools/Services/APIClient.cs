using Desktop_Client.Core.Abstracts;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Tools.Services;

internal class APIClient : IAPIClient
{
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _hostURL;
    private readonly IConfiguration _config;

    public APIClient(IConfiguration config)
    {
        _config = config;
        _hostURL = _config["HostURL:HTTP"];
    }

    public async Task<bool> DeleteAsync(string url)
    {
        bool isSuccess;

        using (RestClient client = new(_hostURL))
        {
            RestRequest request = new(url);

            var response = await client.DeleteAsync(request);

            isSuccess = response.IsSuccessful;
        }

        return isSuccess;
    }

    public async Task<TEntity> GetAsync<TEntity>(string url) where TEntity : class
    {
        TEntity entity;

        using (RestClient client = new(_hostURL))
        {
            RestRequest request = new(url);

            var response = await client.GetAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                InfoBox.Show(response.Content, response.StatusCode.ToString());
            }

            entity = JsonSerializer.Deserialize<TEntity>(response.Content, _jsonOptions);
        }

        return entity;
    }

    public async Task<TResponse> PostAsync<TEntity, TResponse>(TEntity value, string url) where TEntity : class
    {
        TResponse resp;

        using (RestClient client = new(_hostURL))
        {
            RestRequest request = new RestRequest(url).AddJsonBody(value);

            var response = await client.PostAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                InfoBox.Show(response.Content, response.StatusCode.ToString());
            }

            resp = JsonSerializer.Deserialize<TResponse>(response.Content, _jsonOptions);
        }

        return resp;
    }

    public async Task<TResponse> PutAsync<TEntity, TResponse>(TEntity value, string url) where TEntity : class
    {
        TResponse resp;

        using (RestClient client = new(_hostURL))
        {
            RestRequest request = new RestRequest(url).AddJsonBody(value);

            var response = await client.PutAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                InfoBox.Show(response.Content, response.StatusCode.ToString());
            }

            resp = JsonSerializer.Deserialize<TResponse>(response.Content, _jsonOptions);
        }

        return resp;
    }

    public async Task<bool> SendFileAsync(string filePath, string url)
    {
        bool isSuccess;

        using (RestClient client = new(_hostURL))
        {
            RestRequest request = new RestRequest(url).AddFile("file", filePath, "multipart/form-data");

            var response = await client.PostAsync(request);

            isSuccess = response.IsSuccessful;
        }

        return isSuccess;
    }
}