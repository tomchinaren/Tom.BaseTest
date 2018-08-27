using Raven.Message.RabbitMQ;
using Raven.Message.RabbitMQ.Configuration;
using Raven.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest
{
    public class MqDemo
    {
        private static string exchange = "gg.memberPro";
        private static string queue = "gg.memberPro.memberPlus.ChangeStatus.test";
        private string routeKey = "gg.memberPro.memberPlus.ChangeStatus";
        private static string connectionString = "amqp://guest:guest@localhost:5672";

        public static void Test()
        {
            var go = true;
            var mq = new MqDemo();
            var index = 0;
            while (go)
            {
                Console.WriteLine(" Press A to publish and B to consume.");
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.A)
                {
                    //Rbmq.TestPub();
                    mq.Publish((index++).ToString());
                }
                else if (key.Key == ConsoleKey.B)
                {
                    //Rbmq.TestConsumeAsync();
                    mq.Subscribe();
                }
                else
                {
                    go = false;
                }
            }
        }

        public bool Subscribe()
        {
            //配置
            var RabbitClient = ClientFactory.Create(config: new ClientConfiguration
            {
                Uri = connectionString,
                Name = "rabbit",
                SerializerType = SerializerType.NewtonsoftJson
            });
            //订阅
            return RabbitClient.Subscribe<string>(queue, exchange, routeKey, msg => MessageReceived(msg));
        }

        private bool MessageReceived(string msg)
        {
            Console.WriteLine("MessageReceived {0} ", msg);
            //TODO

            return true;
        }

        public bool Publish(string message)
        {
            //配置
            var RabbitClient = ClientFactory.Create(config: new ClientConfiguration
            {
                Uri = connectionString,
                Name = "rabbit",
                SerializerType = SerializerType.NewtonsoftJson
            });
            return RabbitClient.Publish(message, routeKey, exchange);
        }
    }
}
