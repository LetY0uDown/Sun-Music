﻿using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using Desktop_Client.Properties;
using Material.Icons;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Desktop_Client.Views.Windows;

[Lifetime(Lifetime.Transient)]
public partial class MainWindow : Window, INavigationWindow
{
    private double _rowHeight;
    private double _rowsCount;
    private int _selectedRowIndex = 0;

    private readonly MainNavigationViewModel _viewModel;
    private readonly INavigationService _navigation;

    public MainWindow (MainNavigationViewModel viewModel, INavigationService navigation)
    {
        _viewModel = viewModel;
        _navigation = navigation;
    }

    public async Task Display ()
    {
        InitializeComponent();

        Show();

        _rowsCount = NavBar.RowDefinitions.Count;
        
        _rowHeight = NavigationCanvas.ActualHeight / _rowsCount;
        SelectionFlag.Height = _rowHeight;

        DataContext = _viewModel;

        _navigation.SetViewModel(_viewModel);

        await _viewModel.Display();
    }

    async Task INavigationWindow.Hide ()
    {
        Hide();

        await _viewModel.Leave();
    }

    private void NavButton_Click (object sender, RoutedEventArgs e)
    {
        // Canvas.SetTop(SelectionFlag, _selectedRowIndex * _rowHeight);

        var button = sender as RadioButton;
        _selectedRowIndex = Grid.GetRow(button);

        var newBG = (SolidColorBrush)button.Foreground;

        var backgroundAnim = new ColorAnimation() {
            To = newBG.Color,
            Duration = TimeSpan.FromMilliseconds(150)
        };

        Storyboard.SetTargetProperty(backgroundAnim, new("(Border.Background).(SolidColorBrush.Color)"));
        Storyboard.SetTarget(backgroundAnim, SelectionFlag);

        var positionAnim = new DoubleAnimation() {
            To = _selectedRowIndex * _rowHeight,
            Duration = TimeSpan.FromMilliseconds(150)
        };

        Storyboard.SetTargetProperty(positionAnim, new("(Canvas.Top)"));
        Storyboard.SetTarget(positionAnim, SelectionFlag);

        var sb = new Storyboard();
        sb.Children.Add(positionAnim);
        sb.Children.Add(backgroundAnim);
        sb.Begin();
    }

    private void TitleBar_LeftMouseDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (Settings.Default.IsWindowDraggable)
            DragMove();
    }

    private void MinimizeButton_Click (object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click (object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OptionsButton_Checked (object sender, RoutedEventArgs e)
    {
        SelectionFlag.Visibility = Visibility.Hidden;
    }

    private void OptionsButton_Unchecked (object sender, RoutedEventArgs e)
    {
        SelectionFlag.Visibility = Visibility.Visible;
    }
}