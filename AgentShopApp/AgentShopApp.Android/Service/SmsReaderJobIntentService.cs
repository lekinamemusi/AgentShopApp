using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var messageList = getAllSMS(this, filterModel.SenderId, filterModel.StartDate, filterModel.EndDate);
                foreach (var item in messageList)
                    await SMSSaverRepository.SaveMessageToDb(item);
            }
            catch (Exception ex)
            {
                App.Database.LogException(ex, this.GetType().FullName);
            }
        }

        public IEnumerable<SmsMessageModel> getAllSMS(Context context,
            IEnumerable<string> addressFilter,
            DateTime fromDateFilter,
            DateTime toDateFilter)
        {

            var result = new List<SmsMessageModel>();
            string[] projection = new string[] {
                Telephony.Sms.Inbox.InterfaceConsts.Id,
                Telephony.Sms.Inbox.InterfaceConsts.Address,
                Telephony.Sms.Inbox.InterfaceConsts.Body,
                Telephony.Sms.Inbox.InterfaceConsts.Date,
                Telephony.Sms.Inbox.InterfaceConsts.DateSent
            };
            List<String> selectionArgs = new List<string>();
            //address filter
            string whereClause = " 1 = 1 ";
            foreach (var allowedSender in addressFilter)
            {
                whereClause = string.Format("{0} and {1} = ? ", whereClause, Telephony.Sms.Inbox.InterfaceConsts.Address);
                selectionArgs.Add(allowedSender);
            }
            //end address filter
            //from date filter
            var universalTimeFrom = fromDateFilter.Date.ToUniversalTime();
            var fromDateUnixTime = App.Database.GetUnixTimeStampFromUniversalTime(universalTimeFrom);
            whereClause = string.Format("{0} and {1} >= ? ", whereClause, Telephony.Sms.Inbox.InterfaceConsts.DateSent);
            selectionArgs.Add(fromDateUnixTime.ToString());
            //to date filter
            //var endDateDate = toDateFilter.Date;
            var universalTimeTo = toDateFilter.Date.ToUniversalTime();
            var toDateUnixTime = App.Database.GetUnixTimeStampFromUniversalTime(universalTimeTo);
            //whereClause = string.Format("{0} and {1} < ? ", whereClause, Telephony.Sms.Inbox.InterfaceConsts.DateSent);
            //selectionArgs.Add(toDateUnixTime.ToString());

            var sortOrder = string.Format("{0} desc", Telephony.Sms.Inbox.InterfaceConsts.DateSent);
            var cursor = context.ContentResolver.Query(Telephony.Sms.Inbox.ContentUri,
                projection,
                whereClause,
                selectionArgs.ToArray(),
               sortOrder);

            if (cursor != null && cursor.MoveToFirst())
            {
                do
                {
                    String id = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.Id));
                    String address = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.Address));
                    String body = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.Body));
                    String date = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.Date));
                    String dateSent = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.DateSent));

                    result.Add(new SmsMessageModel
                    {
                        SenderId = address,
                        TextMessage = body,
                    });
                } while (cursor.MoveToNext());
            }
            return result;
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