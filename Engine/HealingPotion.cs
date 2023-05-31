
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Represents a healing potion item.
    /// </summary>
    public class HealingPotion : Potion
    {
       

        /// <summary>
        /// Gets or sets the amount of hit points the potion can heal.
        /// </summary>
        public int AmountToHeal { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HealingPotion"/> class with the specified values.
        /// </summary>
        /// <param name="id">The ID of the potion.</param>
        /// <param name="name">The name of the potion.</param>
        /// <param name="namePlural">The plural name of the potion.</param>
        /// <param name="amountHeal">The amount of hit points the potion can heal.</param>
        /// <param name="price">The price of the potion.</param>
        public HealingPotion(int id, string name, string namePlural, int amountHeal, int price) 
            : base(id, name, namePlural, price)
        {
            AmountToHeal = amountHeal;
        }

      
    }
}
