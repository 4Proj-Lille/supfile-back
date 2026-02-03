namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public class TestController : BaseAuthController
{

    public TestController(ILogger<TestController> logger,
        IUserRepository userRepository,
        IWebHostEnvironment env) : base(logger, userRepository, env)
    {
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
}

public class TestResult
{
    public string content  { get; set; }
}