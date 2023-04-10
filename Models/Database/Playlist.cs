using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

    [JsonIgnore]
    public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }

    [NotMapped]
    public IEnumerable<MusicTrack>? MusicTracks { get; set; } = null;
}
