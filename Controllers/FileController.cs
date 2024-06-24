using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlantHealth.Api.Hubs;
using PlantHealth.Api.Constants;

namespace PlantHealth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IHubContext<Detection> _detectionHubContext;

    public FileController(IHubContext<Detection> detectionHubContext)
    {
       _detectionHubContext = detectionHubContext; 
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty or not provided.");
        }

        // Read the file content into a byte array
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            // Send the file bytes to SignalR clients
            await _detectionHubContext.Clients.Group(GroupName.FLUTTER).SendAsync("ReceiveImage", fileBytes, file.FileName);
        }
        
        return Ok("Hello from the server!!");
    }
}