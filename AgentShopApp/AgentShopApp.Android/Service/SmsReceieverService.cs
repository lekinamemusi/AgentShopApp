using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentShopApp.Data.Model;
using AgentShopApp.Model;
using AgentShopApp.SMSProcessor;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace AgentShopApp.Droid.Services
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.LeizamVentures.AgentShopApp.SmsReceieverServiceIntent" })]
    public class SmsReceieverService : IntentService
    {
        public SmsReceieverService()
            : base("SmsReceieverService")
        {

        }
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
        // Magical code that makes the service do wonderful things.
        protected async override void OnHandleIntent(Intent intent)
        {
            try
            {
                var smsMessageModel = JsonConvert.DeserializeObject<SmsMessageModel>(intent.GetStringExtra("smsMessageModel"));
                await SaveMessageToDb(smsMessageModel);
            }
            catch (Exception ex)
            {
                App.Database.LogException(ex, this.GetType().FullName);
            }
        }
    }

}