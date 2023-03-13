using Host.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Database;

namespace Host.Controllers;

[Route("Test")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly DatabaseContext database;
    
    public TestController(DatabaseContext database)
    {
        this.database = database;
    }

    [HttpGet]
    public async Task<IEnumerable<User>> GetUsers()
    {
        return await database.Users.ToListAsync();
    }

    [HttpGet("/{id}")]
    public async Task<User> GetUserByID(string id)
    {
        return await database.Users.FindAsync(id);
    }
}