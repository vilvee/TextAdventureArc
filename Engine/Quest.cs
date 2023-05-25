using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Quest
    {
        public Level Details { get; set; }
        public bool IsCompleted { get; set; }

        public Quest(Level details)
        {
            Details = details;
            IsCompleted = false;
        }
    }
}

