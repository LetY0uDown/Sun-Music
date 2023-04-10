using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.Tools.Extensions;
using Microsoft.Extensions.Configuration;
using Models.Database;
using ATL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Material.Icons;

namespace Desktop_Client.Resources.UserControls;

[Lifetime(Lifetime.Singleton)]
public partial class MusicPlayer : UserControl, IMusicPlayer, INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer = new() {
        Interval = TimeSpan.FromSeconds(1)
    };

    private bool _isPaused;

    private bool _isPlaylistShuffled;
    private bool _isRepeatingTrack;

    private int _currentIndex;
    private int _indexIncrement = 0;

    private readonly IFileManager _fileManager;
    private readonly IConfiguration _config;

    private MusicTrack _trackModel;
    private Track _trackInfo;

    private readonly MediaPlayer _player = new();

    private List<MusicTrack> _playlistOrigin;

    public MusicPlayer (IFileManager fileManager, IConfiguration config)
    {
        InitializeComponent();

        DataContext = this;

        _fileManager = fileManager;
        _config = config;
    }

    public bool IsPlaylistShuffled
    {
        get => _isPlaylistShuffled;
        set {
            _isPlaylistShuffled = value;

            if (_isPlaylistShuffled) {
                ShufflePlaylist();
            } else {
                OrderPlaylist();
            }
        }
    }

    public bool IsPaused
    {
        get => _isPaused;
        set {
            _isPaused = value;

            PlayPauseIcon.Kind = _isPaused ? MaterialIconKind.Pause 
                                           : MaterialIconKind.Play;
        }
    }

    public bool IsRepeating
    {
        get => _isRepeatingTrack;
        set {
            _isRepeatingTrack = value;

            _indexIncrement = _isRepeatingTrack ? 0 : 1;
        }
    }

    public int ProgressMaximum { get; private set; }

    public int ProgressValue { get; set; }

    public string TimeElapsed { get; private set; }

    public string TotalTrackTime { get; private set; }

    public UICommand PlayCommand { get; private set; }

    public UICommand PlayNextCommand { get; private set; }

    public UICommand PlayPrevCommand { get; private set; }

    public UICommand ShufflePlaylistCommand { get; private set; }

    public UICommand RepeatTrackCommand { get; private set; }

    public List<MusicTrack> Playlist { get; set; }

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

            PlaceholderImage.Visibility = TrackPicture == null ? Visibility.Visible
                                                               : Visibility.Hidden;

            _trackModel = value;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public async Task Initialize ()
    {
        _timer.Tick += TimerTick;

        _player.MediaEnded += TrackEnded;

        PlayCommand = new(o => {
            IsPaused = !IsPaused;

            if (_isPaused) {
                _player.Pause();
                _timer.Stop();
            } else {
                _player.Play();
                _timer.Start();
            }
        }, b => MusicTrack is not null);

        PlayNextCommand = new(o => {
            PlayNext();
        }, b => MusicTrack is not null &&
                Playlist is not null &&
                _currentIndex != Playlist.Count - 1);

        PlayPrevCommand = new(o => {
            PlayPrevious();
        }, b => MusicTrack is not null &&
                Playlist is not null &&
                _currentIndex > 0);

        ShufflePlaylistCommand = new(o => { },
                                     b => Playlist is not null);

        RepeatTrackCommand = new(o => { },
                                 b => MusicTrack is not null);
    }

    private void TrackEnded (object sender, EventArgs e)
    {
        if (_indexIncrement == 0 || _currentIndex != Playlist?.Count - 1) {
            PlayTrack(_currentIndex + _indexIncrement);
        }
    }

    public void ContinuePlaying ()
    {
        _player.Play();

        IsPaused = false;
    }

    public void Pause ()
    {
        _player.Pause();

        IsPaused = true;
    }

    public void OrderPlaylist ()
    {
        _player.Close();

        Playlist = _playlistOrigin;

        _currentIndex = 0;

        PlayTrack(_currentIndex);
    }

    public void ShufflePlaylist ()
    {
        _player.Close();

        Playlist = Playlist.Shuffle();

        _currentIndex = 0;

        PlayTrack(_currentIndex);
    }

    public void PlayNext ()
    {
        _player.Close();

        PlayTrack(++_currentIndex);
    }

    public void PlayPrevious ()
    {
        _player.Close();

        PlayTrack(--_currentIndex);
    }

    public void RepeatCurrentTrack (bool repeat)
    {
        IsRepeating = repeat;
    }

    public void SetPlaylist (IEnumerable<MusicTrack> playlist)
    {
        IsPlaylistShuffled = false;

        _playlistOrigin = playlist.ToList();

        Playlist = _playlistOrigin;

        _currentIndex = 0;

        IsRepeating = false;

        if (Playlist.Contains(MusicTrack)) {
            _currentIndex = Playlist.IndexOf(MusicTrack);
        }

        PlayTrack(_currentIndex);
    }
    
    public async Task SetTrack (MusicTrack track)
    {
        _player.Stop();
        _player.Close();

        _timer.Stop();

        Playlist = null;

        MusicTrack = track;

        Directory.CreateDirectory(_config["DownloadedMusicPath"]);

        var localPath = _config["DownloadedMusicPath"] + "\\" + track.FileName;

        if (!File.Exists(localPath)) {
            var stream = await _fileManager.DownloadStream(MusicTrack.ID, "Tracks/File");

            using (FileStream fs = new(localPath, FileMode.Create)) {
                stream?.CopyTo(fs);
            }
        }

        OpenTrack(track.FileName);

        _player.Volume = VolumeSlider.Value / 100;

        _timer.Start();
        _player.Play();

        IsPaused = false;
    }

    public void SetVolume (double volume)
    {
        _player.Volume = volume;
    }

    private void TimerTick (object sender, EventArgs e)
    {
        TimeElapsed = _player.Position.ToString("mm\\:ss");
        ProgressValue++;
    }

    private void OpenTrack (string fileName)
    {
        var localPath = _config["DownloadedMusicPath"] + "\\" + fileName;
        _player.Open(new(localPath, UriKind.Absolute));

        _trackInfo = new(localPath);

        TotalTrackTime = TimeSpan.FromSeconds(_trackInfo.Duration).ToString("mm\\:ss");

        ProgressMaximum = _trackInfo.Duration;

        ProgressValue = 0;
    }

    private void PlayTrack (int index)
    {
        MusicTrack = Playlist[index];

        OpenTrack(MusicTrack.FileName);
        _player.Play();
        _timer.Start();

        IsPaused = false;
    }

    private void VolumeSlider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        SetVolume(e.NewValue / 100);
    }
}