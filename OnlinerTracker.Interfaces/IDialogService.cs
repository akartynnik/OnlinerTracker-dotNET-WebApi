using OnlinerTracker.Data;

namespace OnlinerTracker.Interfaces
{
    public interface IDialogService
    {
        void SendInPopupForAll(PopupType popupType, string message);
        void SendInPopupForUser(PopupType popupType, string message, string connectionId);
    }
}
