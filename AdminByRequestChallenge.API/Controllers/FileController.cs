using Microsoft.AspNetCore.Mvc;

namespace AdminByRequestChallenge.API.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController(IWebHostEnvironment env) : ControllerBase
{
    [HttpGet("DownloadSecretFile")]
    public async Task<IActionResult> DownloadFile()
    {
        const string fileName = "developers.gif";
        var filePath = Path.Combine(env.ContentRootPath, "Assets", fileName);

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

        return File(fileBytes, "image/gif", fileName);
    }
}
