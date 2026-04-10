using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EmsDispatch.Backend.Hubs;

public interface IDispatchHubClient
{
    Task ReceiveDispatchCreated(object dispatch);
    Task ReceiveDispatchUpdated(object dispatch);
    Task ReceiveDispatchAssigned(string dispatchId, string driverId);
    Task ReceiveDispatchStatusChanged(string dispatchId, string newStatus);
}

public class DispatchHub : Hub<IDispatchHubClient>
{
    private readonly ILogger<DispatchHub> _logger;

    public DispatchHub(ILogger<DispatchHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = Context.User?.FindFirst("role")?.Value;

        _logger.LogInformation($"User {userId} connected to DispatchHub with role {userRole}");

        // Add user to role-based groups
        if (!string.IsNullOrEmpty(userRole))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"role_{userRole}");
        }

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation($"User {userId} disconnected from DispatchHub");

        await base.OnDisconnectedAsync(exception);
    }

    public async Task BroadcastDispatchCreated(object dispatch)
    {
        await Clients.Group("role_Dispatcher").ReceiveDispatchCreated(dispatch);
        await Clients.Group("role_Admin").ReceiveDispatchCreated(dispatch);
    }

    public async Task BroadcastDispatchUpdated(object dispatch)
    {
        await Clients.All.ReceiveDispatchUpdated(dispatch);
    }

    public async Task BroadcastDispatchAssigned(string dispatchId, string driverId)
    {
        await Clients.All.ReceiveDispatchAssigned(dispatchId, driverId);
    }

    public async Task BroadcastDispatchStatusChanged(string dispatchId, string newStatus)
    {
        await Clients.All.ReceiveDispatchStatusChanged(dispatchId, newStatus);
    }
}
