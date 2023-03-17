using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;

namespace Desktop_Client.Core.ViewModels;

[Transient]
public class TrackUploadingViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;

    public TrackUploadingViewModel(IAPIClient apiClient)
    {
        _apiClient = apiClient;
    }
}