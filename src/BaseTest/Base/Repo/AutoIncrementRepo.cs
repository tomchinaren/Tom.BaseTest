using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest.Base.Repo
{
    public class AutoIncrementRepo
    {
        public static DbSet<AutoIncrement> DbSet
        {
            get { return MySqlDb.GetInstance().AutoIncrementSet; }
        }
        public long GetNewID(string collectionName, string fieldName = "ID", long addValue = 1, long def = 1)
        {
            var model = new AutoIncrement() { ID = 1, CollectionName = collectionName, FieldName = fieldName, IncrementID = addValue };
            //DbSet.Add(model);
            MySqlDb.GetInstance().Entry(model).State = EntityState.Modified;
            var res = MySqlDb.GetInstance().SaveChanges();
            return res;
        }
    }
}
