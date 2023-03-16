﻿using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using Desktop_Client.Views.Pages;
using System.Windows;

namespace Desktop_Client.Views.Windows;

[Transient]
public partial class AuthWindow : Window, INavigationWindow
{
    private readonly AuthNavigationViewModel _viewModel;

    private readonly INavigationService _navigation;

    public AuthWindow (AuthNavigationViewModel viewModel, INavigationService navigation)
    {
        _viewModel = viewModel;
        _navigation = navigation;
    }

    public void Display ()
    {
        InitializeComponent();

        Show();

        DataContext = _viewModel;

        _navigation.SetViewModel(_viewModel);

        _navigation.SetCurrentPage<RegistrationPage>();
    }

    void INavigationWindow.Hide ()
    {
        Hide();
    }

    private void MinimizeButton_Click (object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click (object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Titlebar_LeftMouseButtonDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }
}