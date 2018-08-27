using btest.Base;
using btest.Base.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest.Base_eBay
{
    public class BaseDemo
    {
        public static void Run()
        {
            try {
                var repo = new AutoIncrementRepo();
                //var x = AutoIncrementRepo.DbSet;
                var id = repo.GetNewID("User");
                Console.WriteLine(id);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine(ex.InnerException.InnerException.Message);
                    }
                }
            }
        }
    }
}
