using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Engine
{
    public class InventoryItem : INotifyPropertyChanged
    {
        private Item _detail;
        private int _quantity;
        public Item Detail
        {
            get => _detail;
            set
            {
                _detail = value;
                OnPropertyChanged("Details");
            }
        }
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
        public int ItemID => Detail.ID;
        public int Price => Detail.Price;
        public string Description => Quantity > 1 ? Detail.NamePlural : Detail.Name;

        public InventoryItem (Item detail, int quantity)
        {
           Detail = detail;  
           Quantity = quantity;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
