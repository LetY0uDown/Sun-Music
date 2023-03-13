namespace Models.Database;

public partial class User : Entity
{
    public User ()
    {
        Chatmembers = new HashSet<ChatMember>();
        Messages = new HashSet<Message>();
        Playlists = new HashSet<Playlist>();
        TrackLikes = new HashSet<TrackLike>();
    }

    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public byte[] ImageBytes { get; set; } = null!;

    public virtual ICollection<ChatMember> Chatmembers { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
    public virtual ICollection<Playlist> Playlists { get; set; }
    public virtual ICollection<TrackLike> TrackLikes { get; set; }
}