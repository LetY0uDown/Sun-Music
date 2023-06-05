using Microsoft.AspNetCore.Mvc;
using Models.Database;

namespace Host.Interfaces;

public interface IMusicTrackService
{
    IAsyncRepository<MusicTrack> Repository { get; }

    Task SaveFile(IFormFile trackForm, string path);

    FileStreamResult GetStream(string path);
}