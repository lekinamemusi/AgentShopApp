using AgentShopApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.SMSProcessor
{
    public interface ISMSProcessor
    {
        bool Support(string senderId);
        SMSMessageStoreData Process(SMSMessageStore rawMessage);
        SMSMessageStoreData ProcessAndSave(SMSMessageStore rawMessage);
    }
}
