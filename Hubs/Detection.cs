using Microsoft.AspNetCore.SignalR;
using PlantHealth.Api.Constants;

namespace PlantHealth.Api.Hubs;

public class Detection : Hub 
{
    private ILogger<Detection> _logger;

    public Detection(ILogger<Detection> logger)
    {
        _logger = logger;
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

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }


    // update image will be class by the rasperbyi:
    public async Task UpdateFile(string fileName, byte[] fileData)
    {
        Console.WriteLine(fileName);
        Console.WriteLine(fileData);
        await Clients.All.SendAsync("FileUploaded", "ras");
    }


    public async Task UploadFile(IFormFile file)
    {
        await Clients.All.SendAsync("FormFile", "ras");
    }
}