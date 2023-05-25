﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Quest
    {
        public Quest Details { get; set; }
        public bool IsCompleted { get; set; }

        public Quest(Quest details)
        {
            Details = details;
            IsCompleted = false;
        }
    }
}
}
