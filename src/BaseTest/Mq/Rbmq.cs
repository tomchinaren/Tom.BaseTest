using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace btest
{
    public class Rbmq
    {
        private static IConnection cnnection;
        private static IModel channel;
        static string _Host = "localhost";
        public static void PublishMsg(string exchange = "", string queue = "", string routingKey = "", object obj = null)
        {

            var factory = new ConnectionFactory();
            factory.HostName = _Host;
            factory.UserName = "guest";
            factory.Password = "guest";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    bool durable = true;
                    if (!string.IsNullOrEmpty(queue))
                    {
                        channel.QueueDeclare(queue, durable, false, false, null);
                        channel.QueueBind(queue, exchange, routingKey);
                    }

                    string message = "";// GetMessage(new string[] { "123" });
                    if (obj != null)
                    {
                        message = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    }
                    var properties = channel.CreateBasicProperties();
                    properties.SetPersistent(true);


                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange, routingKey, properties, body);
                    Console.WriteLine(" set {0}", message);
                }
            }
        }

        public static void Consume(string exchange,string queue,string routingKey)
        {
            var factory = new ConnectionFactory();
            factory.HostName = _Host;
            factory.UserName = "guest";
            factory.Password = "guest";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    bool durable = true;
                    if (!string.IsNullOrEmpty(queue))
                    {
                        channel.QueueDeclare(queue, durable, false, false, null);
                        channel.QueueBind(queue, exchange, routingKey);
                    }
                    channel.BasicQos(0, 1, false);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queue, false, consumer);

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000);

                        Console.WriteLine("Received {0}", message);
                        Console.WriteLine("Done");

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }

        public static void ConsumeAsync(string queue)
        {
            var factory = new ConnectionFactory();
            factory.HostName = _Host;
            factory.UserName = "guest";
            factory.Password = "guest";

            cnnection = factory.CreateConnection();
            channel = cnnection.CreateModel();
            channel.QueueDeclare(queue, durable: true, autoDelete: false, exclusive: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var msgBody = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine(string.Format("***接收时间:{0}，消息内容：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msgBody));
                channel.BasicAck(ea.DeliveryTag, false);
            };
            channel.BasicConsume(queue, noAck: false, consumer: consumer);
        }

        public static void TestPub()
        {
            PublishMsg("wx.event", "Event.user_view_card.10005", routingKey: "Event.user_view_card.10005", obj:new string[] { "jerry view card in 10005" });
        }
        public static void TestConsume()
        {
            Consume("wx.event", "Event.user_view_card.10005", "Event.UserViewCard.10010");
        }
        public static void TestConsumeAsync()
        {
            ConsumeAsync("Event.user_view_card.10005");
        }


    }
}
