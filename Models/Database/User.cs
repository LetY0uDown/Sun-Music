using System.Text.Json.Serialization;

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

    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? ProfileDescription { get; set; }

    public byte[]? ImageBytes { get; set; }

    [JsonIgnore]
    public virtual ICollection<ChatMember> Chatmembers { get; set; }
    [JsonIgnore]
    public virtual ICollection<Message> Messages { get; set; }
    [JsonIgnore]
    public virtual ICollection<Playlist> Playlists { get; set; }
    [JsonIgnore]
    public virtual ICollection<TrackLike> TrackLikes { get; set; }

    public static User Empty { get; } = new User {
        ID = Guid.Empty.ToString(),
        ImageBytes = Array.Empty<byte>(),
        Username = string.Empty,
        Password = string.Empty
    };
}