﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentShopApp.Views.Master
{

    public class MasterDetailPageMenuItem
    {
        public MasterDetailPageMenuItem()
        {
            
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}