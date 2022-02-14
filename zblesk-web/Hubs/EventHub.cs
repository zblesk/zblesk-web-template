using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace zblesk_web.Hubs;

public interface IEventHub
{
    Task Notify(string message);
}

[Authorize]
public class EventHub : Hub<IEventHub>
{
}
