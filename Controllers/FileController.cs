using Microsoft.AspNetCore.Mvc;

namespace PlantHealth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    [HttpPost]
    public IActionResult UpdateFile(IFormFile file)
    {
        var data = file.FileName;
        return Ok("Hello from the server!!");
    }
}