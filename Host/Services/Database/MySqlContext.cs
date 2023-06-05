using Microsoft.EntityFrameworkCore;

namespace Host.Services.Database;

public class MySqlContext : DatabaseContext
{
    private readonly IConfiguration _configuration;

    public MySqlContext(IConfiguration configuration)
    {
        _configuration = configuration;

        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(_configuration["ConnectionStrings:Home:MySql"], ServerVersion.AutoDetect(_configuration["ConnectionStrings:Home:MySql"]));
        }
    }
}