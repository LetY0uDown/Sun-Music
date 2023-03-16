using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;

namespace Desktop_Client.Core.ViewModels;

[Singleton]
internal class TracksViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;

    public TracksViewModel (IAPIClient apiClient)
    {
        _apiClient = apiClient;
    }
}