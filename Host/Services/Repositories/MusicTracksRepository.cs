using Host.Interfaces;
using Host.Services.Database;
using Microsoft.EntityFrameworkCore;
using Models.Database;

namespace Host.Services.Repositories;

public class MusicTracksRepository : IAsyncRepository<MusicTrack>
{
    private readonly DatabaseContext _context;

    public MusicTracksRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<MusicTrack> AddAsync(MusicTrack entity)
    {
        var entry = await _context.MusicTracks.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task DeleteAsync(MusicTrack entity)
    {
        _context.MusicTracks.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<MusicTrack>> GetAsync()
    {
        return await _context.MusicTracks.ToListAsync();
    }

    public async Task<MusicTrack?> FindAsync(object id)
    {
        return await _context.MusicTracks.FindAsync(id);
    }

    public async Task UpdateAsync(MusicTrack entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}