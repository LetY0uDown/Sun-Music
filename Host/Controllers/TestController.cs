using Host.Services;
using Microsoft.AspNetCore.Mvc;

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
}