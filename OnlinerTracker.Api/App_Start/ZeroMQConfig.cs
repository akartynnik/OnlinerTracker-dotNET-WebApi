using NetMQ;
using NetMQ.WebSockets;

namespace OnlinerTracker.Api
{
    public class ZeroMQConfig
    {
        public static void Register()
        {
            using (NetMQContext context = NetMQContext.Create())
            {
                using (WSRouter router = context.CreateWSRouter())
                using (WSPublisher publisher = context.CreateWSPublisher())
                {
                    router.Bind("ws://localhost:80");
                    publisher.Bind("ws://localhost:81");

                    router.ReceiveReady += (sender, eventArgs) =>
                    {
                        byte[] identity = eventArgs.WSSocket.Receive();
                        string message = eventArgs.WSSocket.ReceiveString();

                        eventArgs.WSSocket.SendMore(identity).Send("OK");

                        publisher.SendMore("chat").Send(message);
                    };

                    Poller poller = new Poller();
                    poller.AddSocket(router);
                    poller.AddSocket(publisher);
                    poller.Start();
                }
            }
        }
    }
}
