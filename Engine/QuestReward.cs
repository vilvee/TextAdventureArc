using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Represents a reward for completing a quest.
    /// </summary>
    public class QuestReward
    {
        /// <summary>
        /// Gets or sets the details of the item rewarded.
        /// </summary>
        public Item Details { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item rewarded.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestReward"/> class with the specified details and quantity.
        /// </summary>
        /// <param name="details">The details of the item rewarded.</param>
        /// <param name="quantity">The quantity of the item rewarded.</param>
        public QuestReward(Item details, int quantity)
        {
            Details = details;
            Quantity = quantity;
        }
    }
}