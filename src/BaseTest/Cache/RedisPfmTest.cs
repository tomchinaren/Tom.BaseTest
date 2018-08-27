using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace btest
{
    public class RedisPfmTest
    {
        static int roundIndex = 0;
        static ICache cache = new Redis(1);

        private RedisPfmTest() { }
        public static void TestRound()
        {
            var min = 0;var max = 0;
            ThreadPool.GetMinThreads(out min, out max);
            Console.WriteLine("min:{0},max:{1}", min, max);
            ThreadPool.SetMinThreads(50, 50);
            ThreadPool.GetMinThreads(out min, out max);
            Console.WriteLine("min:{0},max:{1}", min, max);
            var pfm = new RedisPfmTest();
            var case1 = new Case1(pfm);
            var case2 = new Case2(pfm);
            var case3 = new Case3(pfm);
            var case4 = new Case4(pfm);
            var case3x = new Case3x(pfm);

            //case1.Test(10);
            //case2.Test(1);
            case3.Test(1);
            //case4.Test(1);
            //case3x.Test(1);
        }

        public void Run(string title, int beginNum, int endNum, bool needMsg = true)
        {
            //var cache = new Redis(1);

            var sw = new Stopwatch();
            sw.Start();
            var beginTime = DateTime.Now;

            var keyFormat = title + ":{0}";
            var i = beginNum;
            var key = "";
            while (i++ < endNum)
            {
                key = string.Format(keyFormat, i);
                cache.Insert(key, i);
            }

            sw.Stop();
            var endTime = DateTime.Now;
            var total = endNum - beginNum;
            //var msg = string.Format("round {0} count:{1:N} ms:{2} avg:{3:f2} tps:{4:f2}", ++roundIndex, total, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds * 1.0 / total, total * 1000.0 / sw.ElapsedMilliseconds, beginTime, endTime);
            var msg1 = string.Format("round {0} count:{1:N} ms:{2} avg:{3:f2} tps:{4:f2} {5:HH:mm:ss}-{6:HH:mm:ss} tid:{7}", ++roundIndex, total, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds * 1.0 / total, total * 1000.0 / sw.ElapsedMilliseconds, beginTime, endTime, Thread.CurrentThread.ManagedThreadId);
            if (needMsg)
            {
                Console.WriteLine(msg1);
            }
        }

        public List<Task> RunAsTask(string title, int beginNum, int endNum)
        {
            var tasks = new List<Task>();

            var sw = new Stopwatch();
            sw.Start();
            var beginTime = DateTime.Now;

            var keyFormat = title + ":{0}";
            var i = beginNum;
            var key = "";
            while (i++ < endNum)
            {
                key = string.Format(keyFormat, i);
                //cache.Insert(key, i);

                var myKey = key;
                var myI = i;
                tasks.Add(
                    Task.Factory.StartNew(() => { cache.Insert(myKey, myI); })
                );
            }

            return tasks;

            Task.WaitAll(tasks.ToArray());
            sw.Stop();
            var endTime = DateTime.Now;
            var total = endNum - beginNum;
            //var msg = string.Format("round {0} count:{1:N} ms:{2} avg:{3:f2} tps:{4:f2}", ++roundIndex, total, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds * 1.0 / total, total * 1000.0 / sw.ElapsedMilliseconds, beginTime, endTime);
            var msg1 = string.Format("round {0} count:{1:N} ms:{2} avg:{3:f2} tps:{4:f2} {5:HH:mm:ss}-{6:HH:mm:ss} tid:{7}", ++roundIndex, total, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds * 1.0 / total, total * 1000.0 / sw.ElapsedMilliseconds, beginTime, endTime, Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(msg1);
        }


        interface ICase
        {
            RedisPfmTest pfm{ get; set; }
            void Test(int round);
        }

        class Case1 : ICase
        {
            public RedisPfmTest pfm { get; set; }
            public Case1(RedisPfmTest pfm)
            {
                this.pfm = pfm;
            }
            public override string ToString()
            {
                return "Case1";
            }

            public void Test(int round)
            {
                Console.WriteLine("--{0} task count:{1} --", ToString(), 1);
                var lineStr = "-".PadLeft(20, '-');
                Console.WriteLine(lineStr);

                var k = 1000;
                var i = 0;
                var begin = 0;
                var end = 0;
                var total = 0;
                while (i < round)
                {
                    begin = i * k;
                    total = (i + 1) * k;
                    end = begin + total;
                    pfm.Run(this.ToString(), begin, end);
                    i++;
                }

                Console.WriteLine();
            }
        }

        class Case2 : ICase
        {
            public RedisPfmTest pfm { get; set; }
            public Case2(RedisPfmTest pfm)
            {
                this.pfm = pfm;
            }
            public override string ToString()
            {
                return "Case2";
            }

            /// <summary>
            /// 10万一次
            /// </summary>
            /// <param name="round"></param>
            public void Test(int round)
            {
                Console.WriteLine("--{0} {1} task count:{2} --", ToString(), "10w", 1);
                var lineStr = "-".PadLeft(30, '-');
                Console.WriteLine(lineStr);

                var k = 1000;
                var i = 0;
                var begin = 0;
                var end = 0;
                var total = 100*k;
                while (i < round)
                {
                    end = begin + total;
                    pfm.Run(this.ToString(), begin, end);
                    i++;
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// 多Task，根据测试结果和算法来看，建议采用这种方式
        /// </summary>
        class Case3 : ICase
        {
            public RedisPfmTest pfm { get; set; }
            public Case3(RedisPfmTest pfm)
            {
                this.pfm = pfm;
            }
            public override string ToString()
            {
                return "Case3";
            }

            /// <summary>
            /// 10万一次
            /// </summary>
            /// <param name="round"></param>
            public void Test(int round)
            {
                Console.WriteLine("--{0} {1} task count:{2} --", ToString(), "10w", "1-10");
                var lineStr = "-".PadLeft(30, '-');
                Console.WriteLine(lineStr);

                var k = 1000;
                var total = 100*k;
                var threadCount = 50;

                while (threadCount>0)
                {
                    Console.WriteLine(">>task count:{0}", threadCount);
                    var tasks = new List<Task>();
                    var i = 0;
                    var begin = 0;
                    var end = 0;
                    var avgCount = total / threadCount;
                    var lastCount = total % threadCount;
                    while (i < threadCount)
                    {
                        end = begin + avgCount;
                        if (i == threadCount - 1)
                        {
                            end += lastCount;
                        }
                        var title = ToString() + "-" + threadCount;
                        var tBegin = begin;
                        var tEnd = end;
                        var task = Task.Factory.StartNew(() => this.RunOne(title, tBegin, tEnd));
                        tasks.Add(task);
                        begin = end;
                        i++;
                    }

                    var sw = new Stopwatch();
                    sw.Start();
                    Task.WaitAll(tasks.ToArray());
                    sw.Stop();
                    var msg = string.Format("total count:{0:N} ms:{1} avg:{2:f2} tps:{3:f2} tid:{4} isBack:{5}", total, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds * 1.0 / total, total * 1000.0 / sw.ElapsedMilliseconds, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsBackground);
                    Console.WriteLine(msg);

                    threadCount--;
                }

                Console.WriteLine();
            }

            public void RunOne(string title, int begin, int end)
            {
                //Console.WriteLine("RunOne {0} {1} {2}", title, begin, end);
                pfm.Run(title, begin, end, false);
            }
        }


        /// <summary>
        /// 多Thread
        /// </summary>
        class Case4 : ICase
        {
            public RedisPfmTest pfm { get; set; }
            public Case4(RedisPfmTest pfm)
            {
                this.pfm = pfm;
            }
            public override string ToString()
            {
                return "Case4";
            }

            /// <summary>
            /// 10万一次
            /// </summary>
            /// <param name="round"></param>
            public void Test(int round)
            {
                Console.WriteLine("--{0} {1} task count:{2} --", ToString(), "10w", "1-10");
                var lineStr = "-".PadLeft(30, '-');
                Console.WriteLine(lineStr);

                var k = 1000;
                var total = 100 * k;
                var threadCount = 20;

                while (threadCount > 0)
                {
                    Console.WriteLine(">>!ThreadPool.QueueUserWorkItem count:{0}", threadCount);
                    var tasks = new List<Thread>();
                    var mres = new List<ManualResetEvent>();
                    var i = 0;
                    var begin = 0;
                    var end = 0;
                    var avgCount = total / threadCount;
                    var lastCount = total % threadCount;
                    while (i < threadCount)
                    {
                        end = begin + avgCount;
                        if (i == threadCount - 1)
                        {
                            end += lastCount;
                        }
                        var title = ToString() + "-" + threadCount;
                        var tBegin = begin;
                        var tEnd = end;

                        var mre = new ManualResetEvent(false);
                        mres.Add(mre);
                        ThreadPool.QueueUserWorkItem(this.RunOne, new Tuple<ManualResetEvent, string, int, int>(mre, title, tBegin, tEnd));
                        //var task = new Thread(() => this.RunOne(title, tBegin, tEnd));
                        //var task = Task.Factory.StartNew(() => this.RunOne(title, tBegin, tEnd));
                        //tasks.Add(task);
                        begin = end;
                        i++;
                    }

                    var sw = new Stopwatch();
                    sw.Start();
                    WaitHandle.WaitAll(mres.ToArray());
                    //foreach (var task in tasks) task.Start();
                    //Task.WaitAll(tasks.ToArray());
                    sw.Stop();
                    var msg = string.Format("total count:{0:N} ms:{1} avg:{2:f2} tps:{3:f2} tid:{4} isBack:{5}", total, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds * 1.0 / total, total * 1000.0 / sw.ElapsedMilliseconds, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsBackground);
                    Console.WriteLine(msg);

                    threadCount--;
                }

                Console.WriteLine();
            }

            public void RunOne(object obj)
            {
                var tuple = obj as Tuple<ManualResetEvent, string, int, int>;
                RunOne(tuple.Item2, tuple.Item3, tuple.Item4);
                var mre = (ManualResetEvent)tuple.Item1;
                mre.Set();
            }
            public void RunOne(string title, int begin, int end)
            {
                //Console.WriteLine("RunOne {0} {1} {2}", title, begin, end);
                pfm.Run(title, begin, end);
            }
        }


        /// <summary>
        /// 10w个Task（测了一次最高只有8k左右）
        /// </summary>
        class Case3x : ICase
        {
            public RedisPfmTest pfm { get; set; }
            public Case3x(RedisPfmTest pfm)
            {
                this.pfm = pfm;
            }
            public override string ToString()
            {
                return "Case3x";
            }

            /// <summary>
            /// 10万一次
            /// </summary>
            /// <param name="round"></param>
            public void Test(int round)
            {
                Console.WriteLine("--{0} {1} task count:{2} --", ToString(), "10w", "1-10");
                var lineStr = "-".PadLeft(30, '-');
                Console.WriteLine(lineStr);

                var k = 1000;
                var total = 100 * k;
                var threadCount = 20;

                while (threadCount > 0)
                {
                    Console.WriteLine(">>task count:{0}", threadCount);
                    var tasks = new List<Task>();
                    var i = 0;
                    var begin = 0;
                    var end = 0;
                    var avgCount = total / threadCount;
                    var lastCount = total % threadCount;
                    while (i < threadCount)
                    {
                        end = begin + avgCount;
                        if (i == threadCount - 1)
                        {
                            end += lastCount;
                        }
                        var title = ToString() + "-" + threadCount;
                        var tBegin = begin;
                        var tEnd = end;

                        var aTasks = RunOne(title, tBegin, tEnd);
                        tasks.AddRange(aTasks);
                        //var task = Task.Factory.StartNew(() => this.RunOne(title, tBegin, tEnd));
                        //tasks.Add(task);
                        begin = end;
                        i++;
                    }

                    var sw = new Stopwatch();
                    sw.Start();
                    Task.WaitAll(tasks.ToArray());
                    sw.Stop();
                    var msg = string.Format("total count:{0:N} ms:{1} avg:{2:f2} tps:{3:f2} tid:{4} isBack:{5}", total, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds * 1.0 / total, total * 1000.0 / sw.ElapsedMilliseconds, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsBackground);
                    Console.WriteLine(msg);

                    threadCount--;
                }

                Console.WriteLine();
            }

            public List<Task> RunOne(string title, int begin, int end)
            {
                //Console.WriteLine("RunOne {0} {1} {2}", title, begin, end);
                return pfm.RunAsTask(title, begin, end);
            }
        }

    }
}
