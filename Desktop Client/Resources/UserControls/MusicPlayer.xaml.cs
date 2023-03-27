using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_Client.Resources.UserControls;

[Lifetime(Lifetime.Singleton)]
public partial class MusicPlayer : UserControl
{
    private readonly IMusicPlayer _musicPlayer;

    public MusicPlayer(IMusicPlayer musicPlayer)
    {
        InitializeComponent();

        _musicPlayer = musicPlayer;
    }

    private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {

    }
}