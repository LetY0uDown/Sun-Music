namespace Models.Database;

public class MusicTrack : Entity
{
    public string Title { get; set; } = null!;
    public string ArtistName { get; set; } = null!;
    public string AlbumName { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public byte[] ImageBytes { get; set; } = null!;
    public string FileName { get; set; } = null!;
}