using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest.MyBase
{
    public class MyBaseClient
    {
        public static void Run()
        {
            ConfigNode configNode = new ConfigNodeImpl();
            configNode.StartTran(null);
        }
    }
}
