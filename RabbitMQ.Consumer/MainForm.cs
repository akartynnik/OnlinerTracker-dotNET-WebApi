using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Windows.Forms;

namespace RabbitMQ.Consumer
{
    public partial class MainForm : Form
    {
        public MainForm(IModel chanel, string queueName)
        {
            InitializeComponent();

            var consumer = new EventingBasicConsumer(chanel);
            chanel.BasicConsume(queueName, true, consumer);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body) + Environment.NewLine;
                Invoke(new Action(() => mqBox.AppendText(message))); //put message in output box
                gotMessagesCountLbl.Text = (Convert.ToInt32(gotMessagesCountLbl.Text) + 1).ToString();
                //chanel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); //config basic Act tag. Need for clean memory of Q in RabbitMQ when message received"
            };
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
