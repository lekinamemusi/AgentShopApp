using AgentShopApp.Data;
using AgentShopApp.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AgentShopApp.SMSProcessor
{
    public interface ISMSProcessor
    {
        bool Support(string senderId);
        Task<SMSMessageStoreData> ProcessAsync(SMSMessageStore rawMessage);
        Task<SMSMessageStoreData> ProcessAndSaveAsync(SMSMessageStore rawMessage);
    }
}
