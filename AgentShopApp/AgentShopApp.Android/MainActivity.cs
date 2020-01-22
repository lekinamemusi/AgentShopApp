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

namespace AgentShopApp.Droid
{
    [Activity(Label = "AgentShopApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        SmsReceiver smsReceiver;
        IntentFilter intentFilter;
        const int REQUEST_LOCATION = 123;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            requestAllPermission();

            //this below will register the receiver twice
            //smsReceiver = new SmsReceiver();
            //intentFilter = new IntentFilter(SmsReceiver.IntentAction);
            //intentFilter.Priority = 1000;
            //RegisterReceiver(smsReceiver, intentFilter);


            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        private void requestAllPermission()
        {
            string[] allPermission = new string[] 
            {
                Manifest.Permission.ReceiveSms,
                Manifest.Permission.ReadSms,
                Manifest.Permission.SendSms,
                Manifest.Permission.BroadcastSms,
                Manifest.Permission.WriteSms
            };
            initRequestPermission(allPermission);
        }

        private void initRequestPermission(string[] permissions)
        {
            ActivityCompat.RequestPermissions(this, permissions, REQUEST_LOCATION);
        }
    }
}