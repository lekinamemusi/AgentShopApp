using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Data.Model
{
    public class GeneralLedger
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public long UnixTimeStamp { get; set; }

        [Indexed(Unique = false)]
        public long GlAccountId { get; set; }

        public double Credit { get; set; }
        public double Debit { get; set; }

        [Indexed(Unique = false)]
        public string TransactionId { get; set; }

        [Indexed(Unique = false)]
        public DateTime TransactionTime { get; set; }

        [Indexed(Unique = false)]
        public int ClientDataId { get; set; }
        [Indexed(Unique = false)]
        public bool TransactionSynced { get; set; }

        public string Narration { get; set; }


        [Ignore]
        public ClientData ClientData { get; set; }
        [Ignore]
        public GlAccount GlAccount { get; set; }
    }
}
