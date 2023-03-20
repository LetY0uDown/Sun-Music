using Host.Interfaces;

namespace Host.Services;

public class IDGenerator : IIDGenerator
{
    public string GenerateID ()
    {
        return Guid.NewGuid().ToString();
    }
}