using HashidsNet;
using Host.Interfaces;

namespace Host.Services;

public class HashIDGenerator : IHashIDGenerator
{
    private readonly Hashids _hashIDs;

    public HashIDGenerator (IConfiguration configuration)
    {
        _hashIDs = new Hashids(configuration["HashPepper"], minHashLength: 8);
    }

    //TODO: ... fix?
    public string GenerateHash ()
    {
        return _hashIDs.Encode(new Random().Next(1, 10001));
    }
}