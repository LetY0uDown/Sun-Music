using ATL;
using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Models.Database;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Desktop_Client.Resources.UserControls;

[Lifetime(Lifetime.Singleton)]
public partial class MusicPlayer : UserControl, IMusicPlayer, INotifyPropertyChanged
{
    private readonly IFileManager _fileManager;

    private MusicTrack _trackModel;

    private Track _track;

    public MusicPlayer(IFileManager fileManager)
    {
        InitializeComponent();
        
        DataContext = this;

        _fileManager = fileManager;
    }

    public string Title { get; private set; }

    public string Artist { get; private set; }

    public byte[] TrackPicture { get; private set; }

    public MusicTrack MusicTrack
    {
        get => _trackModel;
        set {
            Title = value.Title;
            Artist = value.ArtistName;
            TrackPicture = value.ImageBytes;

            Task.Run(async () => {
                var stream = await _fileManager.DownloadStream(_trackModel.ID, "/Tracks/File/");

                _track = new(stream);
            });

            _trackModel = value;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void ContinuePlaying()
    {
        throw new System.NotImplementedException();
    }

    public void DownloadCurrentTrack()
    {
        _track.Save();
    }

    public void OrderPlaylist()
    {
        throw new System.NotImplementedException();
    }

    public void Pause()
    {
        throw new System.NotImplementedException();
    }

    public void PlayNext()
    {
        throw new System.NotImplementedException();
    }

    public void PlayPrevious()
    {
        throw new System.NotImplementedException();
    }

    public void RepeatCurrentTrack(bool repeat)
    {
        throw new System.NotImplementedException();
    }

    public void SetPlaylist(LinkedList<MusicTrack> playlist)
    {
        throw new System.NotImplementedException();
    }

    public void SetTrack(MusicTrack track)
    {
        MusicTrack = track;
    }

    public void SetVolume(int volume)
    {
        throw new System.NotImplementedException();
    }

    public void ShufflePlaylist()
    {
        throw new System.NotImplementedException();
    }

    private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {

    }
}