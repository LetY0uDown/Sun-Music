using Desktop_Client.Core.Tools.Attributes;
using Models.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

[BaseType]
public interface IMusicPlayer : IService
{
    Task Initialize ();

    Task SetTrack(MusicTrack track);

    void SetVolume(double volume);

    void Pause();

    void ContinuePlaying();

    void PlayPrevious();

    void PlayNext();

    void SetPlaylist(IEnumerable<MusicTrack> playlist);

    void RepeatCurrentTrack(bool repeat);

    void ShufflePlaylist();

    // TODO: Think more about this method's name (it returns normal order to shuffeled playlist)
    void OrderPlaylist();
}