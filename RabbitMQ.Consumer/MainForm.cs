using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Windows.Forms;

namespace RabbitMQ.Consumer
{
    public partial class MainForm : Form
    {
        public MainForm(EventingBasicConsumer consumer)
        {
            InitializeComponent();
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body) + Environment.NewLine;
                Invoke(new Action(() => mqBox.AppendText(message))); //put message in output box
            };
        }
    }
}
