using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Models.Database;

public partial class Chat : Entity
{
    public Chat ()
    {
        Chatmembers = new HashSet<ChatMember>();
        Messages = new HashSet<Message>();
    }

    public string Title { get; set; } = null!;
    public byte[] ImageBytes { get; set; } = null!;
    public string CreatorID { get; set; } = null!;

    [AllowNull]
    public User Creator { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<ChatMember> Chatmembers { get; set; }
    [JsonIgnore]
    public virtual ICollection<Message> Messages { get; set; }
}
