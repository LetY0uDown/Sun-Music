using Models.Database;

namespace Models.Client;

public record class PublicUser
{
    public Guid ID { get; init; }

    public string Username { get; init; }

    public byte[]? ImageBytes { get; init; }

    public static implicit operator PublicUser(User user)
    {
        return new PublicUser
        {
            ID = user.ID,
            Username = user.Username,
            ImageBytes = user.ImageBytes
        };
    }
}