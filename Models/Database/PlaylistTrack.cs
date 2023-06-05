namespace Models.Database;

public class PlaylistTrack : Entity
{
    public Guid PlaylistID { get; set; }
    public Guid TrackID { get; set; }
}