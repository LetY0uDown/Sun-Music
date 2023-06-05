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
public class LikesController : ControllerBase
{
    private readonly DatabaseContext _database;
    private readonly IHubContext<MainHub> _hub;

    public LikesController (DatabaseContext database, IHubContext<MainHub> hub)
    {
        _database = database;
        _hub = hub;
    }

    [HttpPost("Dislike/{trackID:guid}/{userID:guid}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DislikeTrack ([FromRoute] Guid trackID, [FromRoute] Guid userID)
    {
        try {
            var trackLike = await _database.TrackLikes.Include(e => e.Track)
                                                      .FirstOrDefaultAsync(like => like.UserID == userID &&
                                                                                   like.TrackID == trackID);
            _database.TrackLikes.Remove(trackLike);

            await _database.SaveChangesAsync();

            await _hub.Clients.Group($"Likes-{userID}").SendAsync("TrackDisliked", trackLike.Track);

            return NoContent();
        }
        catch (Exception e) {
            return Problem();
        }
    }

    [HttpPost("Like/{trackID:guid}/{userID:guid}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> LikeTrack ([FromRoute] Guid trackID, [FromRoute] Guid userID)
    {
        try {
            TrackLike trackLike = new() {
                ID = Guid.NewGuid(),
                UserID = userID,
                TrackID = trackID
            };

            _database.TrackLikes.Add(trackLike);
            await _database.SaveChangesAsync();

            await _hub.Clients.Group($"Likes-{userID}").SendAsync("TrackLiked", await _database.MusicTracks.FindAsync(trackID));

            return NoContent();
        }
        catch (Exception e) {
            return Problem();
        }
    }

    [HttpGet("User/{id:guid}")]
    public async Task<ActionResult<IEnumerable<MusicTrack>>> GetFavoriteTracksByUser (Guid id)
    {
        try {
            var ids = await _database.TrackLikes.Where(e => e.UserID == id)
                                                .Select(e => e.TrackID)
                                                .ToListAsync();

            if (ids is null) {
                return NotFound();
            }

            var tracks = await _database.MusicTracks.Where(track => ids.Contains(track.ID))
                                                    .ToListAsync();

            if (tracks is null) {
                return NotFound();
            }

            return Ok(tracks);

        }
        catch (Exception e) {
            return Problem();
        }
    }
}