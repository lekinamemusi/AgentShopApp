using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Data.Model
{
    public class FailedSyncSMS
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string TextMessage { get; set; }
        [Unique(Unique = false)]
        public string TransactionId { get; set; }

        public long UnixTimeStamp { get; set; }

        public bool Sorted { get; set; }

        public string ErrorMessage { get; set; }
    }
}
