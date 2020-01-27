﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace AgentShopApp.Data.Model
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
        [Indexed(Unique = false)]
        public DateTime TransactionTime { get; set; }
        public double Amount { get; set; }
        [Indexed(Unique = false)]
        public int ClientDataId { get; set; }
        [Indexed(Unique = false)]
        public bool TransactionSynced { get; set; }
        [Indexed(Unique = false)]
        public int TransactionTypeId { get; set; }
        
        public string Narration { get; set; }


        [Ignore]
        public ClientData ClientData { get; set; }
        [Ignore]
        public TransactionType TransactionType { get; set; }
        [Ignore]
        public SMSMessageStore SMSMessageStore { get; set; }
    }
}