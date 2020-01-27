using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Data.Model
{
    public class ClientData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique(Unique = false)]
        public string ClientName { get; set; }
        [Unique(Unique = true)]
        public string ClientPhone { get; set; }
        public long UnixTimeStamp { get; set; }
    }
}
