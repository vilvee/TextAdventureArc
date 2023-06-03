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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Threading;

namespace Arcane
{

    public partial class GameWindow : Window
    {

        public Player _player { get; set; }
        private Dictionary<int, string> _imagePaths;
        private Dictionary<int, string> _NPCiDs;
        private int _currentNPCID;
        private string _currentNPCName;
        private int _currentLocationID;

        private StreamReader streamReader;
        private string filePath;
        private string[] lines;
        private int currentLineIndex;
        private bool typewriterEffect = true;
        private bool interruptTypewriter;
        private bool isReading;
        private int currentCharIndex;

        /// <summary>
        /// Initializes a new instance of the GameWindow class.
        /// </summary>
        public GameWindow(Player player)
        {
            InitializeComponent();

            //_plaper = player;
            _player = player;
            UpdateBackgroundImage();

               _player.AddItemToInventory(World.ItemByID(World.ITEM_ID_MIGHT_POTION));

            string characterImagePath = _player.CharacterImagePath;

            // load the imgPlayer from characterImagePath
            SetImage(imgPlayer, characterImagePath);

            // Set up data bindings for player properties
            lblName.SetBinding(ContentProperty, new Binding("CharacterName") { Source = _player });
            lbHitPoints.SetBinding(ContentProperty, new Binding("CurrentHitPoints") { Source = _player });
            lbGold.SetBinding(ContentProperty, new Binding("Gold") { Source = _player });
            lbExperience.SetBinding(ContentProperty, new Binding("ExperiencePoints") { Source = _player });
            lbLevel.SetBinding(ContentProperty, new Binding("Quest") { Source = _player });

            // Set up combo box for weapons
            cboWeapons.ItemsSource = _player.Weapons;
            cboWeapons.DisplayMemberPath = "Name";
            cboWeapons.SelectedValuePath = "Id";

            // Set the selected weapon if available
            if (_player.CurrentWeapon != null)
            {
                cboWeapons.SelectedItem = _player.CurrentWeapon;
            }

            cboWeapons.SelectionChanged += cboWeapons_SelectionChanged;

            // Set up combo box for potions
            cboPotions.ItemsSource = _player.Potions;
            cboPotions.DisplayMemberPath = "Name";
            cboPotions.SelectedValuePath = "Id";

            if (_player.CurrentPotion != null)
            {
                cboPotions.SelectedItem = _player.CurrentPotion;
            }

            cboPotions.SelectionChanged += cboPotions_SelectionChanged;

            // Subscribe to player's property changed event
            _player.PropertyChanged += PlayerOnPropertyChanged;

            // Subscribe to player's message event and move to current location

            MessageHandler.OnMessage += HandleMessage;


            Navigation.MoveTo(_player, _player.CurrentLocation);

        }

        private void SetNPCImage()
        {

            // Set the background image from code-behind
            _NPCiDs = new Dictionary<int, string>
            {
                { 1 , "../../../Images/NPC/LuckyShopOwner.jpg" },
            };

            if (_player.CurrentLocation.VendorWorkingHere != null)
            {
                _currentNPCID = _player.CurrentLocation.VendorWorkingHere.ID;
                _currentNPCName = _player.CurrentLocation.VendorWorkingHere.Name;
                SetImage(imgNPC, _NPCiDs[_currentNPCID]);
            }
            else
            {
                imgNPC.Source = null;
            }
        }

        private void UpdateBackgroundImage()
        {
            // Set the background image from code-behind
            _imagePaths = new Dictionary<int, string>
            {
                { 1 , "../../../Images/homeVillage.jpg" },
                { 2 , "../../../Images/wastelands.jpg" },
                { 3 , "../../../Images/ruinedCity.jpg" },
                { 4 , "../../../Images/forestOfEchoes.jpg" },
                { 5 , "../../../Images/ancientTemple.jpg" },
                { 6 , "../../../Images/cursedDesert.jpg" },
                { 7 , "../../../Images/mountainOfSolitude.jpg" },
                { 8 , "../../../Images/skyFortress.jpg" },
                { 9 , "../../../Images/underwaterRuins.jpg" },
                { 10 , "../../../Images/sanctuary.jpg" },

            };

            _currentLocationID = _player.CurrentLocation.ID;

            this.Background = SetBackgroundImage(_imagePaths[_currentLocationID]);
        }

