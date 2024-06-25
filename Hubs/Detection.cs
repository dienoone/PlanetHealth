using Microsoft.AspNetCore.SignalR;
using PlantHealth.Api.Constants;

namespace PlantHealth.Api.Hubs;

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

    #region Test Connections only

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

        // Check if the file exists
        if (File.Exists(imagePath))
        {
            // Read the image as bytes
            byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

            // Send the bytes to the clients in the group "FLUTTER"
            await Clients.Group(GroupName.FLUTTER).SendAsync("ReceiveImage", imageBytes);
        }
        else
        {
            // Handle the case where the file does not exist
            await Clients.Group(GroupName.FLUTTER).SendAsync("ReceiveImage", null);
        }
    }

    #endregion 
   
    #region Button One

    /// <summary>
    /// [First button]
    /// The flutter will invoke this method,
    /// the time can be optional.
    /// 
    /// This raspberry pi will call the upload file controller.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public async Task InitCapture(int time = 15)
    {
        await Clients.Group(GroupName.RASPBERRYPI).SendAsync("StartCapture", time);
    }

    #endregion

    #region Button Two

    /// <summary>
    /// [Second button]
    /// The flutter will invoke this method, to stop receive images.
    /// </summary>
    /// <returns></returns>
    public async Task EndCapture()
    {
        await Clients.Group(GroupName.RASPBERRYPI).SendAsync("StopCapture");
    }

    #endregion

    #region Button Three

    /// <summary>
    /// The flutter will invoke this method, to get live stream.
    /// </summary>
    /// <returns></returns>
    public async Task GetLiveStream()
    {
        await Clients.Group(GroupName.RASPBERRYPI).SendAsync("StartLiveStream");
    }

    /// <summary>
    /// The raspberry pi will invoke this method, to upload file stream, to 
    /// flutter team.
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public async Task UploadLiveStream(string chunk, int index, int totalChunks)
    {
        try
        {
            Console.WriteLine("here");
            await Clients.Group(GroupName.FLUTTER).SendAsync("ReceiveFrame", chunk, index, totalChunks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing image: {ex.Message}");
        }
    }

    #endregion

    #region Button Four

    /// <summary>
    /// The flutter will invoke this method, to capture an image,
    /// during the live stream.
    /// 
    /// The raspberry pi will call the upload file controller.
    /// </summary>
    /// <returns></returns>
    public async Task CaptureImage()
    {
        await Clients.Group(GroupName.RASPBERRYPI).SendAsync("TakeImage");
    }


    #endregion

    #region Button Five

    /// <summary>
    /// The flutter will invoke this mathod, to end receive live stream.
    /// </summary>
    /// <returns></returns>
    public async Task StopLiveStream()
    {
        await Clients.Group(GroupName.RASPBERRYPI).SendAsync("EndLiveStream");
    }

    #endregion 

}