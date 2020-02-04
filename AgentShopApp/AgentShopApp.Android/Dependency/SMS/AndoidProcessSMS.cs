using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentShopApp.Dependency.SMS;
using AgentShopApp.Droid.Service;
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
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AgentShopApp.Droid.Dependency.SMS.AndoidProcessSMS))]
namespace AgentShopApp.Droid.Dependency.SMS
{
    public class AndoidProcessSMS : IProcessSMS
    {
        Context context = Android.App.Application.Context;
        public AndoidProcessSMS()
        {

        }

        public IEnumerable<SmsMessageModel> GetAllSMS(SMSReaderFilterModel filterModel)
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
            foreach (var allowedSender in filterModel.SenderId)
            {
                whereClause = string.Format("{0} and {1} = ? ", whereClause, Telephony.Sms.Inbox.InterfaceConsts.Address);
                selectionArgs.Add(allowedSender);
            }
            //end address filter
            //from date filter
            var universalTimeFrom = filterModel.StartDate.Date;
            var fromDateUnixTime = App.Database.GetUnixTimeStampFromUniversalTimeOfset(universalTimeFrom).ToUnixTimeMilliseconds();
            whereClause = string.Format("{0} and {1} >= ? ", whereClause, Telephony.Sms.Inbox.InterfaceConsts.DateSent);
            selectionArgs.Add(fromDateUnixTime.ToString());
            //to date filter
            var universalTimeTo = filterModel.EndDate;
            var toDateUnixTime = App.Database.GetUnixTimeStampFromUniversalTimeOfset(universalTimeTo).ToUnixTimeMilliseconds();
            whereClause = string.Format("{0} and {1} < ? ", whereClause, Telephony.Sms.Inbox.InterfaceConsts.DateSent);
            selectionArgs.Add(toDateUnixTime.ToString());

            var sortOrder = string.Format("{0} desc", Telephony.Sms.Inbox.InterfaceConsts.DateSent);
            var cursor = context.ContentResolver.Query(Telephony.Sms.Inbox.ContentUri,
                projection,
                whereClause,
                selectionArgs.ToArray(),
               sortOrder);

            if (cursor != null
                && cursor.MoveToFirst())
            {
                do
                {
                    String id = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.Id));
                    String address = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.Address));
                    String body = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.Body));
                    String date = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.Date));
                    String dateSent = cursor.GetString(cursor.GetColumnIndexOrThrow(Telephony.Sms.Inbox.InterfaceConsts.DateSent));
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(dateSent));
                    result.Add(new SmsMessageModel
                    {
                        SenderId = address,
                        TextMessage = body,
                        SentDate = dateTimeOffset.UtcDateTime
                    });
                } while (cursor.MoveToNext());
            }
            return result;
        }

        public void ScheduleJob(SMSReaderFilterModel smsReaderFilterModel)
        {
            Intent smsMessageModelIntent = new Intent(context, typeof(SmsReaderJobIntentService));
            smsMessageModelIntent.PutExtra("smsReaderFilterModel", JsonConvert.SerializeObject(smsReaderFilterModel));
            SmsReaderJobIntentService.EnqueueWork(context, smsMessageModelIntent);
        }

        public async Task HandleWork(SMSReaderFilterModel smsReaderFilterModel)
        {
            var messageList = new Dependency.SMS.AndoidProcessSMS().GetAllSMS(smsReaderFilterModel);
            var totalList = messageList.Count();
            var counter = 1;
            //Toast.MakeText(context, string.Format("starting sync task size {0}", totalList), ToastLength.Long);
            foreach (var item in messageList)
            {
                try
                {
                    await SMSSaverRepository.SaveMessageToDb(item);
                    //if (context != null)
                    //    Toast.MakeText(context, string.Format("{0} of {1} completed", counter, totalList), ToastLength.Long);
                }
                catch (Exception ex)
                {
                    //if (context != null)
                    //    Toast.MakeText(context, string.Format("{0} of {1} failed", counter, totalList), ToastLength.Long);
                    App.Database.LogException(ex, this.GetType().FullName);
                }
                counter++;
            }
        }
    }
}