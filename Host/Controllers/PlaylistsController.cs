using Host.Interfaces;
using Host.Services;
using Host.Services.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]/")]
public class PlaylistsController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IHubContext<MainHub> _hub;

    public PlaylistsController (DatabaseContext context, IHubContext<MainHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    [HttpGet("Tracks/{id:guid}")]
    public async Task<ActionResult<IEnumerable<MusicTrack>>> GetTracksByPlaylist ([FromRoute] Guid id)
    {
        try { // fix asap
            //var tracks = await _context.PlaylistTracks.Include(e => e.Track)
            //                                          .Where(e => e.PlaylistID == id)
            //                                          .Select(e => e.Track)
            //                                          .ToListAsync();

            //return Ok(tracks);
            return null;
        }
        catch (Exception e) {
            return Problem();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Playlist>> GetPlaylistByID ([FromRoute] string id)
    {
        try {
            return await _context.Playlists.FindAsync(id);
        }
        catch (Exception e) {
            return Problem();
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists ()
    {
        try {
            return await _context.Playlists.Include(u => u.User).ToListAsync();
        }
        catch (Exception ex) {
            return Problem();
        }
    }

    [HttpPost("Create"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<Playlist>> CreatePlaylist ([FromBody] Playlist playlist)
    {
        try {
            playlist.ID = Guid.NewGuid();

            var user = await _context.Users.FindAsync(playlist.UserID);

            if (user is null) {
                return NotFound();
            }

            playlist.User = user;

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            if (playlist.MusicTracks is not null) {
                foreach (var track in playlist.MusicTracks) {
                    PlaylistTrack playlistTrack = new() {
                        ID = Guid.NewGuid(),
                        TrackID = track.ID,
                        PlaylistID = playlist.ID
                    };

                    _context.PlaylistTracks.Add(playlistTrack);
                }

                await _context.SaveChangesAsync();
            }

            await _hub.Clients.Group("Playlists").SendAsync("RecievePlaylist", playlist);
        }
        catch (Exception ex) {
            return Problem();
        }

        return playlist;
    }
}