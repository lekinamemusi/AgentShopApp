using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using AgentShopApp.Droid.ActivityAlert.Droid;
using Android.Support.V4.App;
using Android;
using Android.Support.V4.Content;
using System.Collections.Generic;
using AgentShopApp.Data;
using AgentShopApp.Data.Model;
using System.Threading.Tasks;

namespace AgentShopApp.Droid
{
    [Activity(Label = "AgentShopApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int REQUEST_LOCATION = 123;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            requestAllPermission();

            //Intent downloadIntent = new Intent(this, typeof(Services.SmsReceieverService));
            //var messageModel = new Model.SmsMessageModel
            //{
            //    SenderId = "MPESA",
            //    //TextMessage = "OAO0KWM4LQ confirmed. You bought Ksh50.00 of airtime for 254712106254 on 24/1/20 at 9:09 AM.New  balance is Ksh27,147.00.Use current M-PESA PIN to activate M-PESA if you change sim",
            //    TextMessage = "OAO4KVPDQ4 Confirmed. On 24/1/20 at 8:26 AM Take Ksh1,100.00 cash from PASCALENE W MAINA Your M-PESA float balance is Ksh27,197.00.",
            //    //TextMessage = "OAN3KNQZC7 Confirmed. on 23/1/20 at 7:51 PM Give Ksh1,530.00 to JAPHET MAINA MBOGO 0712106254. New M-PESA float balance is Ksh28,297.00"
            //};
            //downloadIntent.PutExtra("smsMessageModel", Newtonsoft.Json.JsonConvert.SerializeObject(messageModel));
            //StartService(downloadIntent);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        private async Task testAsyncTasks()
        {
            try
            {

                var result = new SMSProcessor.MPESAAgentSMSProcessor();
                var asynResultAt = await result.ProcessAsync(new SMSMessageStore
                {
                    TextMessage = "OAO0KWM4LQ confirmed. You bought Ksh50.00 of airtime for 254712106254 on 24/1/20 at 9:09 AM.New  balance is Ksh27, 147.00.Use current M-PESA PIN to activate M-PESA if you change sim"
                });
                var asynResultDp = await result.ProcessAsync(new SMSMessageStore
                {
                    TextMessage = "OAO4KVPDQ4 Confirmed. On 24/1/20 at 8:26 AM Take Ksh1,100.00 cash from PASCALENE W MAINA Your M-PESA float balance is Ksh27,197.00.",
                });
                var asynResultW = await result.ProcessAsync(new SMSMessageStore
                {
                    TextMessage = "OAN3KNQZC7 Confirmed. on 23/1/20 at 7:51 PM Give Ksh1,530.00 to JAPHET MAINA MBOGO 0712106254. New M-PESA float balance is Ksh28,297.00"
                });
                //asynResult.
            }
            catch (Exception ex)
            {
                int y = 0;
            }
        }

        private void requestAllPermission()
        {
            //https://google-developer-training.github.io/android-developer-phone-sms-course/Lesson%202/2_p_2_sending_sms_messages.html
            string[] allPermission = new string[]
            {
                Manifest.Permission.ReceiveSms,
                //Manifest.Permission.ReadExternalStorage,
                //Manifest.Permission.WriteExternalStorage,
                //Manifest.Permission.ReadSms,
                //Manifest.Permission.SendSms,
                //Manifest.Permission.BroadcastSms,
                //Manifest.Permission.WriteSms
            };
            initRequestPermission(allPermission);
        }

        private void initRequestPermission(string[] permissions)
        {
            var listOfRequiredPerms = new List<string>();
            foreach (var permission in permissions)
            {
                if ((ActivityCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted) == false)
                    listOfRequiredPerms.Add(permission);
            }
            if (listOfRequiredPerms.Count > 0)
            {
                string[] permiList = listOfRequiredPerms.ToArray();
                ActivityCompat.RequestPermissions(this, permiList, REQUEST_LOCATION);
            }
        }
    }
}