        private void SetImage(Image img, string imagePath)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            img.Source = bitmapImage;
        }

        private ImageBrush SetBackgroundImage(string imagePath)
        {
            ImageBrush imageBrush = new();
            imageBrush.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            return imageBrush;
        }

        private bool IsVendorPresent() => (_player.CurrentLocation.VendorWorkingHere != null) ? true : false;


        private void DialogueAvailable()
        {
            if (IsVendorPresent())
            {
                int counter = _player.CurrentLocation.VendorWorkingHere.Counter;
                filePath = $"../../../Dialogues/{_currentNPCName}/{counter}.txt";

                lines = File.ReadAllLines(filePath);
                currentLineIndex = 0;
                currentCharIndex = 0;
                isReading = true;
                DisableAllButtonsForDialogue();

                rtbMessages.Document.Blocks.Clear();
                ReadAndDisplayNextLine();
            }
        }

        private async void ReadAndDisplayNextLine()
        {
            if (!isReading)
            {
                btnNext.IsEnabled = false;
                //EnableAllButtonsExceptDialogue();
                return;
            }

            if (typewriterEffect)
            {
                string currentLine = lines[currentLineIndex];
                typewriterEffect = false;
                interruptTypewriter = false;

                // Split the line by the first comma
                string[] lineParts = currentLine.Split(new char[] { ',' }, 2);

                // Replace the 'V' with the 'vendorname' variable
                lineParts[0] = lineParts[0].Replace("V", _player.CurrentLocation.VendorWorkingHere.Name).Replace("P", _player.CharacterName  );

                // Check if the line contains a comma
                if (lineParts.Length > 1)
                {
                    // Concatenate the modified line parts
                    currentLine = lineParts[0] + ":" + lineParts[1] + Environment.NewLine;
                }

                for (int i = currentCharIndex; i < currentLine.Length; i++)
                {
                    if (interruptTypewriter)
                    {
                        typewriterEffect = true;
                        interruptTypewriter = false;
                        rtbMessages.AppendText(currentLine.Substring(currentCharIndex));
                        rtbMessages.ScrollToEnd();
                        break;
                    }

                    rtbMessages.AppendText(currentLine[i].ToString());
                    await Task.Delay(50);
                    rtbMessages.ScrollToEnd();
                    currentCharIndex++;
                }

                if (!interruptTypewriter && currentCharIndex == currentLine.Length - 1)
                {
                    typewriterEffect = true;
                    currentLineIndex++;
                    currentCharIndex = 0;
                    if (currentLineIndex < lines.Length)
                    {
                        await Task.Delay(500);  // Delay before showing the next line
                        rtbMessages.AppendText(Environment.NewLine + lines[currentLineIndex]);
                        rtbMessages.ScrollToEnd();
                        //DisableAllButtonsForDialogue();
                    }
                    else
                    {
                        isReading = false;
                        EnableAllButtonsExceptDialogue();
                    }
                }
            }
            else
            {
                typewriterEffect = true;
                ReadAndDisplayNextLine();
            }
        }



        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (isReading)
            {
                if (!typewriterEffect)
                {
                    interruptTypewriter = true;
                }
                else
                {
                    currentLineIndex++;
                    currentCharIndex = 0;
                    if (currentLineIndex < lines.Length)
                    {
                        ReadAndDisplayNextLine();
                    }
                    else
                    {
                        isReading = false;
                        
                    }
                }
            }

