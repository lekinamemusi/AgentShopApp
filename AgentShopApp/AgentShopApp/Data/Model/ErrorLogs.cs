using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Data.Model
{
    public class ErrorLogs
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public long UnixTimeStamp { get; set; }
        public DateTime LogTime { get; set; }
        public string ErrorMessage { get; set; }
        public string FullException { get; set; }
        public string Module { get; set; }
    }
}
