using OnlinerTracker.Data;
using OnlinerTracker.Data.Context;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OnlinerTracker.Services
{
    public class NotificationService : INotificationService
    {
        #region Fields and Properties

        private readonly string HtmlTemplite = "<html><body>{0}</body></html>";

        private readonly SmtpClient _defaultServiceClient;

        private readonly SecurityRepository _securityRepo;

        private readonly IProductService _productService;

        private readonly TrackerContext _context;

        private readonly string _smtpAccount;

        public string EmailSenderName { get; set; }

        public string HourInWhichSendingStart { get; set; }

        #endregion

        #region Constructor

        public NotificationService(IProductService productService, string smtpHost, string smtpPortString,
            string smtpAccount, string smtpPassword)
        {
            int smtpPort;
            int.TryParse(smtpPortString, out smtpPort);
            _defaultServiceClient = new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpAccount, smtpPassword)
            };
            _smtpAccount = smtpAccount;
            _productService = productService;
            _context = new TrackerContext();
            _securityRepo = new SecurityRepository();
        }

        #endregion

        #region INotificationService methods

        public async Task<string> CheckNotifications()
        {
            var hourInWhichSendingStart = 0;
            int.TryParse(HourInWhichSendingStart, out hourInWhichSendingStart);
            var lastCheck =
                _context.JobsLogs.OrderByDescending(u => u.CheckedAt)
                    .FirstOrDefault(u => u.Type == JobType.EmailSend && u.IsSuccessed);
            if ((lastCheck != null && lastCheck.CheckedAt.Hour == hourInWhichSendingStart) || DateTime.Now.Hour != hourInWhichSendingStart)
                return string.Empty;

            try
            {
                var usersWithUpdateCount = 0;
                foreach (var user in _securityRepo.GetAllUsers())
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        var products = _productService.GetAllChanges(Guid.Parse(user.Id));
                        var count = products.Count();
                        if (count > 0)
                        {
                            EmailSend("Updates for you", 
                                products.Aggregate(string.Empty, (current, product) => current + (product.Name + " <b>" + product.CurrentCost + "</b> ( old:" + product.DayAgoCost + ")<br/>")), 
                                user.Email);
                            usersWithUpdateCount++;
                        }
                    }
                }
                _context.JobsLogs.Add(new JobLog
                {
                    Type = JobType.EmailSend,
                    CheckedAt = DateTime.Now,
                    IsSuccessed = true,
                    Info = string.Format("Users number, who gets updates: {0}", usersWithUpdateCount)
                });
                _context.SaveChanges();

                return string.Empty;
            }
            catch (Exception ex)
            {
                _context.JobsLogs.Add(new JobLog
                {
                    Type = JobType.EmailSend,
                    CheckedAt = DateTime.Now,
                    IsSuccessed = false,
                    Info = ex.Message
                });
                _context.SaveChanges();
                return ex.Message;
            }
        }

        #endregion

        #region Additional methods

        public void EmailSend(string subject, string body, string recipientEmail)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_smtpAccount, EmailSenderName),
                Subject = subject,
                Body = string.Format(HtmlTemplite, body),
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(recipientEmail));
            message.ReplyToList.Add(new MailAddress(_smtpAccount, EmailSenderName));
            _defaultServiceClient.SendAsync(message, null);
        }

        #endregion
    }
}