using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest
{

    /// <summary>
    /// 看做DB （for mssql、mysql and so on）
    /// </summary>
    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class EFDbContext : DbContext
    {
        /// <summary>
        /// nameOrConnectionString：连接字符串配置名（在App.config或web.config等文件中）或连接字符串
        /// 可以是mssql、mysql等。
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public EFDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }
        public EFDbContext():base("mysql")
        {
        }

        /// <summary>
        /// DB中的表
        /// </summary>
        public DbSet<Table1> Table1s { get; set; }
        public DbSet<Table2> Table2s { get; set; }
        public DbSet<Table3> Table3s { get; set; }
        //public DbSet<Table4> Table4s { get; set; }
    }

    /// <summary>
    /// 表对应实体
    /// </summary>
    public class Table1
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Table2
    {
        public int ID { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }

    public class Table3
    {
        public int ID { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
    public class Table4
    {
        public int ID { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }

    public class EFTest
    {
        public static void TestMsSql()
        {
            Test();
        }
        public static void TestMySql()
        {
            Test("mysql");
        }

        public static void Test(string connectName="mssql")
        {
            //Database.SetInitializer<EFDbContext>(null); 
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EFDbContext>());
            var context = new EFDbContext(connectName);
            var count = context.Table1s.Count();
            Console.WriteLine(count);

            Random rnd = new Random(100);
            var id = rnd.Next()%1000;

            context.Table1s.Add(new Table1() { ID = id, Name = "jerry" });
            context.Table2s.Add(new Table2() { ID = id, Name = "goodboy" });
            context.SaveChanges();

            count = context.Table1s.Count();
            Console.WriteLine(count);
        }
    }
}
