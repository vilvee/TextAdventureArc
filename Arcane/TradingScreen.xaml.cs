using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Engine;

namespace Arcane
{
    /// <summary>
    /// Interaction logic for TradingScreen.xaml
    /// </summary>
    public partial class TradingScreen : Window
    {
        public Player _currentPlayer;

        public TradingScreen(Player player)
        {
            _currentPlayer = player;
            InitializeComponent();

            // Bind the player's inventory to the datagridview 
            dgvMyItems.ItemsSource = _currentPlayer.Inventory;

            // Bind the vendor's inventory to the datagridview 
            dgvVendorItems.ItemsSource = _currentPlayer.CurrentLocation.VendorWorkingHere.Inventory;

        }
        private void dgvMyItems_CellClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            InventoryItem inventoryItem = (InventoryItem)button.Tag;
            int itemId = inventoryItem.ItemID;
            Item itemBeingSold = World.ItemByID(itemId);

            if (itemBeingSold.Price == World.UNSELLABLE_ITEM_PRICE)
            {
                MessageBox.Show("You cannot sell the " + itemBeingSold.Name);
            }
            else
            {
                _currentPlayer.RemoveItemFromInventory(itemBeingSold);
                _currentPlayer.Gold += itemBeingSold.Price;
            }
        }

        private void dgvVendorItems_CellClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            InventoryItem inventoryItem = (InventoryItem)button.Tag;
            int itemId = inventoryItem.ItemID;
            Item itemBeingSold = World.ItemByID(itemId);

            if (_currentPlayer.Gold >= itemBeingSold.Price)
            {
                _currentPlayer.AddItemToInventory(itemBeingSold);
                _currentPlayer.Gold -= itemBeingSold.Price;
            }
            else
            {
                MessageBox.Show("You do not have enough gold to buy the " + itemBeingSold.Name);
            }
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       private void dgvVendorItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGridCellInfo selectedCell = dgvVendorItems.SelectedCells.FirstOrDefault();
            if (selectedCell != null)
            {
                if (selectedCell.Column.DisplayIndex == 3)
                {
                    Item selectedRowItem = selectedCell.Item as Item;
                    if (selectedRowItem != null)
                    {
                        if (_currentPlayer.Gold >= selectedRowItem.Price)
                        {
                            _currentPlayer.AddItemToInventory(selectedRowItem);
                            _currentPlayer.Gold -= selectedRowItem.Price;
                        }
                        else
                        {
                            MessageBox.Show("You do not have enough gold to buy the " + selectedRowItem.Name);
                        }
                    }
                }
            }
        }

        private void dgvMyItems_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            DataGridCellInfo selectedCell = dgvMyItems.SelectedCells.FirstOrDefault();
            if (selectedCell != null)
            {
                if (selectedCell.Column.DisplayIndex == 4)
                {
                    Item selectedRowItem = selectedCell.Item as Item;
                    if (selectedRowItem != null)
                    {
                        if (selectedRowItem.Price == World.UNSELLABLE_ITEM_PRICE)
                        {
                            MessageBox.Show("You cannot sell the " + selectedRowItem.Name);
                        }
                        else
                        {
                            _currentPlayer.RemoveItemFromInventory(selectedRowItem);
                            _currentPlayer.Gold += selectedRowItem.Price;
                        }
                    }
                }
            }
        }
        #region WindowControls

        private void TriggerMoveWindow(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == System.Windows.WindowState.Maximized)
                {
                    WindowState = System.Windows.WindowState.Normal;

                    double pct = PointToScreen(e.GetPosition(this)).X / System.Windows.SystemParameters.PrimaryScreenWidth;
                    Top = 0;
                    Left = e.GetPosition(this).X - (pct * Width);
                }

                DragMove();
            }
        }

        private void TriggerMaximize(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
                WindowState = System.Windows.WindowState.Normal;
            else if (WindowState == System.Windows.WindowState.Normal)
                WindowState = System.Windows.WindowState.Maximized;
        }

        private void TriggerClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TriggerMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        #endregion
    }
}
