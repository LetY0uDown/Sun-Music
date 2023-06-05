using Host.Interfaces;
using Host.Services.Database;
using Microsoft.EntityFrameworkCore;
using Models.Database;

namespace Host.Services.Repositories;

public class UserRepository : IAsyncRepository<User>
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<User> AddAsync(User entity)
    {
        var entry = await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task DeleteAsync(User entity)
    {
        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> FindAsync(object id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task UpdateAsync(User entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}