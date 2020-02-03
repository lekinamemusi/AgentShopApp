using System;
using System.Collections.Generic;
using System.Text;

namespace AgentShopApp.Model
{
    public class SMSReaderFilterModel
    {
        public IEnumerable<String> SenderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
