using NetMQ;
using NetMQ.WebSockets;
using System;
using System.Threading.Tasks;

namespace OnlinerTracker.Services.Contexts
{
    public class NetMqDialogContext
    {
        private Poller _poller;

        private WSPublisher _publisher;

        private WSRouter _router;

        private string _routerUrl;

        private string _publisherUrl;

        public NetMqDialogContext(string routerUrl, string publisherUrl)
        {
            _routerUrl = routerUrl;
            _publisherUrl = publisherUrl;
            Initialize();
        }

        public void Initialize()
        {
            Task.Factory.StartNew(() =>
            {
                using (NetMQContext context = NetMQContext.Create())
                {
                    using ( _router = context.CreateWSRouter())
                    using (_publisher = context.CreateWSPublisher())
                    {
                        _router.Bind(_routerUrl);
                        _publisher.Bind(_publisherUrl);

                        _router.ReceiveReady += SendConnectionIdForCurrentSession;

                        _poller = new Poller();
                        _poller.AddSocket(_router);
                        _poller.Start();

                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void SendConnectionIdForCurrentSession(object sender, WSSocketEventArgs eventArgs)
        {
            var identity = eventArgs.WSSocket.Receive();
            var receivedMessage = eventArgs.WSSocket.ReceiveString();
            //if on client start new session - give him new connectionId
            if (receivedMessage == "wantId")
            {
                eventArgs.WSSocket.SendMore(identity).Send(Guid.NewGuid().ToString());
            }
        }

        public void Send(string connectionId, string message)
        {
            _publisher.SendMore(connectionId).Send(message);
        }
    }
}
