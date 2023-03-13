using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using System;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Transient]
public partial class UsersPage : Page, INavigationPage
{
    public UsersPage()
    {
        InitializeComponent();
    }

    public void Display()
    {
        throw new NotImplementedException();
    }
}