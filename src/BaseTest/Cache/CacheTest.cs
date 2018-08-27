using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest
{
    public class CacheTest
    {
        public static void Test()
        {
            try
            {
                var t = new Table1() { ID = 1, Name = "tom" };
                var key = "pp:10003";
                Cache.Insert<Table1>(key, t);
                Console.WriteLine("end of insert");

                var val = Cache.Get(key);
                Console.WriteLine("end of get {0}", val);
            }
            catch(Exception ex)
            {
                Console.WriteLine("err {0}", ex.Message);
            }
        }
    }
}
