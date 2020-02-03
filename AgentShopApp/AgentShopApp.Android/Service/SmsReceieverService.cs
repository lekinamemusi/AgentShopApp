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
    //this obsolete does not work in oreo and above devices
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.LeizamVentures.AgentShopApp.SmsReceieverServiceIntent" })]
    public class SmsReceieverService : IntentService
    {
        public SmsReceieverService()
            : base("SmsReceieverService")
        {

        }

        protected async override void OnHandleIntent(Intent intent)
        {
            try
            {
                var smsMessageModel = JsonConvert.DeserializeObject<SmsMessageModel>(intent.GetStringExtra("smsMessageModel"));
                await SMSSaverRepository.SaveMessageToDb(smsMessageModel);
            }
            catch (Exception ex)
            {
                App.Database.LogException(ex, this.GetType().FullName);
            }
        }
    }

}