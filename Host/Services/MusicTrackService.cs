using Microsoft.AspNetCore.Mvc;

namespace Host.Services;

public class MusicTrackService : IMusicTrackService
{
    public FileStreamResult GetStream(string path)
    {
        var stream = new FileStream(path, FileMode.Open);

        return new FileStreamResult(stream, "application/octet-stream");
    }

    public async Task SaveFile(IFormFile trackForm, string path)
    {
        using FileStream fs = new(path, FileMode.Create);
        await trackForm.CopyToAsync(fs);
    }
}