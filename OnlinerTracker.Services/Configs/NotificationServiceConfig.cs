using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;

namespace OnlinerTracker.Services.Configs
{
    public class NotificationServiceConfig
    {
        public ISecurityRepository SecurityRepository { get; set; }

        public IProductService ProductService { get; set; }

        public ILogService LogService { get; set; }

        public IStringComposer StringComposer { get; set; }

        public IMessageSender MessageSender { get; set; }
    }
}
