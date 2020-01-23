using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace AgentShopApp.Data
{
    public class SMSMessageStoreData
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public long UnixTimeStamp { get; set; }
        [Indexed(Unique = true)]
        public long SMSMessageStoreId { get; set; }
        [Indexed(Unique = true)]
        public string TransactionId { get; set; }
        public DateTime TransactionTime { get; set; }
        public double AmountIn { get; set; }
        public double AmountOut { get; set; }
        [Indexed(Unique = false)]
        public string ClientIdentifier { get; set; }
        public bool TransactionSynced { get; set; }
        //1 Normal Customer Deposit, 2 Normal Customer withdraw, 3 Loan Customer Deposit, 4 Loan Payment Customer Withdraw
        public int TransactionType { get; set; }

    }
}