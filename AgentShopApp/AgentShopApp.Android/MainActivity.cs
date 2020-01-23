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

            //var allList = App.Database.DatabaseConnection.Table<SMSMessageStore>().ToListAsync();
            //allList.Wait();
            //var result = allList.Result;

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
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