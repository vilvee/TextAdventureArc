using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Engine;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Reflection;

namespace Arcane
{

    public partial class MainWindow : Window
    {
        private const string PLAYER_DATA_FILE_NAME = "PlayerData2.xml";

        private readonly Player _player;


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
                    cboWeapons.IsEnabled = false;
                    btnUseWeapon.IsEnabled = false;
                }
                else
                {
                    cboWeapons.IsEnabled = true;
                    btnUseWeapon.IsEnabled = true;
                }
            }
            if (propertyChangedEventArgs.PropertyName == "Potions")
            {
                cboPotions.ItemsSource = _player.Potions;
                if (!_player.Potions.Any())
                {
                    cboPotions.IsEnabled = false;
                    btnUsePotion.IsEnabled = false;
                }
                else
                {
                    cboPotions.IsEnabled = true;
                    btnUsePotion.IsEnabled = true;
                }
            

        }
            if (propertyChangedEventArgs.PropertyName == "CurrentLocation")
            {
                // Show/hide available movement buttons
                btnNorth.Visibility = (_player.CurrentLocation.LocationToNorth != null) ? Visibility.Visible : Visibility.Hidden;
                btnEast.Visibility = (_player.CurrentLocation.LocationToEast != null) ? Visibility.Visible : Visibility.Hidden;
                btnSouth.Visibility = (_player.CurrentLocation.LocationToSouth != null) ? Visibility.Visible : Visibility.Hidden;
                btnWest.Visibility = (_player.CurrentLocation.LocationToWest != null) ? Visibility.Visible : Visibility.Hidden;
                btnTradeBorder.Visibility = (_player.CurrentLocation.VendorWorkingHere != null)? Visibility.Visible : Visibility.Hidden;
                // Display current location name and description
                FlowDocument flowDocument = new FlowDocument();

                Paragraph nameParagraph = new Paragraph(new Run(_player.CurrentLocation.Name));
                nameParagraph.Margin = new Thickness(0);
                flowDocument.Blocks.Add(nameParagraph);

                Paragraph descriptionParagraph = new Paragraph(new Run(_player.CurrentLocation.Description));
                descriptionParagraph.Margin = new Thickness(0);
                flowDocument.Blocks.Add(descriptionParagraph);

                rtbLocation.Document = flowDocument;


                if (_player.CurrentLocation.NewInstanceOfEnemyLivingHere() == null)
                {
                    cboWeapons.IsEnabled = false;
                    cboPotions.IsEnabled = false;
                    btnUseWeapon.IsEnabled = false;
                    btnUsePotion.IsEnabled = false;
                }
                else
                {
                    cboWeapons.IsEnabled = _player.Weapons.Any();
                    cboPotions.IsEnabled = _player.Potions.Any();
                    btnUseWeapon.IsEnabled = _player.Weapons.Any();
                    btnUsePotion.IsEnabled = _player.Potions.Any();
                }

            }

        }

        private void DisplayMessage(object sender, MessageEventArgs messageEventArgs)
        {
            rtbMessages.AppendText(messageEventArgs.Message + Environment.NewLine);
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
            WorldMap mapScreen = new WorldMap(_player);
            mapScreen.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapScreen.ShowDialog();
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

        private void btnInfo_Checked(object sender, RoutedEventArgs e)
        {
            PlayerInfo playerInfoScreen = new PlayerInfo(_player);
            playerInfoScreen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            playerInfoScreen.Show();
        }
    }
}
