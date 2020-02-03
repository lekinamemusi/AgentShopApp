using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgentShopApp.Model;
using AgentShopApp.SMSProcessor;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace AgentShopApp.Droid.Service
{
    [Service(Name = "com.LeizamVentures.AgentShopApp.SmsReceiverJobIntentService", Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
    public class SmsReceiverJobIntentService : Android.Support.V4.App.JobIntentService
    {
        private static int MY_JOB_ID = 1099;

        protected async override void OnHandleWork(Intent intent)
        {
            try
            {
                var smsMessageModel = JsonConvert.DeserializeObject<SmsMessageModel>(intent.GetStringExtra("smsMessageModel"));

                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.O)
                {
                    var tostMessage = string.Format("S:{0} SDK lt 26", smsMessageModel.TransactionCode);
                    //Toast.MakeText(this, tostMessage, ToastLength.Long).Show();
                }
                else
                {
                    var tostMessage = string.Format("S:{0} SDK gte 27", smsMessageModel.TransactionCode);
                    //Toast.MakeText(this, tostMessage, ToastLength.Long).Show();
                }

                await SMSSaverRepository.SaveMessageToDb(smsMessageModel);
            }
            catch (Exception ex)
            {
                App.Database.LogException(ex, this.GetType().FullName);
            }
        }

        public static void EnqueueWork(Context context, Intent work)
        {
            var cls = Java.Lang.Class.FromType(typeof(SmsReceiverJobIntentService));
            try
            {
                EnqueueWork(context, cls, MY_JOB_ID, work);
            }
            catch (Exception ex)
            {
                App.Database.LogException(ex, "SmsReceieverJobIntentService.Enq");
            }
        }
    }
}
