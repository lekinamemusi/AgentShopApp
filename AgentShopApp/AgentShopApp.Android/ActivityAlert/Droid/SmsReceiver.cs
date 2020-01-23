using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgentShopApp.Data;
using AgentShopApp.SMSProcessor;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AgentShopApp.Droid.ActivityAlert.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
    class SmsReceiver : BroadcastReceiver
    {
        private const string TAG = "AA:SmsReceiver";
        public static string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

        public static string[] InterceptedSenders = new string[] { "MPESA" };
        public override void OnReceive(Context context, Intent intent)
        {
            if (InterceptedSenders == null)
                InterceptedSenders = new string[] { };//to prevent null exceptions here

            if (intent.Action.Equals(Telephony.Sms.Intents.SmsReceivedAction))
            {
                var smsMessages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
                foreach (var smsMessage in smsMessages)
                {
                    //incase the sender is registered
                    if (InterceptedSenders.Where(r => r.ToLower() == smsMessage.DisplayOriginatingAddress.ToLower())
                        .Any())
                    {
                        //to prevent duplicates within the same message remove
                        //var displayValue = string.Format(", Sender {0}, Msg: {1}", smsMessage.DisplayOriginatingAddress, smsMessage.MessageBody);
                        //Toast.MakeText(context, displayValue, ToastLength.Long).Show();

                        DateTime foo = DateTime.UtcNow;
                        long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
                        var taskCreate = App.Database.DatabaseConnection.InsertAsync(new SMSMessageStore
                        {
                            UnixTimeStamp = unixTime,
                            Processed = false,
                            SenderId = smsMessage.DisplayOriginatingAddress,
                            TextMessage = smsMessage.MessageBody,
                            
                        });
                        taskCreate.Wait();
                       
                        //load the created object
                        var taskRead = App.Database.DatabaseConnection.Table<SMSMessageStore>()
                            .Where(r => r.Id == taskCreate.Result)
                            .FirstOrDefaultAsync();
                        taskRead.Wait();
                        //save the created object again the processed one incase it it possible
                        SMSProcessors.ProcessAndSave(taskRead.Result);
                    }


                    //var allList = App.Database.DatabaseConnection.Table<SMSMessageStore>().ToListAsync();
                    //allList.Wait();
                    //var result = allList.Result;
                    //App.Database.DatabaseConnection.cre
                    //Log.Debug(TAG, $"MessageBody {msg.MessageBody}");
                    //Log.Debug(TAG, $"DisplayOriginatingAddress {msg.DisplayOriginatingAddress}");
                    //Log.Debug(TAG, $"OriginatingAddress {msg.OriginatingAddress}");


                }

            }

        }

    }
}