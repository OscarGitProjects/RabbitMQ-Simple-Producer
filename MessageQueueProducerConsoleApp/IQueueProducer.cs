using RabbitMQ.Client;

namespace MessageQueueProducerConsoleApp
{
    public interface IQueueProducer
    {
        void SendMessage(IModel channel, String strMessage, String strQueueName = "default-message-queue", String strExchangeName = "default-exchange");
        void Run(string strMessage = "Hello world", int iNumberOfMessages = 100);
    }
}