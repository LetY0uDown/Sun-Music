using Host.Interfaces;
using Models.Database;

namespace Host.Services;

public class PathHelper : IPathHelper
{
    private readonly IConfiguration _config;

    public PathHelper(IConfiguration config)
    {
        _config = config;
    }

    public string GetMusicFolder()
    {
        return Path.Combine(Environment.CurrentDirectory, _config["Directories:Music"]);
    }

    public string GetTrackPath(MusicTrack track)
    {
        return Path.Combine(GetMusicFolder(), track.FileName);
    }
}