using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Microsoft.Extensions.Configuration;
using Models.Database;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Services;

[Lifetime(Lifetime.Transient)]
internal sealed class APIClient : IAPIClient
{
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _hostURL;
    private readonly IConfiguration _config;

    public APIClient (IConfiguration config)
    {
        _config = config;
        _hostURL = _config["HostURL:HTTP"];
    }

    public async Task<bool> DeleteAsync (string url)
    {
        bool isSuccess = false;

        using (RestClient client = new(_hostURL)) {
            client.Authenticator = new JwtAuthenticator(App.AuthorizeData.Token);
            RestRequest request = new(url, Method.Delete);

            try {
                var response = await client.DeleteAsync(request);

                isSuccess = response.IsSuccessful;
            }
            catch (Exception e) {
                InfoBox.Show(e.Message, "Error");
            }
        }

        return isSuccess;
    }

    public async Task<TEntity> GetAsync<TEntity> (string url) where TEntity : class
    {
        TEntity entity = default;

        using (RestClient client = new(_hostURL)) {
            client.Authenticator = new JwtAuthenticator(App.AuthorizeData.Token);
            RestRequest request = new(url, Method.Get);

            try {
                var response = await client.GetAsync(request);

                if (!response.IsSuccessStatusCode) {
                    //InfoBox.Show(response.Content, response.StatusCode.ToString());

                    return default;
                }

                entity = JsonSerializer.Deserialize<TEntity>(response.Content, _jsonOptions);
            }
            catch (Exception e) {
                InfoBox.Show(e.Message, "Error");
            }
        }

        return entity;
    }

    public async Task<TResponse> PostAsync<TEntity, TResponse> (TEntity value, string url) where TEntity : class
    {
        TResponse resp = default;

        using (RestClient client = new(_hostURL)) {
            if (App.AuthorizeData is not null) {
                client.Authenticator = new JwtAuthenticator(App.AuthorizeData.Token);
            }

            RestRequest request = new RestRequest(url, Method.Post);

            if (value is not null) {
                request.AddJsonBody(value);
            }

            try {
                var response = await client.PostAsync(request);

                if (!response.IsSuccessStatusCode) {
                    InfoBox.Show(response.Content, response.StatusCode.ToString());

                    return default;
                }

                if (string.IsNullOrWhiteSpace(response.Content)) {
                    return default;
                }

                resp = JsonSerializer.Deserialize<TResponse>(response.Content, _jsonOptions);
            }
            catch (Exception e) {
                InfoBox.Show(e.Message, "Error");
            }
        }

        return resp;
    }

    public async Task<TResponse> PutAsync<TEntity, TResponse> (TEntity value, string url) where TEntity : class
    {
        TResponse resp = default;

        using (RestClient client = new(_hostURL)) {
            client.Authenticator = new JwtAuthenticator(App.AuthorizeData.Token);
            RestRequest request = new RestRequest(url, Method.Put).AddJsonBody(value);

            try {
                var response = await client.PutAsync(request);

                if (!response.IsSuccessStatusCode) {
                    InfoBox.Show(response.Content, response.StatusCode.ToString());

                    return default;
                }

                resp = JsonSerializer.Deserialize<TResponse>(response.Content, _jsonOptions);
            }
            catch (Exception e) {
                InfoBox.Show(e.Message, "Error");
            }
        }

        return resp;
    }
}