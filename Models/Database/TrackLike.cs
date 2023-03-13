namespace Models.Database;

public partial class TrackLike : Entity
{
    public string UserID { get; set; } = null!;
    public string TrackID { get; set; } = null!;

    public virtual MusicTrack Track { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}