using Host.Interfaces;
using Host.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]/")]
public class LikesController : ControllerBase
{
    private readonly DatabaseContext _database;
    private readonly IIDGenerator _idGen;
    private readonly IConfiguration _config;

    public LikesController(DatabaseContext database, IIDGenerator idGen, IConfiguration config)
    {
        _database = database;
        _idGen = idGen;
        _config = config;
    }

    [HttpPost("Dislike/{trackID}/{userID}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DislikeTrack([FromRoute] string trackID, [FromRoute] string userID)
    {
        try
        {
            var trackLike = await _database.TrackLikes.FirstOrDefaultAsync(like => like.UserID == userID && like.TrackID == trackID);
            _database.TrackLikes.Remove(trackLike);

            await _database.SaveChangesAsync(true);

            return NoContent();
        }
        catch (Exception e)
        {
            return Problem();
        }
    }

    [HttpPost("Like/{trackID}/{userID}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> LikeTrack([FromRoute] string trackID, [FromRoute] string userID)
    {
        try
        {
            TrackLike trackLike = new() {
                ID = _idGen.GenerateID(),
                UserID = userID,
                TrackID = trackID
            };

            _database.TrackLikes.Add(trackLike);
            await _database.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception e)
        {
            return Problem();
        }
    }

    [HttpGet("User/{id}")]
    public async Task<ActionResult<IEnumerable<MusicTrack>>> GetFavoriteTracksByUser(string id)
    {
        try {
            var ids = await _database.TrackLikes.Where(e => e.UserID == id)?.Select(e => e.TrackID).ToListAsync();

            if (ids is null || !ids.Any()) {
                return NotFound();
            }

            var tracks = await _database.MusicTracks.Where(track => ids.Contains(track.ID)).ToListAsync();

            if (tracks is null || !tracks.Any()) {
                return NotFound();
            }

            return Ok(tracks);

        } catch (Exception e) {
            return Problem();
        }
    }
}