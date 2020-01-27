using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        // Magical code that makes the service do wonderful things.
        protected async override void OnHandleIntent(Intent intent)
        {
            try
            {
                var smsMessageModel = JsonConvert.DeserializeObject<SmsMessageModel>(intent.GetStringExtra("smsMessageModel"));

                var insertedMessageStore = new SMSMessageStore
                {
                    UnixTimeStamp = App.Database.GetUnixTimeStamp(),
                    Processed = false,
                    SenderId = smsMessageModel.SenderId,
                    TextMessage = smsMessageModel.TextMessage,
                };
                await App.Database.DatabaseConnection.InsertAsync(insertedMessageStore);

                //save the created object again the processed one incase it it possible
                var messageData = await SMSProcessors.ProcessAndSaveAsync(insertedMessageStore);
                //if we get here thenmark the SMS as proceseed
                insertedMessageStore.Processed = true;
                await App.Database.DatabaseConnection.UpdateAsync(insertedMessageStore);

            }
            catch (Exception ex)
            {
                App.Database.LogException(ex, this.GetType().FullName);
            }
        }
    }

}