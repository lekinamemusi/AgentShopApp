using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentShopApp.Data;
using AgentShopApp.Data.Model;
using AgentShopApp.Droid.Services;
using AgentShopApp.Model;
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
using Newtonsoft.Json;

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
            try
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
                            
                            Intent downloadIntent = new Intent(context, typeof(SmsReceieverService));
                            var messageModel = new SmsMessageModel
                            {
                                SenderId = smsMessage.DisplayOriginatingAddress,
                                TextMessage = smsMessage.MessageBody,
                                //TextMessage = "OAO4KVPDQ4 Confirmed. On 24/1/20 at 8:26 AM Take Ksh1,100.00 cash from PASCALENE W MAINA Your M-PESA float balance is Ksh27,197.00.",
                            };
                            downloadIntent.PutExtra("smsMessageModel", JsonConvert.SerializeObject(messageModel));

                            context.StartService(downloadIntent);

                        }                     
                    }
                }
            }
            catch (Exception ex)
            {
                App.Database.LogException(ex, this.GetType().FullName);
            }

        }

    }
}