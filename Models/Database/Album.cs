namespace Models.Database;

public partial class Album : Entity
{
    public Album ()
    {
        AlbumTracks = new HashSet<AlbumTrack>();
    }

    public string Title { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public byte[] ImageBytes { get; set; } = null!;
    public string ArtistId { get; set; } = null!;

    public virtual Artist Artist { get; set; } = null!;
    public virtual ICollection<AlbumTrack> AlbumTracks { get; set; }
}