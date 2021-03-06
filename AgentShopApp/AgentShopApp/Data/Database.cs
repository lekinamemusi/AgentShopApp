﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentShopApp.Data.Model;
using SQLite;

namespace AgentShopApp.Data
{
    public class Database
    {
        public SQLiteAsyncConnection DatabaseConnection;

        public Database(string dbPath)
        {
            DatabaseConnection = new SQLiteAsyncConnection(dbPath);
            SetUpDataBase();
        }

        public void SetUpDataBase()
        {
            
            /*await*/
            DatabaseConnection.CreateTableAsync<SMSMessageStore>().Wait();
            /*await*/
            DatabaseConnection.CreateTableAsync<SMSMessageStoreData>().Wait();
            /*await*/
            DatabaseConnection.CreateTableAsync<ClientData>().Wait();
            /*await*/
            DatabaseConnection.CreateTableAsync<TransactionType>().Wait();
            /*await*/
            DatabaseConnection.CreateTableAsync<GeneralLedger>().Wait();
            /*await*/
            DatabaseConnection.CreateTableAsync<GlMasterAccount>().Wait();
            /*await*/
            DatabaseConnection.CreateTableAsync<GlAccount>().Wait();
            /*await*/
            DatabaseConnection.CreateTableAsync<ErrorLogs>().Wait();
            /*await*/
            DatabaseConnection.CreateTableAsync<DateSyncData>().Wait();

            DatabaseConnection.CreateTableAsync<FailedSyncSMS>().Wait();

            //ClearDb();
        }


        public void ClearDb()
        {
            //throw new NotImplementedException("");
            /*await*/
            DatabaseConnection.Table<SMSMessageStore>().DeleteAsync(r => true).Wait();
            /*await*/
            DatabaseConnection.Table<SMSMessageStoreData>().DeleteAsync(r => true).Wait();
            /*await*/
            DatabaseConnection.Table<ClientData>().DeleteAsync(r => true).Wait();
            /*await*/
            DatabaseConnection.Table<TransactionType>().DeleteAsync(r => true).Wait();
            /*await*/
            DatabaseConnection.Table<GeneralLedger>().DeleteAsync(r => true).Wait();
            /*await*/
            DatabaseConnection.Table<GlMasterAccount>().DeleteAsync(r => true).Wait();
            /*await*/
            DatabaseConnection.Table<GlAccount>().DeleteAsync(r => true).Wait();
            /*await*/
            DatabaseConnection.Table<ErrorLogs>().DeleteAsync(r => true).Wait();

            DatabaseConnection.Table<FailedSyncSMS>().DeleteAsync(r => true).Wait();

            
            //await SetUpDefaultAccountsState().DeleteAsync(r => true).Wait();
        }

        public async Task<int> LogException(Exception ex, string moduleName)
        {
            return await this.DatabaseConnection.InsertAsync(new ErrorLogs
            {
                ErrorMessage = ex.Message,
                FullException = ex.ToString(),
                Module = moduleName,
                LogTime = DateTime.Now,
                UnixTimeStamp = this.GetUnixTimeStamp()
            });
        }

        //private async Task SetUpDefaultAccountsState()
        //{
        //    //set up float, expenses, income, equity, liabilites
        //    throw new NotImplementedException();
        //}
        public DateTimeOffset GetUnixTimeStampFromUniversalTimeOfset(DateTime universalTime)
        {
            var unixTime = ((DateTimeOffset)universalTime);
            return unixTime;
        }
        public long GetUnixTimeStampFromUniversalTime(DateTime universalTime)
        {
            long unixTime = ((DateTimeOffset)universalTime).ToUnixTimeSeconds();
            return unixTime;
        }
        public long GetUnixTimeStamp(DateTime dateTime)
        {
            DateTime universalTime = dateTime.ToUniversalTime();
            return GetUnixTimeStampFromUniversalTime(universalTime);
        }
        public long GetUnixTimeStamp()
        {
            return GetUnixTimeStamp(DateTime.Now);
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