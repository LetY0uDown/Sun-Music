using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Transient]
public partial class UserProfilePage : Page, INavigationPage
{
    public UserProfilePage()
    {
        InitializeComponent();
    }

    public void Display()
    {
        throw new System.NotImplementedException();
    }
}