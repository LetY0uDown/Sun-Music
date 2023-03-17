using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop_Client.Views.Windows;

[Transient]
public partial class TrackUploadingWindow : Window, INavigationWindow
{
    public TrackUploadingWindow()
    {

    }

    public Task Display()
    {
        InitializeComponent();

        Show();

        return Task.CompletedTask;
    }

    Task INavigationWindow.Hide()
    {
        Hide();

        return Task.CompletedTask;
    }
}