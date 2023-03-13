namespace Models.Database;

public partial class PlaylistTrack : Entity
{
    public int TrackOrder { get; set; }
    public string PlaylistID { get; set; } = null!;
    public string TrackID { get; set; } = null!;

    public virtual Playlist Playlist { get; set; } = null!;
    public virtual MusicTrack Track { get; set; } = null!;
}