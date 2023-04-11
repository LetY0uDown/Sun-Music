using Host.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Client;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]/")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _db;

    public UsersController (DatabaseContext db)
    {
        _db = db;
    }

    [HttpGet("{id}")]
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
            var users = await _db.Users.Where(user => user.ID != Guid.Empty.ToString()).ToListAsync();

            List<PublicUser> publicUsers = new();

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