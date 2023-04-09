using Host.Interfaces;
using Host.Services;
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
    private readonly IIDGenerator _idGen;

    public PlaylistsController (DatabaseContext context, IHubContext<MainHub> hub, IIDGenerator idGen)
	{
        _context = context;
        _hub = hub;
        _idGen = idGen;
    }

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists ()
	{
		try {
			return await _context.Playlists.Include(u => u.User).ToListAsync();
		} catch (Exception ex) {
			return Problem();
		}
	}

	[HttpPost("Create"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<ActionResult<Playlist>> CreatePlaylist ([FromBody] Playlist playlist)
	{
		try {
			playlist.ID = _idGen.GenerateID();

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
						ID = _idGen.GenerateID(),
						Track = track,
						TrackID = track.ID,
						Playlist = playlist,
						PlaylistID = playlist.ID
					};

					_context.MusicTracks.Attach(playlistTrack.Track);
					_context.PlaylistTracks.Add(playlistTrack);
                }

                await _context.SaveChangesAsync();
            }
			
			await _hub.Clients.Group("Playlists").SendAsync("RecievePlaylist", playlist);
		} catch (Exception ex) {
			return Problem();
		}

		return playlist;
	}
}