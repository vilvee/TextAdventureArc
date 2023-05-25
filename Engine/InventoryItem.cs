using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class InventoryItem
    {
        public int Quantity { get; set; }
        public Item Detail { get; set; } 

        public InventoryItem (Item detail, int quantity)
        {
           Detail = detail;  
           Quantity = quantity;
        }
    }
}
