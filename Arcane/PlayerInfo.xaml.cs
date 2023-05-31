using Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
   
    public partial class PlayerInfo : Window
    {
        private Player _CurrentPlayer;

        public PlayerInfo(Player player)
        {
            InitializeComponent();

            // Set the current player.
            _CurrentPlayer = player;

            // Bind the properties of the player to the labels.
            lbHitPoints.SetBinding(ContentProperty, new Binding("CurrentHitPoints") { Source = _CurrentPlayer });
            lbGold.SetBinding(ContentProperty, new Binding("Gold") { Source = _CurrentPlayer });
            lbExperience.SetBinding(ContentProperty, new Binding("ExperiencePoints") { Source = _CurrentPlayer });
            lbLevel.SetBinding(ContentProperty, new Binding("Level") { Source = _CurrentPlayer });

            // Set the item source of the inventory and quests data grids.
            dgvInventory.ItemsSource = _CurrentPlayer.Inventory;
            dgvQuests.ItemsSource = _CurrentPlayer.Quests;
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

        // Sort direction variables for the inventory and quests data grids.
        private ListSortDirection dgvInventorySortDirection = ListSortDirection.Ascending;
        private ListSortDirection dgvQuestsSortDirection = ListSortDirection.Ascending;

        /// <summary>
        /// Event handler for sorting the inventory data grid.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void dgvInventory_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            // Get the sort member path (property name) from the column being sorted.
            string propertyName = e.Column.SortMemberPath;

            // Get the inventory of the current player.
            BindingList<InventoryItem> inventory = _CurrentPlayer.Inventory;

            if (inventory != null)
            {
                // Sort the inventory based on the sort direction.
                List<InventoryItem> sortedInventory = dgvInventorySortDirection == ListSortDirection.Ascending ?
                    inventory.OrderBy(x => GetPropertyValue(x, propertyName)).ToList() :
                    inventory.OrderByDescending(x => GetPropertyValue(x, propertyName)).ToList();

                // Clear and reassign the sorted inventory as the data source of the inventory data grid.
                dgvInventory.ItemsSource = null;
                dgvInventory.ItemsSource = sortedInventory;

                // Toggle the sort direction for the next sorting operation.
                dgvInventorySortDirection = dgvInventorySortDirection == ListSortDirection.Ascending ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }
        }

        /// <summary>
        /// Event handler for sorting the quests data grid.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void dgvQuests_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            // Get the sort member path (property name) from the column being sorted.
            string propertyName = e.Column.SortMemberPath;

            // Get the quests of the current player.
            BindingList<Quest> quests = _CurrentPlayer.Quests;

            if (quests != null)
            {
                // Sort the quests based on the sort direction.
                List<Quest> sortedQuests = dgvQuestsSortDirection == ListSortDirection.Ascending ?
                    quests.OrderBy(x => GetPropertyValue(x, propertyName)).ToList() :
                    quests.OrderByDescending(x => GetPropertyValue(x, propertyName)).ToList();

                // Clear and reassign the sorted quests as the data source of the quests data grid.
                dgvQuests.ItemsSource = null;
                dgvQuests.ItemsSource = sortedQuests;

                // Toggle the sort direction for the next sorting operation.
                dgvQuestsSortDirection = dgvQuestsSortDirection == ListSortDirection.Ascending ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }
        }

        /// <summary>
        /// Retrieves the value of a specific property from an object.
        /// </summary>
        /// <param name="obj">The object from which to retrieve the property value.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the specified property.</returns>
        private object GetPropertyValue(object obj, string propertyName)
        {
            // Get the PropertyInfo of the specified property.
            PropertyInfo property = obj.GetType().GetProperty(propertyName);

            // Get and return the value of the property.
            return property.GetValue(obj);
        }

        /// <summary>
        /// Event handler for the "Map" button click event.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            // Create a new instance of the WorldMap screen.
            WorldMap mapScreen = new WorldMap(_CurrentPlayer);

            // Set the startup location of the WorldMap screen to the center of its owner.
            mapScreen.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Show the WorldMap screen as a modal dialog.
            mapScreen.ShowDialog();
        }


    }
}
