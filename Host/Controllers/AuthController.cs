using Host.Interfaces;
using Host.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Database;
using Models.Client;

namespace Host.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IPasswordEncoder _passEncoder;
    private readonly IHashIDGenerator _hashGen;
    private readonly IAuthTokenGen _authTokenGen;
    private readonly ILogger _logger;
    private readonly DatabaseContext _db;

    public AuthController(DatabaseContext db, IHashIDGenerator hashGen, IPasswordEncoder passEncoder, IAuthTokenGen authTokenGen)
    {
        _db = db;
        _hashGen = hashGen;
        _passEncoder = passEncoder;
        _authTokenGen = authTokenGen;
    }

    [HttpPost("/Register")]
    public async Task<ActionResult<AuthorizeData>> RegisterUser([FromBody] User user)
    {
        try {
            var isNameUnique = _db.Users.Any(u => u.Username == user.Username);

            if (isNameUnique) {
                Forbid("User with this username already exist");
            }

            user.Password = _passEncoder.Encode(user.Password);

            user.ID = _hashGen.GenerateHash();

            var entity = await _db.Users.AddAsync(user);

            return new AuthorizeData (
                user.ID,
                _authTokenGen.GetToken(user.ID, user.Password)
            );
        }
        catch {
            return null!; // Handle error :D
        }
    }

    [HttpPost("/Login")]
    public async Task<ActionResult<AuthorizeData>> AuthorizeUser([FromBody] User user)
    {
        user.Password = _passEncoder.Encode(user.Password);
        User? userInDB = null!;

        try
        {
            userInDB = _db.Users.FirstOrDefault(u => u.Username == user.Username);
        }
        catch
        {
            return null!; // Handle error :D
        }

        if (userInDB is null) {
            return NotFound("Wrong username");
        }

        if (userInDB.Password != user.Password) {
            return Forbid("Wrong password");
        }

        return new AuthorizeData (
            userInDB.ID, 
            _authTokenGen.GetToken(userInDB.ID, user.Password)
            ); 
    }
}