﻿using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Singleton]
public partial class TracksPage : Page, INavigationPage
{
    private readonly TracksViewModel _viewModel;

    public TracksPage (TracksViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public async Task Display ()
    {
        InitializeComponent();

        await _viewModel.Initialize();
        DataContext = _viewModel;
    }
}