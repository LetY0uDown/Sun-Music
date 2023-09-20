using ATL;
using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Pages;
using Desktop_Client.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Models.Client;
using Models.Database;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Desktop_Client.Core.ViewModels.Auth;

[Lifetime(Lifetime.Transient)]
public sealed class RegistrationViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;
    private readonly INavigationService _navigation;
    private readonly IConfiguration _config;

    public RegistrationViewModel(IAPIClient apiClient, INavigationService navigation, IConfiguration config)
    {
        _apiClient = apiClient;
        _navigation = navigation;
        _config = config;
    }

    public UICommand RegistrationCommand { get; private set; }

    public UICommand RedirectToLoginCommand { get; private set; }

    public UICommand SelectPictureCommand { get; private set; }

    public bool DoRememberData { get; set; } = Properties.Settings.Default.DoRememberData;

    public string Username { get; set; } = Properties.Settings.Default.SavedUsername;

    public string Password { get; set; } = Properties.Settings.Default.SavedPassword;

    public BitmapImage ProfilePicture { get; set; }

    public override Task Display()
    {
        RedirectToLoginCommand = new(o => {
            _navigation.SetCurrentPage<LoginPage>();
        });

        RegistrationCommand = new(async o => {
            User user = new() {
                ID = Guid.Empty,
                Username = Username,
                Password = Password,
                ImageBytes = ImageConverter.BytesFromImage(ProfilePicture)
            };

            var authData = await _apiClient.PostAsync<User, AuthorizeData>(user, "Register");

            if (authData is not null) {
                App.AuthorizeData = authData;
                await _navigation.SetMainWindow<MainWindow>();

                SaveData();
            }
        }, b => !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(Password) &&
                ProfilePicture is not null);

        SelectPictureCommand = new(o => {
            var fileDialog = new OpenFileDialog {
                Filter = _config["Filters:Images"]
            };

            if (fileDialog.ShowDialog() == true) {
                ProfilePicture = ImageConverter.CreateImageFromFile(fileDialog.FileName);
            }
        });

        return base.Display();
    }

    private void SaveData ()
    {
        Properties.Settings.Default.DoRememberData = DoRememberData;

        if (DoRememberData) {
            Properties.Settings.Default.SavedPassword = Password;
            Properties.Settings.Default.SavedUsername = Username;
        } else {
            Properties.Settings.Default.SavedPassword = string.Empty;
            Properties.Settings.Default.SavedUsername = string.Empty;
        }

        Properties.Settings.Default.Save();
    }
}