namespace Models.Database;

public partial class AlbumTrack : Entity
{
    public int TrackOrder { get; set; }
    public string AlbumID { get; set; } = null!;
    public string TrackID { get; set; } = null!;

    public virtual Album Album { get; set; } = null!;
    public virtual MusicTrack Track { get; set; } = null!;
}