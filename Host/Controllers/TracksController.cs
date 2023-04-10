using Host.Interfaces;
using Host.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]")]
public class TracksController : ControllerBase
{
    private readonly DatabaseContext _database;
    private readonly IIDGenerator _idGen;
    private readonly IConfiguration _config;
    private readonly IHubContext<MainHub> _hub;

    public TracksController(DatabaseContext database, IIDGenerator idGen, IConfiguration config, IHubContext<MainHub> hub)
    {
        _database = database;
        _idGen = idGen;
        _config = config;
        _hub = hub;
    }

    [HttpGet("/Tracks/File/{id}")]
    public IActionResult DownloadFile([FromRoute] string id)
    {
        var track = _database.MusicTracks.Find(id);

        if (track is null)
            return NotFound();

        var path = Path.Combine(Environment.CurrentDirectory, _config["Directories:Music"], track.FileName);

        if (!System.IO.File.Exists(path))
            return NotFound();

        var stream = new FileStream(path, FileMode.Open);

        return File(stream, "application/octet-stream");
    }

    [HttpGet("/Tracks")]
    public ActionResult<IEnumerable<MusicTrack>> GetTracks()
    {
        IEnumerable<MusicTrack> tracks;

        try
        {
            tracks = _database.MusicTracks;
        }
        catch
        {
            return NotFound();
        }

        return Ok(tracks);
    }

    [HttpPost("/Tracks/Upload")]
    public async Task<ActionResult<string>> PostTrack([FromBody] MusicTrack track)
    {
        var isTrackExist = _database.MusicTracks.Any(t => t.Title == track.Title
                                                          && t.ArtistName == track.ArtistName);

        if (isTrackExist)
        {
            return BadRequest();
        }

        track.ID = _idGen.GenerateID();

        try
        {
            await _database.MusicTracks.AddAsync(track);
            await _database.SaveChangesAsync();
        }
        catch
        {
            // TODO: catch exception
        }

        return track.ID;
    }

    [HttpPost("/Tracks/Upload/File/{id}")]
    public async Task<ActionResult<bool>> PostMusicFile([FromRoute] string id, [FromForm] IFormFile file)
    {
        var path = Path.Combine(Environment.CurrentDirectory, _config["Directories:Music"]);
        var filePath = Path.Combine(path, file.FileName);

        try
        {
            var track = _database.MusicTracks.Find(id);

            if (track is null)
                return NotFound();

            if (System.IO.File.Exists(filePath))
                return BadRequest();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            track.FileName = file.FileName;
            await _database.SaveChangesAsync();

            await _hub.Clients.Group("Tracks").SendAsync("RecieveTrack", track);

            using FileStream fs = new(filePath, FileMode.Create);
            await file.CopyToAsync(fs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }
}