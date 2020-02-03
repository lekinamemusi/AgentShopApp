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

            testSendAllMessageTypesAsync();
            //var allMessages = App.Database.DatabaseConnection.Table<SMSMessageStore>()
            //    .OrderByDescending(r => r.Id)
            //    .ToListAsync();
            //allMessages.Wait();
            //var allMessagesL = allMessages.Result;

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        private void testSendAllMessageTypesAsync()
        {
            try
            {
                var listMessages = new List<String>();
                #region load the diffrent types of supported messages
                //customer deposit
                //listMessages.Add("OAO4KVPDQ4 Confirmed. On 24/1/20 at 8:26 AM Take Ksh1,100.00 cash from PASCALENE W MAINA Your M-PESA float balance is Ksh27,197.00.");
                //////customer withdraw
                //listMessages.Add("OAN3KNQZC7 Confirmed. on 23/1/20 at 7:51 PM Give Ksh1,530.00 to JAPHET MAINA MBOGO 0712106254. New M-PESA float balance is Ksh28,297.00");
                //////withdraw from non registered line
                //listMessages.Add("OA314OGB3N  Confirmed.On 3/1/20 at 11:18 AM.Give Ksh5,100.00 cash to +254734373935.New M-PESA float balance is Ksh23,103.00");
                ////customer buy airtime
                //listMessages.Add("OAO0KWM4LQ confirmed. You bought Ksh50.00 of airtime for 254712106254 on 24/1/20 at 9:09 AM.New  balance is Ksh27,147.00.Use current M-PESA PIN to activate M-PESA if you change sim");
                //////commission
                //listMessages.Add("OA1831LCUQ confirmed. Commission of Ksh17,807.19 paid to your Working account. Working Balance is Ksh16,968.03 on 1/1/20 at 2:10 AM");
                //////selling float to agent
                //listMessages.Add("NLV92KJXU5 Business Deposit Confirmed on 31/12/19 at 3:43 PM. Take Ksh20,100.00 cash from 105104 - Jays Call Mobile Accra Trading Centre Ground Floor Nairobi. Your Float balance is Ksh51,617.00");
                //////but float from agent
                //listMessages.Add("NLO1WBXPWD Business Deposit Confirmed on 24/12/19 at 3:59 PM. Give Ksh130,000.00 cash to 147101 - Ropem Telcom Bethsaida Ziwani Kariokor. New Working balance is Ksh147,550.69");
                //////float to working
                //listMessages.Add("OA2545YOU3 Confirmed. Ksh16,000.00  transferred from Float Ac.to Working Ac..New Working Ac. Balance is Ksh32,968.03.New Float Balance is Ksh50,003.00 on 2/1/20 at 4:26 PM");
                //////working to float
                //listMessages.Add("OA354XX90L Confirmed. Ksh500.00   transferred from Working to Float.New Working Balance is Ksh468.03.New Float Balance is Ksh1,084.00 on 3/1/20 at 4:58 PM.");
                //////transaction reversal debit float links to a withdraw
                //listMessages.Add("OA516AJI01 confirmed. Reversal of transaction OAN3KNQZC7 has been successfully reversed on 5/1/20 at 1:14 PM and Ksh1,530.00 is debited from your M-PESA account. Your new float account balance is Ksh39,516.00");
                //////transaction reversal credit float links to a deposit
                //listMessages.Add("OA516AJI99 confirmed. Reversal of transaction OAO4KVPDQ4 has been successfully reversed on 5/1/20 at 1:14 PM and Ksh1,100.00 is credited from your M-PESA account. Your new float account balance is Ksh39,516.00");
                #endregion
                foreach (var item in listMessages)
                {
                    Intent downloadIntent = new Intent(this, typeof(Services.SmsReceieverService));
                    var messageModel = new Model.SmsMessageModel
                    {
                        SenderId = "MPESA",
                        TextMessage = item,
                    };
                    downloadIntent.PutExtra("smsMessageModel", Newtonsoft.Json.JsonConvert.SerializeObject(messageModel));
                    StartService(downloadIntent);
                }


            }
            catch (Exception)
            {
                int y = 0;

            }
        }

        private void requestAllPermission()
        {
            //https://google-developer-training.github.io/android-developer-phone-sms-course/Lesson%202/2_p_2_sending_sms_messages.html
            string[] allPermission = new string[]
            {
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.WriteExternalStorage,
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