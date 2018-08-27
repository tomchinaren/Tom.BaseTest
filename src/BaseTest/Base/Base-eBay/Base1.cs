using btest.Base.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest.Base.Base_eBay
{
    public class Base1
    {
        public static void Run()
        {
            try
            {
                //add tran
                var tranSet = MySqlDb.GetInstance().TransactionSet;
                var tran = new Transaction() { BuyerID = 1, SellerID = 2, Amount = 100 };
                tranSet.Add(tran);
                var msg1 = new UserUpdateDto() { Type = 1, UserID = tran.SellerID, Amount = tran.Amount };
                var msg2 = new UserUpdateDto() { Type = 2, UserID = tran.BuyerID, Amount = tran.Amount };
                Rbmq.PublishMsg(obj: msg1);
                Rbmq.PublishMsg(obj: msg2);

                MySqlDb.GetInstance().SaveChanges();
            }
            catch(Exception ex)
            {
                Console.WriteLine("");
            }
        }
    }
}
