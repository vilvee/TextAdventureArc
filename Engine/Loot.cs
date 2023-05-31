using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Represents loot that can be obtained.
    /// </summary>
    public class Loot
    {
        /// <summary>
        /// Gets or sets the details of the item.
        /// </summary>
        public Item? Details { get; set; }

        /// <summary>
        /// Gets or sets the drop percentage of the loot.
        /// </summary>
        public int DropPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the loot is a default item.
        /// </summary>
        public bool IsDefaultItem { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loot"/> class with the specified values.
        /// </summary>
        /// <param name="details">The details of the item.</param>
        /// <param name="dropPercentage">The drop percentage of the loot.</param>
        /// <param name="isDefaultItem">A value indicating whether the loot is a default item.</param>
        public Loot(Item? details, int dropPercentage, bool isDefaultItem)
        {
            Details = details;
            DropPercentage = dropPercentage;
            IsDefaultItem = isDefaultItem;
        }
    }
}