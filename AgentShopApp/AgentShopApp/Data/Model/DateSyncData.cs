using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Data.Model
{
    public class DateSyncData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed(Unique = false)]
        public string SenderId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Indexed(Unique = false)]
        public bool SyncComplete { get; set; }

        [Indexed(Unique = false)]
        public long UnixTimeStamp { get; set; }
    }
}
