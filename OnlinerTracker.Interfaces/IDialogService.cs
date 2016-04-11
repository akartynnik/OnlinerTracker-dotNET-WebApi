using OnlinerTracker.Data;

namespace OnlinerTracker.Interfaces
{
    public interface IDialogService
    {
        void SendInPopupForUser(PopupType popupType, string message, string connectionId);
    }
}
