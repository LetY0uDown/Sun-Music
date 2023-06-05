namespace Models.Database;

public class TrackLike : Entity
{
    public Guid UserID { get; set; }
    public Guid TrackID { get; set; }

    public MusicTrack Track { get; set; } = null!;
    public User User { get; set; } = null!;
}