using Host.Interfaces;
using Host.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]/")]
public class TracksController : ControllerBase
{
    private readonly IHubContext<MainHub> _hub;
    private readonly IPathHelper _pathHelper;
    private readonly IMusicTrackService _trackService;

    public TracksController(IConfiguration config,
        IHubContext<MainHub> hub, IPathHelper pathHelper,
        IMusicTrackService trackService)
    {
        _hub = hub;
        _pathHelper = pathHelper;
        _trackService = trackService;
    }

    [HttpGet("File/{id:guid}")]
    public async Task<IActionResult> DownloadFile([FromRoute] Guid id)
    {
        var track = await _trackService.Repository.FindAsync(id);

        if (track is null)
            return NotFound();

        var path = _pathHelper.GetTrackPath(track);

        if (!System.IO.File.Exists(path))
            return NotFound();

        return _trackService.GetStream(path);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MusicTrack>>> GetTracks()
    {
        IEnumerable<MusicTrack> tracks;

        try {
            tracks = await _trackService.Repository.GetAsync();
        }
        catch {
            return NotFound();
        }

        return Ok(tracks);
    }

    [HttpPost("Upload"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<Guid>> PostTrack([FromBody] MusicTrack track)
    {
        var isTrackExist = (await _trackService.Repository.GetAsync()).Any(t => t.Title == track.Title
                                                                                && t.ArtistName == track.ArtistName);

        if (isTrackExist) {
            return BadRequest();
        }

        track.ID = Guid.NewGuid();

        try{
            await _trackService.Repository.AddAsync(track);
        } catch {
            // TODO: catch exception
        }

        return track.ID;
    }

    [HttpPost("Upload/File/{id:guid}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<bool>> PostMusicFile([FromRoute] Guid id, [FromForm] IFormFile trackFile)
    {
        var path = _pathHelper.GetMusicFolder();
        var filePath = Path.Combine(path, trackFile.FileName);

        try {
            var track = await _trackService.Repository.FindAsync(id);

            if (track is null)
                return NotFound();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (System.IO.File.Exists(filePath))
                return NoContent();

            track.FileName = trackFile.FileName;
            await _trackService.Repository.UpdateAsync(track);

            await _hub.Clients.Group("Tracks").SendAsync("RecieveTrack", track);

            await _trackService.SaveFile(trackFile, filePath);
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }
}