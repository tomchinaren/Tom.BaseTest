using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest.Advance
{
    public class ClosureDemo
    {
        static int staticCopy;
        public static void Test()
        {
            Test0();
            Test1();
            Test2();
            Test3();
        }
        static void Test0()
        {
            var actions = new List<Action>();
            for (int counter = 0; counter < 3; counter++)
            {
                staticCopy = counter;
                actions.Add(() => Console.WriteLine("0 static {0}",staticCopy));
            }
            foreach (var action in actions) action();
        }

        static void Test1()
        {
            int copy;//闭包一
            var actions = new List<Action>();
            for (int counter = 0; counter < 3; counter++)
            {
                copy = counter;
                actions.Add(() => Console.WriteLine("1 {0}",copy));
            }
            foreach (var action in actions) action();
        }


        static void Test2()
        {
            var actions = new List<Action>();
            for (int counter = 0; counter < 3; counter++)
            {
                int copy = counter;//闭包二
                actions.Add(() => Console.WriteLine("2 {0}", copy));
            }
            foreach (var action in actions) action();
        }

        static void Test3()
        {
            var actions = new List<Action>();
            for (int counter = 0; counter < 3; counter++)//[3]闭包三
            {
                actions.Add(() => Console.WriteLine("3 {0}", counter));
            }
            foreach (var action in actions) action();
        }
    }
}
