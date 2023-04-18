using Host.Interfaces;
using Host.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Database;
using Models.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Host.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IPasswordEncoder _passEncoder;
    private readonly IIDGenerator _hashGen;
    private readonly IAuthTokenGen _authTokenGen;
    private readonly IHubContext<MainHub> _hub;
    private readonly ILogger<AuthController> _logger;
    private readonly DatabaseContext _db;

    public AuthController(DatabaseContext db, IIDGenerator hashGen, IPasswordEncoder passEncoder,
                          IAuthTokenGen authTokenGen, IHubContext<MainHub> hub, ILogger<AuthController> logger)
    {
        _db = db;
        _hashGen = hashGen;
        _passEncoder = passEncoder;
        _authTokenGen = authTokenGen;
        _hub = hub;
        _logger = logger;
    }

    [HttpPost("/Register")]
    public async Task<ActionResult<AuthorizeData>> RegisterUser([FromBody] User user)
    {
        try {
            var isNameUnique = _db.Users.Any(u => u.Username == user.Username);

            if (isNameUnique) {
                return Problem("Пользователь с таким именем уже существует, попробуйте другое");
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
        catch (Exception e) {
            _logger.Log(LogLevel.Warning, e, "Registration process went wrong");
            return Problem("Что-то пошло не так. Попробуйте ещё раз или обратитесь к администратору", e.Message);
        }
    }

    [HttpPost("/Login")]
    public async Task<ActionResult<AuthorizeData>> AuthorizeUser([FromBody] User user)
    {
        user.Password = _passEncoder.Encode(user.Password);
        User? userInDB = null!;

        try {
            userInDB = await _db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        }
        catch (Exception e)  {
            _logger.Log(LogLevel.Warning, e, "Login process went wrong");
            return Problem("Что-то пошло не так. Попробуйте ещё раз или обратитесь к администратору", e.Message);
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