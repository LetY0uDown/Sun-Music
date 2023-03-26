using Host.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Client;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _db;

    public UsersController (DatabaseContext db)
    {
        _db = db;
    }

    [HttpGet("/u/{id}")]
    public async Task<ActionResult<User>> GetUserByID ([FromRoute] string id)
    {
        try {
            var user = await _db.Users.FindAsync(id);

            if (user is null)
                return NotFound();

            return user;
        }
        catch {
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PublicUser>>> GetUserList ()
    {
        try {
            var users = _db.Users.ToList();

            List<PublicUser> publicUsers = new List<PublicUser>();

            foreach (var u in users) {
                publicUsers.Add(u);
            }

            return Ok(publicUsers);
        }
        catch {
            return BadRequest();
        }
    }
}