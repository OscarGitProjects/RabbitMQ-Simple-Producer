using RabbitMQ.Client;

namespace MessageQueueProducerConsoleApp
{
    public class Program : SimpleQueueProducer
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            IModel channel = null;

            try
            {
                SimpleQueueProducer producer = new SimpleQueueProducer();

                // Create a IModel channel to RabbitMQ
                channel = producer.CreateChannel("guest", "guest", "/", "localhost", 5672);

                Console.WriteLine("Running SimpleQueueProducer...");

                // Send 100 messages to Rabbit MQ
                int iCount = 0;
                int iSendCount = 0;

                while (iSendCount < 100)
                {
                    // Send message
                    producer.SendSimpleMessage(channel, "Hello world " + iCount, "simpleMessage");
                    iCount++;
                    iSendCount++;

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex) { 
                Console.WriteLine("Program->Main() exception: " + ex.ToString());
            }
            finally
            {
                if (channel != null)
                    channel.Close();
            }            

            Console.WriteLine("Press a key to close application");
            Console.ReadLine();
        }
    }
}