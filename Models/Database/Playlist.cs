namespace Models.Database;

public partial class Playlist : Entity
{
    public Playlist ()
    {
        PlaylistTracks = new HashSet<PlaylistTrack>();
    }

    public string Title { get; set; } = null!;
    public byte[]? ImageBytes { get; set; }
    public string UserID { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }
}
