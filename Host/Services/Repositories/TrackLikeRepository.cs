using Host.Interfaces;
using Host.Services.Database;
using Microsoft.EntityFrameworkCore;
using Models.Database;

namespace Host.Services.Repositories;

public class TrackLikeRepository : IAsyncRepository<TrackLike>
{
    private readonly DatabaseContext _context;

    public TrackLikeRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<TrackLike> AddAsync(TrackLike entity)
    {
        var entry = await _context.TrackLikes.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task DeleteAsync(TrackLike entity)
    {
        _context.TrackLikes.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TrackLike>> GetAsync()
    {
        return await _context.TrackLikes.Include(e => e.Track).ToListAsync();
    }

    public async Task<TrackLike?> FindAsync(object id)
    {
        return await _context.TrackLikes.Include(e => e.Track)
                                        .FirstOrDefaultAsync(e => e.ID == (Guid)id);
    }

    public async Task UpdateAsync(TrackLike entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}