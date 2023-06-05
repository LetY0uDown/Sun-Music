using Microsoft.EntityFrameworkCore;

namespace Host.Services.Database;

public class SqlServerContext : DatabaseContext
{
    private readonly IConfiguration _config;

    public SqlServerContext(IConfiguration config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_config["ConnectionStrings:Colledge:SqlServer"]);
        }
    }
}