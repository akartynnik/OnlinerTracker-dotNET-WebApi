using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace OnlinerTracker.Services.Contexts
{
    [HubName("dialog")]
    public class SignalRDialogHub : Hub
    {
    }
}
