using Models.Database;

namespace Host.Interfaces;

public interface IPathHelper
{
    string GetTrackPath(MusicTrack track);

    string GetMusicFolder();
}