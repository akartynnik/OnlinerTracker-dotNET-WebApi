using RabbitMQ.Client;
using System;
using System.Windows.Forms;

namespace RabbitMQ.Consumer
{
    internal static class Program
    {
        private static IConnection _connection;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            var _connection = factory.CreateConnection();
            var channel = _connection.CreateModel();
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                  exchange: "first-exchange",
                  routingKey: "");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(channel, queueName));
        }
    }
}