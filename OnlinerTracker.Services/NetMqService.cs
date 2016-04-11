using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Services.Contexts;

namespace OnlinerTracker.Services
{
    public class NetMqService : IDialogService
    {
        private readonly NetMqDialogContext _dialogContext;
        
        public NetMqService(NetMqDialogContext dialogContext)
        {
            _dialogContext = dialogContext;
        }

        public void SendInPopupForUser(PopupType popupType, string message, string connectionId)
        {
            _dialogContext.Send(connectionId, message);
        }
    }
}
