using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Data.Model
{
    public class TransactionType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique(Unique = true)]
        public string Name { get; set; }
        public long UnixTimeStamp { get; set; }
    }
}
