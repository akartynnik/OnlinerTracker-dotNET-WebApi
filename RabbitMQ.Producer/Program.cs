using RabbitMQ.Client;
using System;
using System.Windows.Forms;

namespace RabbitMQ.Producer
{
    static class Program
    {
        private static IConnection _connection;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread] 
        static void Main()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            var _connection = factory.CreateConnection();
            var channel = _connection.CreateModel();
            channel.ExchangeDeclare("first-exchange", "fanout");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(channel));
        }
    }
}
