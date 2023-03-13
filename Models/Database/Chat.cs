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

    public virtual ICollection<ChatMember> Chatmembers { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
}
