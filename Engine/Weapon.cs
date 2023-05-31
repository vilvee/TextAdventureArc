using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Represents a weapon item in the game.
    /// </summary>
    public class Weapon : Item
    {
        /// <summary>
        /// Gets or sets the minimum damage of the weapon.
        /// </summary>
        public int MinimumDamage { get; set; }

        /// <summary>
        /// Gets or sets the maximum damage of the weapon.
        /// </summary>
        public int MaximumDamage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Weapon"/> class with the specified ID, name, plural name, minimum damage, maximum damage, and price.
        /// </summary>
        /// <param name="id">The ID of the weapon.</param>
        /// <param name="name">The name of the weapon.</param>
        /// <param name="namePlural">The plural name of the weapon.</param>
        /// <param name="minimumDamage">The minimum damage of the weapon.</param>
        /// <param name="maximumDamage">The maximum damage of the weapon.</param>
        /// <param name="price">The price of the weapon.</param>
        public Weapon(int id, string name, string namePlural, int minimumDamage, int maximumDamage, int price)
            : base(id, name, namePlural, price)
        {
            MaximumDamage = maximumDamage;
            MinimumDamage = minimumDamage;
        }
    }
}