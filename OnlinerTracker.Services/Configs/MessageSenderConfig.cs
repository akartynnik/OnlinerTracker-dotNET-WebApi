namespace OnlinerTracker.Services.Configs
{
    public class MessageSenderConfig
    {
        public string SmtpHost { get; set; }

        public string SmtpPortString { get; set; }

        public string SmtpAccount { get; set; }

        public string SmtpPassword { get; set; }

        public string EmailSenderName { get; set; }

        public string HourInWhichSendingStart { get; set; }
    }
}
