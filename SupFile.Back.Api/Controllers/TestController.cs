using Microsoft.AspNetCore.StaticFiles;
using SupFile.Back.Storage.Interfaces;

namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public class TestController : BaseAuthController
{
    private readonly IStorageProvider _storageProvider;

    public TestController(ILogger<TestController> logger,
        IStorageProvider storageProvider,
        IUserRepository userRepository,
        IWebHostEnvironment env) : base(logger, userRepository, env)
    {
        _storageProvider = storageProvider;
    }

    [HttpGet("hello")]
    [AllowAnonymous]
    public ActionResult<TestResult> GetHello()
    {
        TestResult result = new()
        {
            content = "Hello World from the server !"
        };

        return Ok(result);
    }
    
    [HttpGet("hello/authorized")]
    public ActionResult<TestResult> GetAuthorizedHello()
    {
        TestResult result = new()
        {
            content = "Hello World from the server !"
        };

        return Ok(result);
    }
    
    [HttpPost("files/upload")]
    [AllowAnonymous]
    public async Task<ActionResult> UploadPicture(IFormFile file, [FromQuery] bool forceRewrite = false)
    {
        //string name, string extension, byte[] content, bool forceRewrite = false, string baseUrl = ""
        var name = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        var writeResult = await _storageProvider.WriteAsync(name, extension, bytes, forceRewrite);
        return ToCreatedAtActionResult(writeResult, nameof(DownloadPicture), new { name, extension });
    }


    [HttpGet("files/download")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadPicture([FromQuery] string name, [FromQuery] string extension)
    {
        var provider = new FileExtensionContentTypeProvider();

        var fileResult = await _storageProvider.ReadAsync(name, extension);
        if (fileResult.IsFailed)
        {
            return ToErrorActionResult(fileResult.ToResult());
        }

        if (!provider.TryGetContentType($"{name}.{extension}", out string contentType))
        {
            contentType = "application/octet-stream";
        }

        return File(fileResult.Value, contentType, name);
    }
}

public class TestResult
{
    public string content  { get; set; }
}