using Host.Interfaces;
using Host.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Database;

namespace Host.Controllers;

[ApiController, Route("[controller]")]
public class MusicController : ControllerBase
{
    private readonly DatabaseContext _database;
    private readonly IIDGenerator _idGen;

    public MusicController(DatabaseContext database, IIDGenerator idGen)
    {
        _database = database;
        _idGen = idGen;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MusicTrack>>> GetTracks()
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

    [HttpPost]
    public async Task<ActionResult<string>> PostTrack([FromBody] MusicTrack track)
    {
        var isTrackExist = _database.MusicTracks.Any(t => t.Title == track.Title
                                                          && t.ArtistName == track.ArtistName);

        if (isTrackExist) {
            return BadRequest();
        }

        track.ID = _idGen.GenerateID();

        try {
            _database.MusicTracks.Add(track);
            await _database.SaveChangesAsync();
        }
        catch {
            // TODO: catch exception
        }

        return track.ID;
    }

    [HttpPost("/Music/File/{id}")]
    public async Task<ActionResult<bool>> PostFile(IFormFile file, [FromRoute] string id)
    {
        var path = Path.Combine(Environment.CurrentDirectory, "Music/");

        if (System.IO.File.Exists(path + file.FileName)) {
            return BadRequest();
        }

        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        try {
            using (FileStream fs = new(path, FileMode.Create)) {
                await file.CopyToAsync(fs);
            }
        }
        catch {
            return false;
        }

        return true;
    }
}