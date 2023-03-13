namespace Models.Database;

public partial class ChatMember : Entity
{
    public string ChatId { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public virtual Chat Chat { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
