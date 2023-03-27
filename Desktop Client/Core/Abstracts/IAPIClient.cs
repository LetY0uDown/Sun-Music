using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

public interface IAPIClient : IService
{
    Task<T> GetAsync<T>(string url) where T : class;

    Task<TResponse> PostAsync<TEntity, TResponse>(TEntity value, string url) where TEntity : class;

    Task<TResponse> PutAsync<TEntity, TResponse>(TEntity value, string url) where TEntity : class;

    Task<bool> DeleteAsync(string uri);
}