            if (!isReading)
            {
                EnableAllButtonsExceptDialogue();
            }
        }


        void EnableAllButtonsExceptDialogue()
        {
            btnNext.IsEnabled = false;
            btnNorth.IsEnabled = true;
            btnEast.IsEnabled = true;
            btnSouth.IsEnabled = true;
            btnWest.IsEnabled = true;
            btnInfo.IsEnabled = true;
            btnMap.IsEnabled = true;
            btnSave.IsEnabled = true;
            btnSaveAndQuit.IsEnabled = true;
            btnTrade.IsEnabled = true;
        }

        void DisableAllButtonsForDialogue( )
        {
          
           btnNorth.IsEnabled = false;
           btnEast.IsEnabled = false;
           btnSouth.IsEnabled = false;
           btnWest.IsEnabled = false;
           btnInfo.IsEnabled = false;
           btnMap.IsEnabled = false;
           btnSave.IsEnabled = false;
           btnSaveAndQuit.IsEnabled = false;
           btnTrade.IsEnabled = false;


        }


    



    #region Directions
    /// <summary>
    /// Handles the click event for the North button and moves the player north.
    /// </summary>
    private void btnNorth_Click(object sender, EventArgs e)
        {
            Navigation.MoveNorth(_player);
            UpdateBackgroundImage();
            SetNPCImage();
            DialogueAvailable();
        }

        /// <summary>
        /// Handles the click event for the East button and moves the player east.
        /// </summary>
        private void btnEast_Click(object sender, EventArgs e)
        {
            Navigation.MoveEast(_player);
            UpdateBackgroundImage();
            SetNPCImage();
            DialogueAvailable();
        }

        /// <summary>
        /// Handles the click event for the South button and moves the player south.
        /// </summary>
        private void btnSouth_Click(object sender, EventArgs e)
        {
            Navigation.MoveSouth(_player);
            UpdateBackgroundImage();
            SetNPCImage();
            DialogueAvailable();
        }

        /// <summary>
        /// Handles the click event for the West button and moves the player west.
        /// </summary>
        private void btnbtnWest_Click(object sender, EventArgs e)
        {
            Navigation.MoveWest(_player);
            UpdateBackgroundImage();
            SetNPCImage();
            DialogueAvailable();
        }
        #endregion

        #region Buttons and Drop Menus

        public Combat HasEnemy()
        {
            if (_player.CurrentLocation.HasAEnemy)
            {
                Combat combat = new Combat(_player.CurrentLocation);
                return combat;
            }

            return null;
        }

        /// <summary>
        /// Handles the click event for the Use Weapon button and uses the currently selected weapon.
        /// </summary>
        private void btnUseWeapon_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected weapon from the cboWeapons ComboBox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;
            Combat combat = HasEnemy();

            if (combat != null)
            {
                combat.UseWeapon(_player, currentWeapon);
            }

        }

        /// <summary>
        /// Handles the click event for the Use Potion button and uses the currently selected potion.
        /// </summary>
        private void btnUsePotion_Click(object sender, RoutedEventArgs e)
        {

            // Get the currently selected potion from the combobox
            Potion potion = (Potion)cboPotions.SelectedItem;
            Combat combat = HasEnemy();
            if (combat != null)
            {
                if (potion is HealingPotion)
                {
                    HealingPotion healingPotion = potion as HealingPotion;
                    if (healingPotion != null)
                    {
                        combat.UsePotion(_player, healingPotion);
                    }
                }
                else if (potion is MightPotion)
                {
                    MightPotion mightPotion = potion as MightPotion;
                    if (mightPotion != null)
                    {
                        combat.UsePotion(_player, null, mightPotion);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the selection changed event for the cboWeapons ComboBox and updates the player's current weapon.
        /// </summary>
        private void cboWeapons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_player.CurrentWeapon != null && cboWeapons.SelectedItem == null)
            {
                cboWeapons.SelectedItem = _player.CurrentWeapon;
            }
        }

        private void cboWeapons_Loaded(object sender, RoutedEventArgs e)
        {
            AutoSelectWeaponIfSingle();
        }

        private void AutoSelectWeaponIfSingle()
        {
            if (cboWeapons.Items.Count == 1)
            {
                cboWeapons.SelectedItem = cboWeapons.Items[0];
            }
        }

        private void cboPotions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_player.CurrentPotion != null && cboPotions.SelectedItem == null)
            {
                cboPotions.SelectedItem = _player.CurrentPotion;
            }
        }

        private void cboPotions_Loaded(object sender, RoutedEventArgs e)
        {
            if (cboPotions.Items.Count > 0)
            {
                cboPotions.SelectedIndex = 0; // Set the default item index to 0 (the first item)
            }
        }

        /// <summary>
        /// Event handler for the "Info" button click event.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnInfo_Checked(object sender, RoutedEventArgs e)
        {
            // Create a new instance of the PlayerInfo screen.
            PlayerInfo playerInfoScreen = new PlayerInfo(_player);
            // Set the startup location of the PlayerInfo screen to the center of the window.
            playerInfoScreen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            // Show the PlayerInfo screen.
            playerInfoScreen.Show();
        }

        /// <summary>
        /// Handles the click event for the Trade button and opens the trading screen.
        /// </summary>
        private void btnTrade_Click(object sender, RoutedEventArgs e)
        {
            TradingScreen tradingScreen = new TradingScreen(_player);
            tradingScreen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            tradingScreen.ShowDialog();
        }

        /// <summary>
        /// Handles the click event for the Map button and opens the world map screen.
        /// </summary>
        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            WorldMap mapScreen = new WorldMap(_player);
            mapScreen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mapScreen.ShowDialog();
        }


        /// <summary>
        /// Handles the click event for the Map button and opens the world map screen.
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string dataFile = _player.PlayerDataFileName;
            // Save player data to XML file
            File.WriteAllText(dataFile, _player.ToXmlString());

            /*         // Save player data to the database
                     PlayerDataMapper.SaveToDatabase(_player);
                     // Close the window.*/
        }

        /// <summary>
        /// Handles the click event for the Map button and opens the world map screen.
        /// </summary>
        private void btnSaveAndQuit_Click(object sender, RoutedEventArgs e)
        {
            string dataFile = _player.PlayerDataFileName;
            // Save player data to XML file
            File.WriteAllText(dataFile, _player.ToXmlString());

            /*         // Save player data to the database
                     PlayerDataMapper.SaveToDatabase(_player);
                     // Close the window.*/
            Close();
        }
        #endregion

        /// <summary>
        /// Handles the PropertyChanged event of the player and updates UI elements based on the changed property.
        /// </summary>
        private void PlayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Weapons")
            {
                Weapon previouslySelectedWeapon = _player.CurrentWeapon;

                cboWeapons.ItemsSource = _player.Weapons;

                if (previouslySelectedWeapon != null && _player.Weapons.Exists(w => w.ID == previouslySelectedWeapon.ID))
                {
                    cboWeapons.SelectedItem = previouslySelectedWeapon;
                }

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
                Potion previouslySelectedPotion = _player.CurrentPotion;

                cboPotions.ItemsSource = _player.Potions;

                if (previouslySelectedPotion != null && _player.Potions.Exists(w => w.ID == previouslySelectedPotion.ID))
                {
                    cboWeapons.SelectedItem = previouslySelectedPotion;
                }

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
                btnTradeBorder.Visibility = (_player.CurrentLocation.VendorWorkingHere != null) ? Visibility.Visible : Visibility.Hidden;

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


        /// <summary>
        /// Displays a message in the messages text box.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="messageEventArgs">The event arguments containing the message.</param>
        private void HandleMessage(object sender, MessageEventArgs e)
        {
            rtbMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
            rtbMessages.ScrollToEnd();
        }

        #region WindowControls

        /// <summary>
        /// Triggers window movement when the left mouse button is pressed.
        /// </summary>
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
             
            string dataFile = _player.PlayerDataFileName;
            // Save player data to XML file
            File.WriteAllText(dataFile, _player.ToXmlString());

            /*         // Save player data to the database
                     PlayerDataMapper.SaveToDatabase(_player);
                     // Close the window.*/
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