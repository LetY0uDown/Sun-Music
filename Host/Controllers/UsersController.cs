using Host.Services.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Client;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]/")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _db;
    private readonly ILogger<UsersController> _logger;

    public UsersController (DatabaseContext db, ILogger<UsersController> logger)
    {
        _db = db;
        _logger = logger;
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
        catch (Exception e) {
            _logger.LogWarning(e.Message);
            return Problem(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PublicUser>>> GetUserList ()
    {
        try {
            var users = await _db.Users.Where(user => user.ID != Guid.Empty).ToListAsync();

            List<PublicUser> publicUsers = new();

            foreach (var u in users) {
                publicUsers.Add(u);
            }

            return Ok(publicUsers);
        }
        catch (Exception e) {
            _logger.LogWarning(e.Message);
            return Problem(e.Message);
        }
    }

    [HttpPut("Edit/{id}")]
    public async Task<ActionResult> EditUser ([FromRoute] string id, [FromBody] User user)
    {
        try {
            _db.Entry(user).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return NoContent();
        } catch (Exception e) { 
            _logger.LogWarning(e.Message);
            return Problem(e.Message);
        }
    }
}