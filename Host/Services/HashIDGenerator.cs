using HashidsNet;
using Host.Interfaces;

namespace Host.Services;

public class HashIDGenerator : IHashIDGenerator
{
    private readonly Hashids _hashIDs;

    public HashIDGenerator(IConfiguration configuration)
    {
        _hashIDs = new Hashids(configuration["HashPepper"], 8);
    }

    public string GenerateHash()
    {
        return _hashIDs.Encode(new Random().Next(1, 10001));
    }
}