using Host.Interfaces;
using Host.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Database;
using Models.Client;
using Microsoft.AspNetCore.SignalR;

namespace Host.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IPasswordEncoder _passEncoder;
    private readonly IIDGenerator _hashGen;
    private readonly IAuthTokenGen _authTokenGen;
    private readonly IHubContext<MainHub> _hub;
    private readonly ILogger _logger;
    private readonly DatabaseContext _db;

    public AuthController(DatabaseContext db, IIDGenerator hashGen, IPasswordEncoder passEncoder, IAuthTokenGen authTokenGen, IHubContext<MainHub> hub)
    {
        _db = db;
        _hashGen = hashGen;
        _passEncoder = passEncoder;
        _authTokenGen = authTokenGen;
        _hub = hub;
    }

    [HttpPost("/Register")]
    public async Task<ActionResult<AuthorizeData>> RegisterUser([FromBody] User user)
    {
        try {
            var isNameUnique = _db.Users.Any(u => u.Username == user.Username);

            if (isNameUnique) {
                return Forbid("Bearer");
            }

            user.Password = _passEncoder.Encode(user.Password);

            user.ID = _hashGen.GenerateID();

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            await _hub.Clients.Group("Users").SendAsync("RecieveUser", (PublicUser)user);

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
            return Forbid("Bearer");
        }

        return new AuthorizeData (
            userInDB.ID, 
            _authTokenGen.GetToken(userInDB.ID, user.Password)
            ); 
    }
}