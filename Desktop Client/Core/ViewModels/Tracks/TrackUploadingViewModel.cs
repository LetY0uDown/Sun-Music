using ATL;
using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Models.Database;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Desktop_Client.Core.ViewModels.Tracks;

[Lifetime(Lifetime.Transient)]
public class TrackUploadingViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;
    private readonly IConfiguration _config;

    private MusicTrack _musicTrack;

    public TrackUploadingViewModel (IAPIClient apiClient, IConfiguration config)
    {
        _apiClient = apiClient;
        _config = config;
    }

    public UICommand SelectFileCommand { get; private set; }

    public UICommand SelectImageCommand { get; private set; }

    public UICommand UploadTrackCommand { get; private set; }

    public string FilePath { get; set; }

    public string Title { get; set; }

    public string Artist { get; set; }

    public string Album { get; set; }

    public BitmapImage TrackImage { get; private set; }

    public override Task Display ()
    {
        SelectFileCommand = new(o => {
            OpenFileDialog fileDialog = new() {
                Filter = _config["Filters:Music"]
            };

            if ((bool)fileDialog.ShowDialog()) {
                FilePath = fileDialog.FileName;

                Track track = new(fileDialog.FileName);
                
                Title = track.Title;
                Artist = track.Artist;
                Album = track.Album;

                _musicTrack = new() {
                    Title = Title,
                    ArtistName = Artist,
                    AlbumName = Album,
                    DurationMs = (int)track.DurationMs,
                    ID = Guid.Empty.ToString(),
                    FileName = fileDialog.FileName,
                    ReleaseDate = DateTime.Now
                };
            }
        });

        SelectImageCommand = new(o => {
            if (_musicTrack is null) {
                InfoBox.Show("Сначала выбирете трек");
                return;
            }

            var fileDialog = new OpenFileDialog {
                Filter = _config["Filters:Images"]
            };

            if (fileDialog.ShowDialog() == true) {
                TrackImage = ImageConverter.CreateImageFromFile(fileDialog.FileName);

                _musicTrack.ImageBytes = ImageConverter.BytesFromImage(TrackImage);
            }
        });

        UploadTrackCommand = new(async o => {
            _musicTrack.Title = Title;
            _musicTrack.AlbumName = Album;
            _musicTrack.ArtistName = Artist;

            var id = await _apiClient.PostAsync<MusicTrack, string>(_musicTrack, "Tracks/Upload");

            if (id is not null) {
                await _apiClient.SendFileAsync(FilePath, $"Tracks/Upload/File/{id}");
            }
        });

        return base.Display();
    }
}