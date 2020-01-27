using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Data.Model
{
    public class GlAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public long UnixTimeStamp { get; set; }
        [Unique(Unique = false)]
        public int GlMasterAccountId { get; set; }
        [Unique(Unique = true)]
        public string AccountName { get; set; } 

        [Ignore]
        public GlMasterAccount GlMasterAccount { get; set; }

    }
}
