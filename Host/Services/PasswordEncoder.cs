using Host.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace Host.Services;

public class PasswordEncoder : IPasswordEncoder
{
    private readonly IConfiguration _configuration;

    public PasswordEncoder(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Encode(string pass)
    {
        var saltBytes = Encoding.UTF8.GetBytes(_configuration["PasswordSalt"]);

        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: pass,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 16));

        return hash;
    }
}