namespace OnlinerTracker.Interfaces
{
    public interface INotificationService
    {
        void SendNotifications(int hourInWhichSendingStart);

        void SendNotifications();
    }
}
