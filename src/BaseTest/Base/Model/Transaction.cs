using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest.Base
{
    public class Transaction
    {
        public long ID { get; set; }
        public long SellerID { get; set; }
        public long BuyerID { get; set; }
        public double Amount { get; set; }
    }


}
