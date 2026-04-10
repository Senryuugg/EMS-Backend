using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EmsDispatch.Backend.Hubs;

public interface ILocationHubClient
{
    Task ReceiveLocationUpdate(string driverId, double latitude, double longitude);
    Task ReceiveAmbulanceLocationUpdate(string ambulanceId, double latitude, double longitude);
}

public class LocationHub : Hub<ILocationHubClient>
{
    private readonly ILogger<LocationHub> _logger;

    public LocationHub(ILogger<LocationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation($"User {userId} connected to LocationHub for real-time tracking");

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"location_user_{userId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, "location_trackers");
        }

        await base.OnConnectedAsync();
    }

    public async Task UpdateLocation(double latitude, double longitude, string? address = null)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return;

        _logger.LogDebug($"Location update from user {userId}: {latitude}, {longitude}");

        // Broadcast to all connected clients
        await Clients.Group("location_trackers").ReceiveLocationUpdate(userId, latitude, longitude);
    }

    public async Task UpdateAmbulanceLocation(string ambulanceId, double latitude, double longitude, string? address = null)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return;

        _logger.LogDebug($"Ambulance {ambulanceId} location update: {latitude}, {longitude}");

        // Broadcast to all tracking clients
        await Clients.Group("location_trackers").ReceiveAmbulanceLocationUpdate(ambulanceId, latitude, longitude);
    }
}
