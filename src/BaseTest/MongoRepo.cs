using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest
{

    public class MongoDb
    {
        private IMongoDatabase _db;

        public MongoDb(string connectionString)
        {
            _db = GetDataBase(connectionString);
        }
        public MongoDb(string host, int port)
        {
            _db = GetDataBase(host,port);
        }

        /// <summary>  
        /// 连接超时设置 秒  
        /// </summary>  
        private readonly int CONNECT_TIME_OUT = 10;

        /// <summary>  
        /// 数据库的名称  
        /// </summary>  
        private readonly string DB_NAME = "test";

        private IMongoDatabase GetDataBase(string connectionString)
        {
            MongoClient client = new MongoClient(connectionString);
            return client.GetDatabase(DB_NAME);
        }
        /// <summary>  
        /// 得到数据库实例。port一般是27017
        /// </summary>  
        private IMongoDatabase GetDataBase(string host, int port)
        {
            MongoClientSettings mongoSetting = new MongoClientSettings();
            //设置连接超时时间  
            mongoSetting.ConnectTimeout = new TimeSpan(CONNECT_TIME_OUT * TimeSpan.TicksPerSecond);
            //设置数据库服务器  
            mongoSetting.Server = new MongoServerAddress(host, port);
            //创建Mongo的客户端  
            MongoClient client = new MongoClient(mongoSetting);
            //得到服务器端并且生成数据库实例  
            return client.GetDatabase(DB_NAME);
        }

        public Task<bool> Insert<T>(T t)
        {
            //集合名称  
            string collectionName = typeof(T).Name;
            return Insert<T>(t, collectionName);
        }

        private async Task<bool> Insert<T>(T t, string collectionName)
        {
            if (this._db == null)
            {
                return false;
            }
            try
            {
                IMongoCollection<BsonDocument> mc = this._db.GetCollection<BsonDocument>(collectionName);
                BsonDocument bd = t.ToBsonDocument();
                await mc.InsertOneAsync(bd);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Update(Table1 t)
        {
            if (this._db == null)
            {
                return false;
            }
            try
            {
                IMongoCollection<Table1> mc = this._db.GetCollection<Table1>("Table1");
                //BsonDocument bd = t.ToBsonDocument();
                //bd.Remove("ID");
                //UpdateDefinition<Table1> update = new BsonDocumentUpdateDefinition<Table1>(bd);
                var update = Builders<Table1>.Update.Set("Name", t.Name);
                var m =  await mc.UpdateOneAsync(x => x.ID == t.ID, update);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class MongoDemo
    {
        public static async Task Test()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mongodb"].ConnectionString;
            var db = new MongoDb(connectionString);
            var t1 = new Table1() { ID = 9, Name = "999" };
            await db.Insert<Table1>(t1);

            var db2 = new MongoDb("localhost",27018);
            var t2 = new Table1() { ID = 8, Name = "888" };
            await db2.Insert<Table1>(t2);

        }

        public static async Task Test2()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mongodb"].ConnectionString;
            var db = new MongoDb(connectionString);
            var t1 = new Table1() { ID = 8, Name = "8527y" };
            await db.Update(t1);
        }


    }
}
