using Models.Database;
using System.Collections.Generic;

namespace Desktop_Client.Core.Abstracts;

public interface IMusicPlayer : IService
{
    void SetTrack(MusicTrack track);

    void SetVolume(int volume);

    void Pause();

    void ContinuePlaying();

    void PlayPrevious();

    void PlayNext();

    void SetPlaylist(LinkedList<MusicTrack> playlist);

    void RepeatCurrentTrack(bool repeat);

    void ShufflePlaylist();

    // TODO: Think more about this method's name (it returns normal order to shuffeled playlist)
    void OrderPlaylist();

    void DownloadCurrentTrack();
}