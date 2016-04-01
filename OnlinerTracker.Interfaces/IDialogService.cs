using OnlinerTracker.Data;

namespace OnlinerTracker.Interfaces
{
    public interface IDialogService
    {
        void ShowDialogBox(DialogType dialogType, string message);
    }
}
