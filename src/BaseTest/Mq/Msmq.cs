using System;
using System.Messaging;
namespace btest
{
    public class Msmq
    {
        private string Path;
        /// <summary>
        /// 1.通过Create方法创建使用指定路径的新消息队列
        /// </summary>
        /// <param name="queuePath"></param>
        public void Createqueue(string queuePath)
        {
            try
            {
                if (!MessageQueue.Exists(queuePath))
                {
                    MessageQueue.Create(queuePath);
                }
                else
                {
                    Console.WriteLine(queuePath + "已经存在！");
                }
                Path = queuePath;
            }
            catch (MessageQueueException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///  2.连接消息队列并发送消息到队列
        /// 远程模式：MessageQueue rmQ = new MessageQueue("FormatName:Direct=OS:machinename//private$//queue");
        ///     rmQ.Send("sent to regular queue - Atul");对于外网的MSMQ只能发不能收
        /// </summary>
        public void SendMessage(object msg)
        {
            try
            {
                //连接到本地队列
                MessageQueue myQueue = new MessageQueue(Path);
                //MessageQueue myQueue = new MessageQueue("FormatName:Direct=TCP:192.168.12.79//Private$//myQueue1");
                //MessageQueue rmQ = new MessageQueue("FormatName:Direct=TCP:121.0.0.1//private$//queue");--远程格式
                Message myMessage = new Message();
                myMessage.Body = msg;
                myMessage.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                //发生消息到队列中
                myQueue.Send(myMessage);
                Console.WriteLine("发送消息：{0}", msg);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 3.连接消息队列并从队列中接收消息
        /// </summary>
        public void ReceiveMessage()
        {
            MessageQueue myQueue = new MessageQueue(Path);
            myQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            try
            {
                //从队列中接收消息
                Message myMessage = myQueue.Receive();// myQueue.Peek();--接收后不消息从队列中移除
                string context = myMessage.Body.ToString();
                Console.WriteLine("Receive消息内容：" + context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void PeekMessage()
        {
            try
            {
                MessageQueue myQueue = new MessageQueue(Path);
                myQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                //接收后不消息从队列中移除
                Message myMessage = myQueue.Peek();
                string context = myMessage.Body.ToString();
                Console.WriteLine("Peek消息内容：" + context);
            }
            catch(Exception e)
            {
                Console.WriteLine("Peek Err. {0}",e.Message);
            }

        }

        /// <summary>
        /// 4.清空指定队列的消息
        /// </summary>
        public void ClealMessage()
        {
            MessageQueue myQueue = new MessageQueue(Path);
            myQueue.Purge();
            Console.WriteLine("已清空对了{0}上的所有消息", Path);
        }

        /// <summary>
        /// 5.连接队列并获取队列的全部消息，不删除消息
        /// </summary>
        public void GetAllMessage()
        {
            MessageQueue myQueue = new MessageQueue(Path);
            Message[] allMessage = myQueue.GetAllMessages();
            XmlMessageFormatter formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            for (int i = 0; i < allMessage.Length; i++)
            {
                allMessage[i].Formatter = formatter;
                Console.WriteLine("第{0}条消息为:{1}", i + 1, allMessage[i].Body.ToString());
            }
        }

    }

    public class MsmqDemo
    {
        public static void Test()
        {
            var queue = new Msmq();
            queue.Createqueue(".\\Private$\\myQueue1");
            queue.SendMessage("msg 1 named tom!");
            queue.SendMessage("msg 2 named jerry!");
            queue.PeekMessage();
            queue.PeekMessage();
            queue.ReceiveMessage();
            queue.GetAllMessage();
            queue.ReceiveMessage();
            queue.ReceiveMessage();
            queue.PeekMessage();
            queue.ClealMessage();
        }

        public static void Test2()
        {
            var queue = new Msmq();
            queue.Createqueue(".\\Private$\\myQueue1");

            int num = 0;
            while (true) {
                queue.SendMessage(string.Format("msg {0}", num++));
                var key = Console.ReadLine();
                if (key == "exit")
                {
                    break;
                }
            }
        }
    }
}
