using MessageQueueProducerConsoleApp.UI;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MessageQueueProducerConsoleApp
{
    /// <summary>
    /// Topic exchange uses routing key, but it does not do an exact match on the routing key. 
    /// Instead it does a patterna match based on the pattern
    /// </summary>
    public class TopicExchangeQueueProducer : BaseQueueProducer, IQueueProducer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ui">Reference to user interface</param>
        public TopicExchangeQueueProducer(IUI ui) : base(ui)
        {}


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
        public void SendMessage(IModel channel, string strMessage, string strQueueName = "default-message-queue", string strExchangeName = "default-exchange", string strRoutingKey = "acount.init")
        {
            if (channel == null)
                throw new ArgumentNullException($"{nameof(TopicExchangeQueueProducer)}->SendMessage(). Reference to IModel channel is null");

            try
            {
                // Create the message
                var message = new { Producer = "TopicExchangeQueueProducer", Message = strMessage };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish(
                    exchange: strExchangeName,
                    routingKey: strRoutingKey,
                    basicProperties: null,
                    body: body);

                this.m_Ui.WriteLine($"Sent topic exchange message {message} to RabbitMQ");
            }
            catch (Exception exc)
            {
                this.m_Ui.WriteLine($"{nameof(TopicExchangeQueueProducer)}->SendMessage() exception: " + exc.ToString());
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
                // Create a IModel channel to RabbitMQ
                channel = this.CreateChannel("guest", "guest", "/", "localhost", 5672);

                // Create exchange
                string strExchangeName = "topic-exchange";
                string strQueueName = "topic-exchange-message-queue";
                string strRoutingKey = "acount.default";

                var timeToLive = new Dictionary<string, Object>
                {
                    { "x-message-ttl", 30000}
                };

                channel.ExchangeDeclare(
                    exchange: strExchangeName,
                    type: ExchangeType.Topic,
                    durable: false,
                    autoDelete: false,
                    arguments: timeToLive);

                this.m_Ui.WriteLine($"Running TopicExchangeQueueProducer... Sending {iNumberOfMessages} messages");

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
                this.m_Ui.WriteLine($"{nameof(TopicExchangeQueueProducer)}->Run() exception: " + exc.ToString());
                throw;
            }
            finally
            {
                channel?.Close();
            }
        }
    }
}