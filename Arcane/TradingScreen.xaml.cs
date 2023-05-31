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

            // Bind the player's inventory to the My Items data grid view.
            dgvMyItems.ItemsSource = _currentPlayer.Inventory;

            // Bind the vendor's inventory to the Vendor Items data grid view.
            dgvVendorItems.ItemsSource = _currentPlayer.CurrentLocation.VendorWorkingHere.Inventory;
        }

        /// <summary>
        /// Event handler for the cell click event in the My Items data grid view.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
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
                // Remove the item from the player's inventory and add its price to the player's gold.
                _currentPlayer.RemoveItemFromInventory(itemBeingSold);
                _currentPlayer.Gold += itemBeingSold.Price;
            }
        }

        /// <summary>
        /// Event handler for the cell click event in the Vendor Items data grid view.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void dgvVendorItems_CellClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            InventoryItem inventoryItem = (InventoryItem)button.Tag;
            int itemId = inventoryItem.ItemID;
            Item itemBeingSold = World.ItemByID(itemId);

            if (_currentPlayer.Gold >= itemBeingSold.Price)
            {
                // Add the item to the player's inventory and deduct its price from the player's gold.
                _currentPlayer.AddItemToInventory(itemBeingSold);
                _currentPlayer.Gold -= itemBeingSold.Price;
            }
            else
            {
                MessageBox.Show("You do not have enough gold to buy the " + itemBeingSold.Name);
            }
        }

        /// <summary>
        /// Event handler for the Close button click event.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // Close the trading screen.
            Close();
        }

        /// <summary>
        /// Event handler for the selection changed event in the Vendor Items data grid view.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void dgvVendorItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGridCellInfo selectedCell = dgvVendorItems.SelectedCells.FirstOrDefault();
            if (selectedCell != null)
            {
                // Check if the selected cell is in the "Buy" button column.
                if (selectedCell.Column.DisplayIndex == 3)
                {
                    // Get the selected row item as an Item object.
                    Item selectedRowItem = selectedCell.Item as Item;
                    if (selectedRowItem != null)
                    {
                        if (_currentPlayer.Gold >= selectedRowItem.Price)
                        {
                            // Add the item to the player's inventory and deduct its price from the player's gold.
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

        /// <summary>
        /// Event handler for the selection changed event in the My Items data grid view.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void dgvMyItems_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            DataGridCellInfo selectedCell = dgvMyItems.SelectedCells.FirstOrDefault();
            if (selectedCell != null)
            {
                // Check if the selected cell is in the "Sell" button column.
                if (selectedCell.Column.DisplayIndex == 4)
                {
                    // Get the selected row item as an Item object.
                    Item selectedRowItem = selectedCell.Item as Item;
                    if (selectedRowItem != null)
                    {
                        if (selectedRowItem.Price == World.UNSELLABLE_ITEM_PRICE)
                        {
                            MessageBox.Show("You cannot sell the " + selectedRowItem.Name);
                        }
                        else
                        {
                            // Remove the item from the player's inventory and add its price to the player's gold.
                            _currentPlayer.RemoveItemFromInventory(selectedRowItem);
                            _currentPlayer.Gold += selectedRowItem.Price;
                        }
                    }
                }
            }
        }

        #region WindowControls

        /// <summary>
        /// Event handler for moving the window when the user clicks and drags it.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TriggerMoveWindow(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == System.Windows.WindowState.Maximized)
                {
                    // If the window is maximized, restore it to normal state and calculate the new position based on the mouse position.
                    WindowState = System.Windows.WindowState.Normal;

                    double pct = PointToScreen(e.GetPosition(this)).X / System.Windows.SystemParameters.PrimaryScreenWidth;
                    Top = 0;
                    Left = e.GetPosition(this).X - (pct * Width);
                }

                // Move the window.
                DragMove();
            }
        }

        /// <summary>
        /// Event handler for maximizing or restoring the window when the user clicks on it.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TriggerMaximize(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
            {
                // If the window is already maximized, restore it to normal state.
                WindowState = System.Windows.WindowState.Normal;
            }
            else if (WindowState == System.Windows.WindowState.Normal)
            {
                // If the window is in normal state, maximize it.
                WindowState = System.Windows.WindowState.Maximized;
            }
        }

        /// <summary>
        /// Event handler for closing the window.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TriggerClose(object sender, RoutedEventArgs e)
        {
            // Close the window.
            Close();
        }

        /// <summary>
        /// Event handler for minimizing the window.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TriggerMinimize(object sender, RoutedEventArgs e)
        {
            // Minimize the window.
            WindowState = System.Windows.WindowState.Minimized;
        }

        #endregion

    }
}