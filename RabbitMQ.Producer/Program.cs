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
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            var factory = new ConnectionFactory {HostName = "localhost"};
            var _connection = factory.CreateConnection();
            var chanel = _connection.CreateModel();
            chanel.QueueDeclare("chanel", false, false, false, null);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(chanel));
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            _connection.Close();
        }
    }
}
