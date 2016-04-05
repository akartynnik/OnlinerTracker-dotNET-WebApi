using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;

namespace OnlinerTracker.Services
{
    [HubName("dialog")]
    public class DialogService : IDialogService
    {
        private IHubContext _hubContext;
        public DialogService(IHubContext hubContext)
        {
            _hubContext = hubContext;
        }

        public void SendInPopupForAll(PopupType popupType, string message)
        {
            _hubContext.Clients.All.showPopup(popupType.ToString(), message);
        }

        public void SendInPopupForUser(PopupType popupType, string message, string connectionId)
        {
            _hubContext.Clients.Client(connectionId).showPopup(popupType.ToString(), message);
        }
    }
}
