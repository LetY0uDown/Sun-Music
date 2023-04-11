namespace Models.Database;

public partial class Message : Entity
{
    public string SenderID { get; set; } = null!;
    public string ChatID { get; set; } = null!;
    public string Text { get; set; } = null!;

    public DateTime TimeSended { get; set; }

    public virtual Chat Chat { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
}
