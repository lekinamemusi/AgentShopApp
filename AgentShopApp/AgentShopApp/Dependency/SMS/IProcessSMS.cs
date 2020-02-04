using AgentShopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgentShopApp.Dependency.SMS
{
    public interface IProcessSMS
    {
        IEnumerable<SmsMessageModel> GetAllSMS(SMSReaderFilterModel filterModel);

        void ScheduleJob(SMSReaderFilterModel smsReaderFilterModel);
    }
}