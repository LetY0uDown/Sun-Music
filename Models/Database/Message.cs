namespace Models.Database;

public class Message : Entity
{
    public Guid SenderID { get; set; }
    public Guid ChatID { get; set; }
    public string Text { get; set; } = null!;

    public DateTime TimeSended { get; set; }

    public Chat Chat { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
