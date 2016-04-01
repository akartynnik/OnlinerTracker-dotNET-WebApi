using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace OnlinerTracker.Api.Hubs
{
    [HubName("popup")]
    public class PopupCommunicationHub: Hub
    {
        public void Initialize()
        {
        }
        public void SendMessage(string clientId, string message)
        {
            Clients.Client(clientId).sendMessage(message);
        }
    }
}