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
using System.IO;
using System.Windows.Media.Imaging;

namespace Desktop_Client.Core.ViewModels;

[Transient]
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

        RedirectToLoginCommand = new(o => {
            _navigation.SetCurrentPage<LoginPage>();
        });

        RegistrationCommand = new(async o => {
            User user = new()
            {
                ID = string.Empty,
                Username = Username,
                Password = Password,
                ImageBytes = GetBytesFromImage(ProfilePicture)
            };

            var authData = await _apiClient.PostAsync<User, AuthorizeData>(user, "Register");

            if (authData is not null)
            {
                App.AuthorizeData = authData;
                _navigation.SetMainWindow<MainWindow>();
            }


        }, b => !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(Password));

        SelectPictureCommand = new(o => {
            var fileDialog = new OpenFileDialog {
                Filter = _config["ImagesFilter"]
            };

            if (fileDialog.ShowDialog() == true) {
                BitmapImage picture = new();

                picture.BeginInit();

                picture.UriSource = new(fileDialog.FileName);
                picture.CacheOption = BitmapCacheOption.OnLoad;

                picture.EndInit();

                ProfilePicture = picture;
            }
        });
    }

    public UICommand RegistrationCommand { get; private init; }

    public UICommand RedirectToLoginCommand { get; private init; }

    public UICommand SelectPictureCommand { get; private init; }

    public string Username { get; set; }

    public string Password { get; set; }

    public BitmapImage ProfilePicture { get; set; }

    private static byte[] GetBytesFromImage(BitmapImage image)
    {
        if (image is null) return Array.Empty<byte>();

        JpegBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(image));

        byte[] imageBytes;

        using (MemoryStream stream = new())
        {
            encoder.Save(stream);
            imageBytes = stream.ToArray();
        }

        return imageBytes;
    }
}