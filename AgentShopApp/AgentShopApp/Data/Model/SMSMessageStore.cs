using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace AgentShopApp.Data
{
    public class SMSMessageStore
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string TextMessage { get; set; }
        public string SenderId { get; set; }
        public bool Processed { get; set; }
        public long UnixTimeStamp { get; set; }

    }
}