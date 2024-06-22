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
        
        _logger.LogDebug($"A user is connection with a connection id ${Context.ConnectionId}");
        return base.OnConnectedAsync();
    }


    // update image will be class by the rasperbyi:
    public async void UpdateFile(IFormFile file)
    {
        var data = file.Name;
        Console.WriteLine(data);
        await Clients.All.SendAsync("FileUploaded", "Hello from the server");
    }
}