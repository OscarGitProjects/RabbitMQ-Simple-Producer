using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MessageQueueProducerConsoleApp
{
    public class SimpleQueueProducer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SimpleQueueProducer()
        {
        }

        /// <summary>
        /// Method create a IModel channel to RabbitMQ
        /// </summary>
        /// <param name="strUserName">Username</param>
        /// <param name="strPassword">Password</param>
        /// <param name="strVirtualHost">Virtual host</param>
        /// <param name="strHostName">Hostname</param>
        /// <param name="iPortNumber">Port number</param>
        /// <returns>IModel channel</returns>
        /// <exception cref="Exception">Throws exception</exception>
        public IModel CreateChannel(String strUserName = "guest", String strPassword = "guest", String strVirtualHost = "/", String strHostName="localhost", int iPortNumber=5672)
        {
            try
            {
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = strHostName,
                    Port = iPortNumber,
                    UserName = strUserName,
                    Password = strPassword,
                    VirtualHost = strVirtualHost
                };

                var connection = connectionFactory.CreateConnection();
                var channel = connection.CreateModel();
                return channel;
            }
            catch(Exception exc)
            {
                Console.WriteLine($"{nameof(SimpleQueueProducer)}->CreateChannel() exception: " + exc.ToString());
                throw;
            }            
        }


        /// <summary>
        /// Method sends a message to a queue in RabbitMQ
        /// </summary>
        /// <param name="channel">IModel channel</param>
        /// <param name="strMessage">Message</param>
        /// <param name="strQueueName">Name of queue</param>
        /// <exception cref="ArgumentNullException">Throws if reference to IModel channel is null</exception>
        /// <exception cref="Exception">Throws exception</exception>
        public void SendSimpleMessage(IModel channel, String strMessage, String strQueueName = "simpleMessage")
        {
            if (channel == null)
                throw new ArgumentNullException($"{nameof(SimpleQueueProducer)}->SendSimpleMessage(). Reference to IModel channel is null");

            try
            {
                channel.QueueDeclare(queue: strQueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // Create the message
                var message = new { Producer = "SimpleQueueProducer", Message = strMessage };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish(exchange: "",
                    routingKey: strQueueName,
                    basicProperties: null,
                    body: body);

                Console.WriteLine($"Sent message {message} to RabbitMQ");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"{nameof(SimpleQueueProducer)}->SendSimpleMessage() exception: " + exc.ToString());
                throw;
            }
        }
    }
}