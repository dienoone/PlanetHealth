using Microsoft.AspNetCore.SignalR;
using PlantHealth.Api.Constants;
using PlantHealth.Api.Models;

namespace PlantHealth.Api.Hubs;

public class Camera : Hub 
{
    private readonly ILogger<Camera> _logger;
    private readonly string _id;
    public Camera(ILogger<Camera> logger)
    {
        _logger = logger;
        _id = Guid.NewGuid().ToString();
    }

    public string Get(string target) => $"Hello {target} {_id}";

    public async Task ReceiveStream(IAsyncEnumerable<string> messages, string param)
    {
        _logger.LogInformation($"starting to read stream: {param}");

        await foreach (var message in messages)
        {
            _logger.LogInformation($"Receiving {message} {param} {_id}");
        }

        _logger.LogInformation("finished stream");
    } 

    public override async Task OnConnectedAsync()
    {
        
        Console.WriteLine("hit...");
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

    public async Task SendFile()
    {
        await Clients.Group(GroupName.RASPBERRYPI).SendAsync("startcapture");
    }

    public async Task SendFrame(UploadFileChunk chunk)
    {
        Console.WriteLine("Received frame...");
        try
        {
            await Clients.Group(GroupName.FLUTTER).SendAsync("ReceiveFrame", chunk);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing image: {ex.Message}");
        }
    }

}