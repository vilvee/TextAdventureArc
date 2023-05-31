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
            _CurrentPlayer = player;

            lbHitPoints.SetBinding(ContentProperty, new Binding("CurrentHitPoints") { Source = _CurrentPlayer });
            lbGold.SetBinding(ContentProperty, new Binding("Gold") { Source = _CurrentPlayer });
            lbExperience.SetBinding(ContentProperty, new Binding("ExperiencePoints") { Source = _CurrentPlayer });
            lbLevel.SetBinding(ContentProperty, new Binding("Level") { Source = _CurrentPlayer });


            
            dgvInventory.ItemsSource = _CurrentPlayer.Inventory;
            dgvQuests.ItemsSource = _CurrentPlayer.Quests;


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

        private ListSortDirection dgvInventorySortDirection = ListSortDirection.Ascending;
        private ListSortDirection dgvQuestsSortDirection = ListSortDirection.Ascending;

        private void dgvInventory_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            string propertyName = e.Column.SortMemberPath;
            BindingList<InventoryItem> inventory = _CurrentPlayer.Inventory;

            if (inventory != null)
            {
                List<InventoryItem> sortedInventory = dgvInventorySortDirection == ListSortDirection.Ascending ?
                    inventory.OrderBy(x => GetPropertyValue(x, propertyName)).ToList() :
                    inventory.OrderByDescending(x => GetPropertyValue(x, propertyName)).ToList();

                dgvInventory.ItemsSource = null;
                dgvInventory.ItemsSource = sortedInventory;
                dgvInventorySortDirection = dgvInventorySortDirection == ListSortDirection.Ascending ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }
        }

        private void dgvQuests_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            string propertyName = e.Column.SortMemberPath;
            BindingList<Quest> quests = _CurrentPlayer.Quests;

            if (quests != null)
            {
                List<Quest> sortedQuests = dgvQuestsSortDirection == ListSortDirection.Ascending ?
                    quests.OrderBy(x => GetPropertyValue(x, propertyName)).ToList() :
                    quests.OrderByDescending(x => GetPropertyValue(x, propertyName)).ToList();

                dgvQuests.ItemsSource = null;
                dgvQuests.ItemsSource = sortedQuests;
                dgvQuestsSortDirection = dgvQuestsSortDirection == ListSortDirection.Ascending ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            PropertyInfo property = obj.GetType().GetProperty(propertyName);
            return property.GetValue(obj);
        }

        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            WorldMap mapScreen = new WorldMap(_CurrentPlayer);
            mapScreen.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapScreen.ShowDialog();
        }
    }
}
