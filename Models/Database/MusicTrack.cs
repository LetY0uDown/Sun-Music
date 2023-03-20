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
    public int DurationMs { get; set; }
    public DateTime ReleaseDate { get; set; }
    public byte[] ImageBytes { get; set; } = null!;
    public string FileName { get; set; } = null!;

    public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }
    public virtual ICollection<TrackLike> TrackLikes { get; set; }
}
