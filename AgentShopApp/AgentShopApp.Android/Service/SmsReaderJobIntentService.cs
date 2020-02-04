using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentShopApp.Droid.Dependency.SMS;
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
    [Service(Name = "com.LeizamVentures.AgentShopApp.SmsReaderJobIntentService", Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
    public class SmsReaderJobIntentService : Android.Support.V4.App.JobIntentService
    {
        private static int MY_JOB_ID = 1098;

        protected async override void OnHandleWork(Intent intent)
        {
            try
            {
                var filterModel = JsonConvert.DeserializeObject<SMSReaderFilterModel>(intent.GetStringExtra("smsReaderFilterModel"));
                var andrdprc = new AndoidProcessSMS();
                await andrdprc.HandleWork(filterModel);
            }
            catch (Exception ex)
            {
                App.Database.LogException(ex, this.GetType().FullName);
            }
        }


        public static void EnqueueWork(Context context, Intent work)
        {
            var cls = Java.Lang.Class.FromType(typeof(SmsReaderJobIntentService));
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