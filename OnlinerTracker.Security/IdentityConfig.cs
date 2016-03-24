namespace OnlinerTracker.Security
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    /*
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var smtpClient = GetSmtpClient();

            var @from = new MailAddress(ConfigurationManager.AppSettings["emailService:DisplayAddress"],
                                        ConfigurationManager.AppSettings["emailService:DisplayName"]);

            var @to = new MailAddress(message.Destination);

            var messageToSend = new MailMessage(@from, @to)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(messageToSend);
        }

        private static SmtpClient GetSmtpClient()
        {
            int port;
            var portSpecified = int.TryParse(ConfigurationManager.AppSettings["emailService:SmtpPort"], out port);
            var host = ConfigurationManager.AppSettings["emailService:SmtpHost"];

            var smtpClient = portSpecified ? new SmtpClient(host, port) : new SmtpClient(host);

            smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailService:AccountLogin"],
                                                           ConfigurationManager.AppSettings["emailService:AccountPassword"]);

            smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["emailService:SmtpEnableSsl"]);
            return smtpClient;
        }
    }


    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<SecurityContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }*/
}
