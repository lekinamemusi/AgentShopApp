using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Data.Model
{
    public class GlMasterAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique(Unique = true)]
        public string AccountName { get; set; }

        public bool HasDebitBalance { get; set; }

        public long UnixTimeStamp { get; set; }
    }
}
