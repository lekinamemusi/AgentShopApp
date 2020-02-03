using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentShopApp.Data;
using AgentShopApp.Data.Model;
using AgentShopApp.Droid.Service;
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
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = Int32.MaxValue)]
    class SmsReceiver : BroadcastReceiver
    {
        public static string[] InterceptedSenders = new string[] { "MPESA" };

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                string.Format("MPA:");
                if (InterceptedSenders == null)
                    InterceptedSenders = new string[] { };//to prevent null exceptions here

                if (intent.Action.Equals(Telephony.Sms.Intents.SmsReceivedAction))
                {
                    var smsMessages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
                    var combinedMessage = smsMessages
                        .Where(r => InterceptedSenders.Where(r2 => r2.ToLower() == r.DisplayOriginatingAddress.ToLower())
                        .Any())
                        .Select(r => r.DisplayMessageBody.ToString())
                        .Aggregate((s1, s2) => string.Format("{0}{1}", s1, s2));

                    if (combinedMessage.Length > 0)
                    {
                        var messageModel = new SmsMessageModel
                        {
                            SenderId = smsMessages.FirstOrDefault().DisplayOriginatingAddress,
                            TextMessage = combinedMessage,
                        };
                        var tostMessage = string.Format("MPA:{0}", messageModel.TransactionCode);
                        //Toast.MakeText(context, tostMessage, ToastLength.Long).Show();
                        //then process this message
                        Intent smsMessageModelIntent = new Intent(context, typeof(SmsReceiverJobIntentService));
                        smsMessageModelIntent.PutExtra("smsMessageModel", JsonConvert.SerializeObject(messageModel));
                        SmsReceiverJobIntentService.EnqueueWork(context, smsMessageModelIntent);
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
