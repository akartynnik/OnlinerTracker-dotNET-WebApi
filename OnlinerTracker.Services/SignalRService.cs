using Microsoft.AspNet.SignalR;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;

namespace OnlinerTracker.Services
{

    public class SignalRService : IDialogService
    {
        private readonly IHubContext _dialogContext;
        public SignalRService(IHubContext dialogContext)
        {
            _dialogContext = dialogContext;
        }
        public void SendInPopupForUser(PopupType popupType, string message, string connectionId)
        {
            _dialogContext.Clients.Client(connectionId).showPopup(popupType.ToString(), message);
        }
    }
}
