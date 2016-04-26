namespace OnlinerTracker.Interfaces
{
    public interface IMessageSender
    {
        void SendEmail(string subject, string body, string recipientEmail, bool isBodyHtml = true);
    }
}
