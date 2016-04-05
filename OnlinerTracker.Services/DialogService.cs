using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;

namespace OnlinerTracker.Services
{
    [HubName("dialog")]
    public class DialogService : IDialogService
    {
        private IHubContext _context;
        public DialogService(IHubContext context)
        {
            _context = context;
        }

        public void SendInPopupForAll(PopupType popupType, string message)
        {
            _context.Clients.All.showPopup(popupType.ToString(), message);
        }

        public void SendInPopupForUser(PopupType popupType, string message, string connectionId)
        {
            _context.Clients.Client(connectionId).showPopup(popupType.ToString(), message);
        }
    }
}
