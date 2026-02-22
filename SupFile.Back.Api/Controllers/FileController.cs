using SupFile.Back.Storage.Interfaces;

namespace SupFile.Back.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IStorageProvider _storageProvider;

    public FileController(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> UploadPicture(IFormFile file)
    {
        //string name, string extension, byte[] content, bool forceRewrite = false, string baseUrl = ""
        var name = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        try
        {
            await _storageProvider.WriteAsync(name, extension, bytes);
        }
        catch
        {
            return BadRequest();
        }

        return Ok();
    }
}
