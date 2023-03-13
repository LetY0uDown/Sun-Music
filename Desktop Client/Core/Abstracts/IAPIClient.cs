using Desktop_Client.Core.Tools.Attributes;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

[Singleton]
public interface IAPIClient
{
    Task<T> GetAsync<T> (string url) where T : class;

    Task<TResponse> PostAsync<TEntity, TResponse> (TEntity value, string url) where TEntity : class;

    Task<bool> SendFileAsync(string filePath, string url);

    Task<TResponse> PutAsync<TEntity, TResponse> (TEntity value, string url) where TEntity : class;

    Task<bool> DeleteAsync (string url);
}