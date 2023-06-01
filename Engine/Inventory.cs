using System.ComponentModel;


namespace Engine
{
    /// <summary>
    /// Represents an item in the inventory with its quantity.
    /// </summary>
    public class Inventory : INotifyPropertyChanged // INotifyPropertyChanged is an interface that allows us to notify the UI when a property changes.
    {

        private Item _detail;
        private int _quantity;

        /// <summary>
        /// Gets or sets the item details.
        /// </summary>
        public Item Detail
        {
            get => _detail;
            set
            {
                _detail = value;
                OnPropertyChanged("Details");
            }
        }

          /// <summary>
       /// Gets or sets the quantity of the item.
       /// </summary>
    public int Quantity
       {
           get => _quantity;
           set
           {
               _quantity = value;
               OnPropertyChanged("Quantity");
               OnPropertyChanged("Description");
           }
       }

        /// <summary>
        /// Gets the ID of the item.
        /// </summary>
        public int ItemID => Detail.ID;

/*        /// <summary>
        /// Gets the price of the item.
        /// </summary>
        public int Price => Detail.Price;*/

        /// <summary>
        /// Gets the description of the item.
        /// </summary>
        public string Description => Quantity > 1 ? Detail.NamePlural : Detail.Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class with the specified item and quantity.
        /// </summary>
        /// <param name="detail">The item details.</param>
        /// <param name="quantity">The quantity of the item.</param>
        public Inventory(Item detail, int quantity)
        {
            Detail = detail;
            Quantity = quantity;

        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property name.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
