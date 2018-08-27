using btest.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest.Base
{

    public class MySqlDb : DbContext
    {
        private static MySqlDb _Repo;
        public static MySqlDb GetInstance() {
            if (_Repo == null)
            {
                _Repo = new MySqlDb("mysql");
            }
            return _Repo;
        }
        private MySqlDb(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            CreatModels();
        }

        private void CreatModels()
        {
            //var x= this.AutoIncrementSet.Count();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add<AutoIncrement>(new EntityTypeConfiguration<AutoIncrement>());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AutoIncrement> AutoIncrementSet { get; set; }
        public DbSet<Transaction> TransactionSet { get; set; }
        public DbSet<User> UserSet { get; set; }
        public DbSet<UpdateApply> UpdateApplySet { get; set; }
    }
}
