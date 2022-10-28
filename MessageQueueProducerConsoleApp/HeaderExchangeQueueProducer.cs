using MessageQueueProducerConsoleApp.UI;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MessageQueueProducerConsoleApp
{
    /// <summary>
    /// Header exchange routes messages based on header values and are very similar to Topic exchange.
    /// Topic exchange uses routing key, but it does not do an exact match on the routing key. 
    /// Instead it does a patterna match based on the pattern
    /// </summary>
    public class HeaderExchangeQueueProducer : BaseQueueProducer, IQueueProducer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ui">Reference to user interface</param>
        public HeaderExchangeQueueProducer(IUI ui) : base(ui)
        { }


        /// <summary>
        /// Method sends a message to a queue in RabbitMQ
        /// </summary>
        /// <param name="channel">IModel channel</param>
        /// <param name="strMessage">Message</param>
        /// <param name="strQueueName">Name of queue</param>
        /// <param name="strExchangeName">Name of the exchange</param>
        /// <param name="strRoutingKey">Routing key</param>
        /// <exception cref="ArgumentNullException">Throws if reference to IModel channel is null</exception>
        /// <exception cref="Exception">Throws exception</exception>
        public void SendMessage(IModel channel, String strMessage, String strQueueName = "default-message-queue", String strExchangeName = "default-exchange", String strRoutingKey = "acount.init")
        {
            if (channel == null)
                throw new ArgumentNullException($"{nameof(HeaderExchangeQueueProducer)}->SendMessage(). Reference to IModel channel is null");

            try
            {
                // Create the message
                var message = new { Producer = "HeaderExchangeQueueProducer", Message = strMessage };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                var properties = channel.CreateBasicProperties();
                properties.Headers = new Dictionary<string, Object> { { "account", "new" } };

                channel.BasicPublish(
                    exchange: strExchangeName,
                    routingKey: String.Empty,
                    basicProperties: properties,
                    body: body);

                this.m_Ui.WriteLine($"Sent header exchange message {message} to RabbitMQ");
            }
            catch (Exception exc)
            {
                this.m_Ui.WriteLine($"{nameof(HeaderExchangeQueueProducer)}->SendMessage() exception: " + exc.ToString());
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

                // Create exchange
                string strExchangeName = "header-exchange";
                string strQueueName = "header-exchange-message-queue";
                string strRoutingKey = String.Empty;

                var timeToLive = new Dictionary<string, Object>
                {
                    { "x-message-ttl", 30000}
                };

                channel.ExchangeDeclare(
                    exchange: strExchangeName,
                    type: ExchangeType.Headers,
                    durable: false,
                    autoDelete: false,
                    arguments: timeToLive);

                this.m_Ui.WriteLine($"Running HeaderExchangeQueueProducer... Sending {iNumberOfMessages} messages");

                // Send iNumberOfMessages messages to RabbitMQ
                int iCount = 0;

                while (iCount < iNumberOfMessages)
                {
                    // Send message
                    this.SendMessage(channel, strMessage + " " + iCount, strQueueName, strExchangeName, strRoutingKey);
                    iCount++;

                    Thread.Sleep(1000);
                }
            }
            catch (Exception exc)
            {
                this.m_Ui.WriteLine($"{nameof(HeaderExchangeQueueProducer)}->Run() exception: " + exc.ToString());
                throw;
            }
            finally
            {
                channel?.Close();
            }
        }
    }
}
