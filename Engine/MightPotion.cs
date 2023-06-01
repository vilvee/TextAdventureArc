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
    public class MightPotion : Potion
    {
        /// <summary>
        /// Gets or sets the amount of hit points the potion can heal.
        /// </summary>
        public int BonusHit { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="HealingPotion"/> class with the specified values.
        /// </summary>
        /// <param name="id">The ID of the potion.</param>
        /// <param name="name">The name of the potion.</param>
        /// <param name="namePlural">The plural name of the potion.</param>
        /// <param name="bonusHit">The amount of hit points the potion can heal.</param>
        /// <param name="price">The price of the potion.</param>
        public MightPotion(int id, string name, string namePlural, int bonusHit, int price )
            : base(id, name, namePlural, price)
        {
            BonusHit = bonusHit; 
        }
    }
}