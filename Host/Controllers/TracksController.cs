using Host.Interfaces;
using Host.Services;
using Host.Services.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]/")]
public class TracksController : ControllerBase
{
    private readonly DatabaseContext _database;
    private readonly IIDGenerator _idGen;
    private readonly IConfiguration _config;
    private readonly IHubContext<MainHub> _hub;
    private readonly IPathHelper _pathHelper;
    private readonly IMusicTrackService _trackService;

    public TracksController (DatabaseContext database, IIDGenerator idGen, IConfiguration config,
        IHubContext<MainHub> hub, IPathHelper pathHelper,
        IMusicTrackService trackService)
    {
        _database = database;
        _idGen = idGen;
        _config = config;
        _hub = hub;
        _pathHelper = pathHelper;
        _trackService = trackService;
    }

    [HttpGet("File/{id}")]
    public IActionResult DownloadFile ([FromRoute] string id)
    {
        var track = _database.MusicTracks.Find(id);

        if (track is null)
            return NotFound();

        var path = _pathHelper.GetTrackPath(track);

        if (!System.IO.File.Exists(path))
            return NotFound();

        return _trackService.GetStream(path);
    }

    [HttpGet]
    public ActionResult<IEnumerable<MusicTrack>> GetTracks ()
    {
        IEnumerable<MusicTrack> tracks;

        try {
            tracks = _database.MusicTracks;
        }
        catch {
            return NotFound();
        }

        return Ok(tracks);
    }

    [HttpPost("Upload"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<string>> PostTrack ([FromBody] MusicTrack track)
    {
        var isTrackExist = _database.MusicTracks.Any(t => t.Title == track.Title
                                                          && t.ArtistName == track.ArtistName);

        if (isTrackExist) {
            return BadRequest();
        }

        track.ID = _idGen.GenerateID();

        try {
            await _database.MusicTracks.AddAsync(track);
            await _database.SaveChangesAsync();
        }
        catch {
            // TODO: catch exception
        }

        return track.ID;
    }

    [HttpPost("Upload/File/{id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<bool>> PostMusicFile ([FromRoute] string id, [FromForm] IFormFile trackFile)
    {
        var path = _pathHelper.GetMusicFolder();
        var filePath = Path.Combine(path, trackFile.FileName);

        try {
            var track = _database.MusicTracks.Find(id);

            if (track is null)
                return NotFound();

            if (!System.IO.File.Exists(filePath) && !Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
            }

            track.FileName = trackFile.FileName;
            await _database.SaveChangesAsync();

            await _hub.Clients.Group("Tracks").SendAsync("RecieveTrack", track);

            await _trackService.SaveFile(trackFile, filePath);
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }
}