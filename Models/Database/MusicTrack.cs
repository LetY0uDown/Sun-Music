using System.Text.Json.Serialization;

namespace Models.Database;

public partial class MusicTrack : Entity
{
    public MusicTrack ()
    {
        PlaylistTracks = new HashSet<PlaylistTrack>();
        TrackLikes = new HashSet<TrackLike>();
    }

    public string Title { get; set; } = null!;
    public string ArtistName { get; set; } = null!;
    public string AlbumName { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public byte[] ImageBytes { get; set; } = null!;
    public string FileName { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }
    [JsonIgnore]
    public virtual ICollection<TrackLike> TrackLikes { get; set; }
}
