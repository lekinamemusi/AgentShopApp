using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace AgentShopApp.Data
{
    public class Database
    {

        public SQLiteAsyncConnection DatabaseConnection;

        public Database(string dbPath)
        {
            DatabaseConnection = new SQLiteAsyncConnection(dbPath);
            DatabaseConnection.CreateTableAsync<SMSMessageStore>().Wait();
            DatabaseConnection.CreateTableAsync<SMSMessageStoreData>().Wait();
            
        }


        //public Task<List<Person>> GetPeopleAsync()
        //{
        //    return _database.Table<Person>().ToListAsync();
        //}

        //public Task<int> SavePersonAsync(Person person)
        //{
        //    return _database.InsertAsync(person);
        //}
    }

}