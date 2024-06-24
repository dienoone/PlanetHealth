using Microsoft.AspNetCore.SignalR;
using PlantHealth.Api.Constants;

namespace PlantHealth.Api.Hubs;

/// <summary>
/// this is just for testing flutter team;
/// </summary>
public class Detection : Hub 
{
    private ILogger<Detection> _logger;
    private readonly IWebHostEnvironment _env;

    public Detection(
        ILogger<Detection> logger,
        IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public override async Task OnConnectedAsync()
    {
        
        var queryParams = Context.GetHttpContext()!.Request.Query;

        // Access specific query parameters
        if (queryParams.ContainsKey("type"))
        {
            var paramValue = queryParams["type"].ToString();
            _logger.LogWarning($"param value ${paramValue}");
            // Use paramValue as needed

            switch(paramValue)
            {
                case "raspberrypi":
                    await Groups.AddToGroupAsync(Context.ConnectionId, GroupName.RASPBERRYPI);
                    break;
                case "flutter":
                    await Groups.AddToGroupAsync(Context.ConnectionId, GroupName.FLUTTER);
                    break;
            }
        }

        await base.OnConnectedAsync();
        _logger.LogWarning($"A user is connection with a connection id ${Context.ConnectionId}");
    }

    /// <summary>
    /// This is only of test perpose with fullter, 
    /// TODO: Reomve this function!!
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    /// <summary>
    /// this function will be called by flutter, then the function
    /// will send to raspberrypi to capture the image.
    /// 
    /// Then the raspberrypi will call UploadFile 
    /// </summary>
    /// <returns></returns>
    public async Task GetImage()
    {
        Console.WriteLine("hit");

        // Construct the full path to the image file
        string imagePath = Path.Combine(_env.ContentRootPath, "Hubs", "leave.jpg");
        Console.WriteLine(imagePath);
        _logger.LogWarning(imagePath);
        // Check if the file exists
        if (File.Exists(imagePath))
        {
            _logger.LogWarning("file exists already...");
            // Read the image as bytes
            byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

            // Send the bytes to the clients in the group "RASPBERRYPI"
            await Clients.Group(GroupName.FLUTTER).SendAsync("ReceiveImage", imageBytes);
        }
        else
        {
            // Handle the case where the file does not exist
            await Clients.Group(GroupName.FLUTTER).SendAsync("ReceiveImage", null);
        }
    }


    // public async Task UploadFile(IFormFile file)
    // {
    //     await Clients.All.SendAsync("FormFile", "ras");
    
    // }


    // update image will be class by the rasperbyi:
    // public async Task UpdateFile(string fileName, byte[] fileData)
    // {
    //     Console.WriteLine(fileName);
    //     Console.WriteLine(fileData);
    //     await Clients.All.SendAsync("FileUploaded", "ras");
    // }


    
}