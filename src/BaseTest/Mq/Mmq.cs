using Raven.Message.RabbitMQ;
using Raven.Message.RabbitMQ.Abstract;
using Raven.Message.RabbitMQ.Configuration;
using Raven.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace btest
{
    public class Mmq
    {
        public static void Test()
        {
            //WxEventMQClientHelper.GetInstance().Publish(10010, "user_view_card", "0:no sub");
            WxEventMQClientHelper.GetInstance().Subscribe(mallID:10008,busType:"tomtest");
            return;

            //WxEventMQClientHelper.GetInstance().Publish(10005, "user_view_card_pre", "1:05 pre");
            WxEventMQClientHelper.GetInstance().Publish(10005, "user_view_card", "view card 1");
            WxEventMQClientHelper.GetInstance().Publish(10005, "user_view_card", "3:05 view card 2");
            WxEventMQClientHelper.GetInstance().Publish(10005, "user_view_card", "4:05 view card 3");
            WxEventMQClientHelper.GetInstance().Publish(10008, "user_view_card", "5:08 view card 4");
            //WxEventMQClientHelper.GetInstance().Subscribe(mallID:10005);
        }
    }

    public class WxEventMQClientHelper
    {
        int i = 0;
        private static string _ExChange = "wx.test";
        private static string _KeyFormat = "wxevent.{0}.{1}";
        private static string _ConnectionString = "amqp://guest:guest@localhost:5672";//

        private static IRabbitClient _RabbitClient;

        static WxEventMQClientHelper()
        {
            _Instance = new WxEventMQClientHelper();
            _RabbitClient = ClientFactory.Create(config: new ClientConfiguration
            {
                Uri = _ConnectionString,
                Name = "rabbit",
                SerializerType = SerializerType.NewtonsoftJson
            },log:new Log());
        }
        private static WxEventMQClientHelper _Instance;
        public static WxEventMQClientHelper GetInstance()
        {
            return _Instance;
        }

        public void Publish(long mallID, string eventType = "user_view_card", string msg ="")
        {
            var routeKey = string.Format(_KeyFormat, eventType, mallID);

            var b = _RabbitClient.Publish(msg, routeKey, _ExChange);
            Console.WriteLine("Publish msg:{0} ex:{1} k:{2} result:{3}", msg, _ExChange, routeKey, b);
        }

        public void Subscribe(string busType="wxcardspc",string eventType= "user_view_card", long mallID=10001)
        {
            var routeKey = string.Format(_KeyFormat, eventType, mallID);
            var queue = routeKey + "." + busType;
            var b = _RabbitClient.Subscribe<string>(queue, _ExChange, routeKey, msg => MessageReceived(msg));
            Console.WriteLine("Subscribe q:{0} ex:{1} k:{2} result:{3}", queue, _ExChange, routeKey, b);
        }

        private bool MessageReceived(string msg)
        {
            Console.WriteLine("{2} MessageReceived {0} at i:{1}", msg, i, DateTime.Now.ToShortTimeString());
            if (i == 1)
            {
                Thread.Sleep(5000);
                Console.WriteLine("{3} MessageReceived {0} at i:{1} sleep {2}s", msg, i, 5, DateTime.Now.ToShortTimeString());
            }
            i++;
            return true;
        }

    }
    class Log : ILog
    {
        public void LogDebug(string info, object dataObj)
        {
            Console.WriteLine("log {0}",info);
        }

        public void LogError(string errorMessage, Exception ex, object dataObj)
        {
            Console.WriteLine("log err {0}", errorMessage +":"+ex);
        }
    }



}
