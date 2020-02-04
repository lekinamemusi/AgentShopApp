using AgentShopApp.Model;
using AgentShopApp.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AgentShopApp.SMSProcessor
{
    public class SMSSaverRepository
    {
        public static string[] InterceptedSenders { get => new string[] { "MPESA" }; }

        public static async Task SaveMessageToDb(SmsMessageModel smsMessageModel)
        {
            //proccess the message to get transaction id
            //each should be saved against the transaction id to make syncing easy
            var stringManipulate = smsMessageModel.TextMessage;
            int startIndex = 0, endIndexCurrent = stringManipulate.IndexOf(' ');
            //get the transaction code
            var transactionCode = stringManipulate.Substring(startIndex, endIndexCurrent).Trim();
            var insertedMessageStore = App.Database.DatabaseConnection.Table<SMSMessageStore>()
                .FirstOrDefaultAsync(r => r.TransactionID == transactionCode);
            SMSMessageStore SMSMessageStore = insertedMessageStore.Result;
            if (SMSMessageStore == null)
            {
                SMSMessageStore = new SMSMessageStore
                {
                    TransactionID = transactionCode,
                    UnixTimeStamp = App.Database.GetUnixTimeStamp(),
                    Processed = false,
                    SenderId = smsMessageModel.SenderId,
                    TextMessage = smsMessageModel.TextMessage,
                };
                await App.Database.DatabaseConnection.InsertAsync(SMSMessageStore);
            }

            //save the created object again the processed one incase it it possible
            var messageData = await SMSProcessors.ProcessAndSaveAsync(SMSMessageStore);
            //if we get here thenmark the SMS as proceseed
            SMSMessageStore.Processed = true;
            await App.Database.DatabaseConnection.UpdateAsync(SMSMessageStore);
        }

    }
}
