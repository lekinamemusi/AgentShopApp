using AgentShopApp.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AgentShopApp.Data.Model;
using System.Threading.Tasks;

namespace AgentShopApp.SMSProcessor
{
    public static class SMSProcessors
    {
        public static List<ISMSProcessor> Processors = new List<ISMSProcessor>
        {
            new MPESAAgentSMSProcessor(),
        };

        public static async Task<SMSMessageStoreData> ProcessAsync(SMSMessageStore rawMessage)
        {
            if (rawMessage == null)
                throw new ArgumentNullException("rawMessage");

            SMSMessageStoreData result = null;
            var currentProcessor = Processors.FirstOrDefault(r => r.Support(rawMessage.SenderId));
            if (currentProcessor != null)
                result = await currentProcessor.ProcessAsync(rawMessage);
            return result;
        }

        public static async Task<SMSMessageStoreData> ProcessAndSaveAsync(SMSMessageStore rawMessage)
        {
            if (rawMessage == null)
                throw new ArgumentNullException("rawMessage");

            SMSMessageStoreData result = null;
            var currentProcessor = Processors.FirstOrDefault(r => r.Support(rawMessage.SenderId));
            if (currentProcessor != null)
                result = await currentProcessor.ProcessAndSaveAsync(rawMessage);
            return result;
        }
    }
}
