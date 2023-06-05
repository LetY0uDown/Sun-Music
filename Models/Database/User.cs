namespace Models.Database;

public class User : Entity
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? ProfileDescription { get; set; }

    public byte[]? ImageBytes { get; set; }

    public static User Empty { get; } = new User
    {
        ID = Guid.Empty,
        ImageBytes = Array.Empty<byte>(),
        Username = string.Empty,
        Password = string.Empty
    };
}