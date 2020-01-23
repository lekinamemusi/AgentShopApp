using AgentShopApp.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgentShopApp.SMSProcessor
{
    public static class SMSProcessors
    {
        public static List<ISMSProcessor> Processors = new List<ISMSProcessor>
        {
            new MPESAAgentSMSProcessor(),
        };

        public static SMSMessageStoreData Process(SMSMessageStore rawMessage)
        {
            if (rawMessage == null)
                throw new ArgumentNullException("rawMessage");

            SMSMessageStoreData result = null;
            var currentProcessor = Processors.FirstOrDefault(r => r.Support(rawMessage.SenderId));
            if (currentProcessor != null)
                result = currentProcessor.Process(rawMessage);
            return result;
        }

        public static SMSMessageStoreData ProcessAndSave(SMSMessageStore rawMessage)
        {
            if (rawMessage == null)
                throw new ArgumentNullException("rawMessage");

            SMSMessageStoreData result = null;
            var currentProcessor = Processors.FirstOrDefault(r => r.Support(rawMessage.SenderId));
            if (currentProcessor != null)
                result = currentProcessor.ProcessAndSave(rawMessage);
            return result;
        }
    }
}
