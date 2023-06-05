using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Database;

public class Playlist : Entity
{
    public string Title { get; set; } = null!;
    public byte[]? ImageBytes { get; set; }
    public Guid UserID { get; set; }

    public User User { get; set; } = null!;

    [NotMapped]
    public IEnumerable<MusicTrack>? MusicTracks { get; set; } = null;
}