using System;
using System.Collections.Generic;
using System.Text;
using AgentShopApp.Data;

namespace AgentShopApp.SMSProcessor
{
    public class MPESAAgentSMSProcessor : ISMSProcessor
    {
        public SMSMessageStoreData Process(SMSMessageStore rawMessage)
        {
            var resultStoreData = new SMSMessageStoreData();



            return resultStoreData;
        }

        public SMSMessageStoreData ProcessAndSave(SMSMessageStore rawMessage)
        {
            var proccessedSMS = this.Process(rawMessage);
            DateTime foo = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            var taskCreate = App.Database.DatabaseConnection.InsertAsync(new SMSMessageStoreData
            {
                UnixTimeStamp = unixTime,
                SMSMessageStoreId = rawMessage.Id,
                TransactionId = proccessedSMS.TransactionId,
                TransactionTime = proccessedSMS.TransactionTime,
                AmountIn = proccessedSMS.AmountIn,
                AmountOut = proccessedSMS.AmountOut,
                ClientIdentifier = proccessedSMS.ClientIdentifier,
                TransactionSynced = false,
                TransactionType = proccessedSMS.TransactionType,
            });
            taskCreate.Wait();
            
            var taskWait = App.Database.DatabaseConnection.Table<SMSMessageStoreData>()
                .Where(r => r.Id == taskCreate.Result)
                .FirstOrDefaultAsync();
            taskWait.Wait();
            return taskWait.Result;
        }

        public bool Support(string senderId)
        {
            if (string.IsNullOrEmpty(senderId))
                return false;
            return senderId.ToLower() == "MPESA".ToLower();
        }
    }
}
