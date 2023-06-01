using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Engine
{
    /// <summary>
    /// Represents a vendor in the game.
    /// </summary>
    public class Vendor : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the name of the vendor.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the inventory of the vendor.
        /// </summary>
        public BindingList<Inventory> Inventory { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vendor"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the vendor.</param>
        public Vendor(string name)
        {
            Name = name;
            Inventory = new BindingList<Inventory>();
        }

        /// <summary>
        /// Adds an item to the vendor's inventory with the specified quantity.
        /// </summary>
        /// <param name="itemToAdd">The item to add to the inventory.</param>
        /// <param name="quantity">The quantity of the item to add (default is 1).</param>
        public void AddItemToInventory(Item itemToAdd, int quantity = 1)
        {
            Inventory item = Inventory.SingleOrDefault(ii => ii.Detail.ID == itemToAdd.ID);
            if (item == null)
            {
                // They didn't have the item, so add it to their inventory
                Inventory.Add(new Inventory(itemToAdd, quantity));
            }
            else
            {
                // They have the item in their inventory, so increase the quantity
                item.Quantity += quantity;
            }

            OnPropertyChanged("Inventory");
        }

        /// <summary>
        /// Removes an item from the vendor's inventory with the specified quantity.
        /// </summary>
        /// <param name="itemToRemove">The item to remove from the inventory.</param>
        /// <param name="quantity">The quantity of the item to remove (default is 1).</param>
        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            Inventory item = Inventory.SingleOrDefault(ii => ii.Detail.ID == itemToRemove.ID);
            if (item == null)
            {
                // The item is not in the vendor's inventory, so ignore it.
                // We might want to raise an error for this situation
            }
            else
            {
                // They have the item in their inventory, so decrease the quantity
                item.Quantity -= quantity;

                // Don't allow negative quantities.
                // We might want to raise an error for this situation
                if (item.Quantity < 0)
                {
                    item.Quantity = 0;
                }

                // If the quantity is zero, remove the item from the list
                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }

                // Notify the UI that the inventory has changed
                OnPropertyChanged("Inventory");
            }
        }

        /// <summary>
        /// Event that is raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event with the specified property name.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}