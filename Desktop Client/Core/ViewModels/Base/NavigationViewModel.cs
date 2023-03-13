using System.Windows.Controls;

namespace Desktop_Client.Core.ViewModels.Base;

public abstract class NavigationViewModel : ViewModel
{
    public Page CurrentPage { get; set; }
}