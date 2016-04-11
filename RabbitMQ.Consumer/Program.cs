using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            var factory = new ConnectionFactory { HostName = "localhost" };
            var _connection = factory.CreateConnection();
            var chanel = _connection.CreateModel();
            chanel.QueueDeclare("chanel", false, false, false, null);
            var consumer = new EventingBasicConsumer(chanel);
            chanel.BasicConsume("chanel", true, consumer);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(consumer));
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            _connection.Close();
        }
    }
}