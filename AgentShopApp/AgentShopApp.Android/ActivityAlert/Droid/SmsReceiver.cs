﻿using System;
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
                        var fullMessage = smsMessage.DisplayMessageBody.ToString();
                        if (InterceptedSenders.Where(r => r.ToLower() == smsMessage.DisplayOriginatingAddress.ToLower())
                            .Any())
                        {
                            var messageSMs = string.Empty;
                            messageSMs = smsMessage.DisplayMessageBody.ToString();

                            Intent downloadIntent = new Intent(context, typeof(SmsReceieverService));
                            var messageModel = new SmsMessageModel
                            {
                                SenderId = smsMessage.DisplayOriginatingAddress,
                                TextMessage = messageSMs,
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