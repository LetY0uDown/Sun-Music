using Microsoft.AspNetCore.Mvc;

namespace Host.Services;

public interface IMusicTrackService
{
    Task SaveFile(IFormFile trackForm, string path);

    FileStreamResult GetStream(string path);
}