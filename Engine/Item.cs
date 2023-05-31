using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Represents an item.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Gets or sets the ID of the item.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the plural name of the item.
        /// </summary>
        public string NamePlural { get; set; }

        /// <summary>
        /// Gets or sets the price of the item.
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class with the specified values.
        /// </summary>
        /// <param name="id">The ID of the item.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="namePlural">The plural name of the item.</param>
        /// <param name="price">The price of the item.</param>
        public Item(int id, string name, string namePlural, int price)
        {
            ID = id;
            Name = name;
            NamePlural = namePlural;
            Price = price;
        }
    }
}