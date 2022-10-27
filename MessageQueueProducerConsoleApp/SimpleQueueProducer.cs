using MessageQueueProducerConsoleApp.UI;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MessageQueueProducerConsoleApp
{
    public class SimpleQueueProducer : BaseQueueProducer, IQueueProducer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ui">Reference to user interface</param>
        public SimpleQueueProducer(IUI ui) : base(ui)
        {
        }

        /// <summary>
        /// Method sends a message to a queue in RabbitMQ
        /// </summary>
        /// <param name="channel">IModel channel</param>
        /// <param name="strMessage">Message</param>
        /// <param name="strQueueName">Name of queue</param>
        /// <param name="strExchangeName">Name of the exchange</param>
        /// <exception cref="ArgumentNullException">Throws if reference to IModel channel is null</exception>
        /// <exception cref="Exception">Throws exception</exception>
        public void SendMessage(IModel channel, String strMessage, String strQueueName = "default-message-queue", String strExchangeName = "default-exchange")
        {
            if (channel == null)
                throw new ArgumentNullException($"{nameof(SimpleQueueProducer)}->SendMessage(). Reference to IModel channel is null");

            try
            {
                // Create the message
                var message = new { Producer = "SimpleQueueProducer", Message = strMessage };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish(
                    exchange: "",
                    routingKey: strQueueName,
                    basicProperties: null,
                    body: body);

                this.m_Ui.WriteLine($"Sent simple message {message} to RabbitMQ");
            }
            catch (Exception exc)
            {
                this.m_Ui.WriteLine($"{nameof(SimpleQueueProducer)}->SendMessage() exception: " + exc.ToString());
                throw;
            }
        }


        /// <summary>
        /// Method is sending a number of messages to RabbitMQ
        /// </summary>
        /// <param name="strMessage">Message</param>
        /// <param name="iNumberOfMessages">Number of messages that shall be sent</param>
        /// <exception cref="Exception">Throws exception</exception>
        public void Run(string strMessage = "Hello world", int iNumberOfMessages = 10)
        {
            IModel? channel = null;

            try
            {
                //SimpleQueueProducer producer = new SimpleQueueProducer();

                // Create a IModel channel to RabbitMQ
                channel = this.CreateChannel("guest", "guest", "/", "localhost", 5672);

                // Create queue
                string strQueueName = "simple-message-queue";
                channel.QueueDeclare(
                    queue: strQueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                this.m_Ui.WriteLine($"Running SimpleQueueProducer... Sending {iNumberOfMessages} messages");

                // Send iNumberOfMessages messages to RabbitMQ
                int iCount = 0;

                while (iCount < iNumberOfMessages)
                {
                    // Send message
                    this.SendMessage(channel, strMessage + " " + iCount, strQueueName);
                    iCount++;

                    Thread.Sleep(1000);
                }
            }
            catch (Exception exc)
            {
                this.m_Ui.WriteLine("SimpleQueueProducer->Run() exception: " + exc.ToString());
                throw;
            }
            finally
            {
                if (channel != null)
                    channel.Close();
            }
        }
    }
}