using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using Material.Icons;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Desktop_Client.Views.Windows;

[Singleton]
public partial class MainWindow : Window
{
    private double _rowHeight;
    private double _rowsCount;
    private int _selectedRowIndex = 0;

    private bool _isPlayingTrack = false;

    public MainWindow (MainViewModel viewModel, INavigationService navigation)
    {
        InitializeComponent();

        DataContext = viewModel;

        navigation.SetViewModel(viewModel);

        _rowsCount = NavBar.RowDefinitions.Count;
    }

    private void Window_SizeChanged (object sender, SizeChangedEventArgs e)
    {
        _rowHeight = NavigationCanvas.ActualHeight / _rowsCount;
        SelectionFlag.Height = _rowHeight;

        Canvas.SetTop(SelectionFlag, _selectedRowIndex * _rowHeight);
    }

    private void NavButton_Click (object sender, RoutedEventArgs e)
    {
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

    private void PlayPauseButton_Click (object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        _isPlayingTrack = !_isPlayingTrack;

        PlayPauseIcon.Kind = _isPlayingTrack ? MaterialIconKind.Play
                                             : MaterialIconKind.Pause;
    }
}