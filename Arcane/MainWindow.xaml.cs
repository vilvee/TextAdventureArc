using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Engine;
using System.IO;
using System.ComponentModel;

namespace Arcane
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string PLAYER_DATA_FILE_NAME = "PlayerData2.xml";

        private Player _player;


        public MainWindow()
        {
            InitializeComponent();
            _player = PlayerDataMapper.CreateFromDatabase();
            if (_player == null)
            {
                if (File.Exists(PLAYER_DATA_FILE_NAME))
                {
                    _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
                }
                else
                {
                    _player = Player.CreateDefaultPlayer();
                }
            }


            lbHitPoints.SetBinding(ContentProperty, new Binding("CurrentHitPoints") { Source = _player });
            lbGold.SetBinding(ContentProperty, new Binding("Gold") { Source = _player });
            lbExperience.SetBinding(ContentProperty, new Binding("ExperiencePoints") { Source = _player });
            lbLevel.SetBinding(ContentProperty, new Binding("Level") { Source = _player });

            dgvInventory.AutoGenerateColumns = false;
            dgvInventory.ItemsSource = _player.Inventory;
            /*dgvInventory.HeadersVisibility = DataGridHeadersVisibility.None;*/

          /*  DataGridTextColumn nameColumn = new DataGridTextColumn
            {
                Header = "Name",
                Width = 197,
                Binding = new Binding("Description")
            };
            dgvInventory.Columns.Add(nameColumn);

            DataGridTextColumn quantityColumn = new DataGridTextColumn
            {
                Header = "Quantity",
                Binding = new Binding("Quantity")
            };
            dgvInventory.Columns.Add(quantityColumn);*/

            dgvQuests.AutoGenerateColumns = false;
            dgvQuests.ItemsSource = _player.Quests;
           /* dgvQuests.HeadersVisibility = DataGridHeadersVisibility.None;*/

           /* DataGridTextColumn nameColumn1 = new DataGridTextColumn
            {
                Header = "Name",
                Width = 197,
                Binding = new Binding("Name")
            };
            dgvQuests.Columns.Add(nameColumn1);

            DataGridTextColumn doneColumn = new DataGridTextColumn
            {
                Header = "Done?",
                Binding = new Binding("IsCompleted")
            };
            dgvQuests.Columns.Add(doneColumn);*/

            cboWeapons.ItemsSource = _player.Weapons;
            cboWeapons.DisplayMemberPath = "Name";
            cboWeapons.SelectedValuePath = "Id";

            if (_player.CurrentWeapon != null)
            {
                cboWeapons.SelectedItem = _player.CurrentWeapon;
            }

            cboWeapons.SelectionChanged += cboWeapons_SelectionChanged;

            cboPotions.ItemsSource = _player.Potions;
            cboPotions.DisplayMemberPath = "Name";
            cboPotions.SelectedValuePath = "Id";

            _player.PropertyChanged += PlayerOnPropertyChanged;

            _player.OnMessage += DisplayMessage;
            _player.MoveTo(_player.CurrentLocation);
        }

        #region Directions
        private void btnNorth_Click(object sender, EventArgs e)
        {
            _player.MoveNorth();
        }
        private void btnEast_Click(object sender, EventArgs e)
        {
            _player.MoveEast();
        }
        private void btnSouth_Click(object sender, EventArgs e)
        {
            _player.MoveSouth();
        }
        private void btnbtnWest_Click(object sender, EventArgs e)
        {
            _player.MoveWest();
        }
        #endregion



        private void btnUseWeapon_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected weapon from the cboWeapons ComboBox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;
            _player.UseWeapon(currentWeapon);
        }

        private void btnUsePotion_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;
            _player.UsePotion(potion);
        }

        private void rtbMessages_TextChanged(object sender, TextChangedEventArgs e)
        {
            // scroll it automatically
            rtbMessages.ScrollToEnd();
        }

        private void MyGame_Closed(object sender, EventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
            PlayerDataMapper.SaveToDatabase(_player);
        }

        private void cboWeapons_SelectionChanged(object sender, SelectionChangedEventArgs e)
              {
                  _player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
              }

        private void PlayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Weapons")
            {
                cboWeapons.ItemsSource = _player.Weapons;
                if (!_player.Weapons.Any())
                {
                    cboWeapons.Visibility = Visibility.Collapsed;
                    btnUseWeapon.Visibility = Visibility.Collapsed;
                }
            }
            if (propertyChangedEventArgs.PropertyName == "Potions")
            {
                cboPotions.ItemsSource = _player.Potions;
                if (!_player.Potions.Any())
                {
                    cboPotions.Visibility = Visibility.Collapsed;
                    btnUsePotion.Visibility = Visibility.Collapsed;
                }
            }
            if (propertyChangedEventArgs.PropertyName == "CurrentLocation")
            {
                // Show/hide available movement buttons
                btnNorth.Visibility = (_player.CurrentLocation.LocationToNorth != null) ? Visibility.Visible : Visibility.Collapsed;
                btnEast.Visibility = (_player.CurrentLocation.LocationToEast != null) ? Visibility.Visible : Visibility.Collapsed;
                btnSouth.Visibility = (_player.CurrentLocation.LocationToSouth != null) ? Visibility.Visible : Visibility.Collapsed;
                btnWest.Visibility = (_player.CurrentLocation.LocationToWest != null) ? Visibility.Visible : Visibility.Collapsed;
                btnTrade.Visibility = (_player.CurrentLocation.VendorWorkingHere != null)? Visibility.Visible : Visibility.Collapsed;
                // Display current location name and description
                FlowDocument flowDocument = new FlowDocument();

                Paragraph nameParagraph = new Paragraph(new Run(_player.CurrentLocation.Name));
                nameParagraph.Margin = new Thickness(0);
                flowDocument.Blocks.Add(nameParagraph);

                Paragraph descriptionParagraph = new Paragraph(new Run(_player.CurrentLocation.Description));
                descriptionParagraph.Margin = new Thickness(0);
                flowDocument.Blocks.Add(descriptionParagraph);

                rtbLocation.Document = flowDocument;


                if (_player.CurrentLocation.EnemyPresent == null)
                {
                    cboWeapons.Visibility = Visibility.Collapsed;
                    cboPotions.Visibility = Visibility.Collapsed;
                    btnUseWeapon.Visibility = Visibility.Collapsed;
                    btnUsePotion.Visibility = Visibility.Collapsed;
                }
                else
                {
                    cboWeapons.Visibility = _player.Weapons.Any() ? Visibility.Visible : Visibility.Collapsed;
                    cboPotions.Visibility = _player.Potions.Any() ? Visibility.Visible : Visibility.Collapsed;
                    btnUseWeapon.Visibility = _player.Weapons.Any() ? Visibility.Visible : Visibility.Collapsed;
                    btnUsePotion.Visibility = _player.Potions.Any() ? Visibility.Visible : Visibility.Collapsed;
                }
            }

        }

        private void DisplayMessage(object sender, MessageEventArgs messageEventArgs)
        {
            rtbMessages.AppendText(messageEventArgs.Message);
            if (messageEventArgs.AddExtraNewLine)
            {
                rtbMessages.AppendText(Environment.NewLine);
            }
            rtbMessages.ScrollToEnd();
        }

        private void btnTrade_Click(object sender, RoutedEventArgs e)
        {
            TradingScreen tradingScreen = new TradingScreen(_player);
            tradingScreen.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            tradingScreen.ShowDialog();
        }

        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            WorldMap mapScreen = new WorldMap();
            mapScreen.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapScreen.ShowDialog();
        }
    }
}
