using Microsoft.AspNetCore.SignalR;

namespace PlantHealth.Api.Hubs;

public class Detection : Hub 
{
    private ILogger<Detection> _logger;

    public Detection(ILogger<Detection> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        
        _logger.LogWarning($"A user is connection with a connection id ${Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }


    // update image will be class by the rasperbyi:
    public async Task UpdateFile(string file, string fileContent)
    {
        Console.WriteLine(file);
        Console.WriteLine(fileContent);
        await Clients.All.SendAsync("FileUploaded", "ras");
    }


    public async Task UploadFile(IFormFile file)
    {
        await Clients.All.SendAsync("FormFile", "ras");
    }
}