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
    public class Potion : Item
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Engine.HealingPotion"/> class with the specified values.
        /// </summary>
        /// <param name="id">The ID of the potion.</param>
        /// <param name="name">The name of the potion.</param>
        /// <param name="namePlural">The plural name of the potion.</param>
        /// <param name="price">The price of the potion.</param>
        public Potion(int id, string name, string namePlural, int price)
            : base(id, name, namePlural, price)
        {
            
        }

       
    }
}