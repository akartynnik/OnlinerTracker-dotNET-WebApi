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

        public void ShowDialogBox(DialogType dialogType, string message)
        {
            _context.Clients.All.showDialogBox(dialogType.ToString(), message);
        }
    }
}
