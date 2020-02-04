using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Model
{
    public class SmsMessageModel
    {
        public string SenderId { get; set; }
        public string TextMessage { get; set; }

        public DateTime SentDate { get; set; }
        public string TransactionCode
        {
            get
            {
                if (string.IsNullOrEmpty(TextMessage))
                    return string.Empty;
                if (TextMessage.Contains(" "))
                {
                    int startIndex = 0, endIndexCurrent = TextMessage.IndexOf(' ');
                    //get the transaction code
                    var transactionCode = TextMessage.Substring(startIndex, endIndexCurrent).Trim();
                    return transactionCode;
                }
                else
                    return string.Empty;
            }
        }
    }
}
