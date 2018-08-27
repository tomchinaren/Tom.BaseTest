using btest.Advance;
using btest.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("begin main");

            //EFTest.Test();
            //EFTest.Test("mysql");
            //MongoDemo.Test().Wait();
            //MongoDemo.Test2().Wait();

            //CacheTest.Test();
            //MsmqDemo.Test();
            //Rbmq.Test();
            //MqDemo.Test();
            //Mmq.Test();
            //PageDomainDemo.Run();
            //Base_eBay.BaseDemo.Run();

            //ClosureDemo.Test();
            RedisPfmTest.TestRound();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            Console.WriteLine("end main");
        }

        static void sleep()
        {
            System.Threading.Thread.Sleep(1000 * 100);
        }

    }
}


