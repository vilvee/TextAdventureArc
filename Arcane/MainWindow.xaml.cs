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

namespace Arcane
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string PLAYER_DATA_FILE_NAME = "PlayerData1.xml";

        private Player _player;
        private Enemy? _currentEnemy;


        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                _player = Player.CreateDefaultPlayer();
            }

            lbHitPoints.SetBinding(ContentProperty, new Binding("CurrentHitPoints") { Source = _player });
            lbGold.SetBinding(ContentProperty, new Binding("Gold") { Source = _player });
            lbExperience.SetBinding(ContentProperty, new Binding("ExperiencePoints") { Source = _player });
            lbLevel.SetBinding(ContentProperty, new Binding("Level") { Source = _player });
            MoveTo(_player.CurrentLocation);
        }

        #region Directions

        private void btnNorth_Click(object sender, RoutedEventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void btnEast_Click(object sender, RoutedEventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void btnSouth_Click(object sender, RoutedEventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        private void btnbtnWest_Click(object sender, RoutedEventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }
        #endregion

        private void MoveTo(Location newLocation)
        {
            //Does the location have any required items
            if (!_player.HasRequiredItemToEnterThisLocation(newLocation))
            {
                rtbMessages.AppendText("You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine);
                return;
            }

            // Update the player's current location
            _player.CurrentLocation = newLocation;

            // Show/hide available movement buttons
            btnNorth.Visibility = (newLocation.LocationToNorth != null) ? Visibility.Visible : Visibility.Hidden;
            btnEast.Visibility = (newLocation.LocationToEast != null) ? Visibility.Visible : Visibility.Hidden;
            btnSouth.Visibility = (newLocation.LocationToSouth != null) ? Visibility.Visible : Visibility.Hidden;
            btnWest.Visibility = (newLocation.LocationToWest != null) ? Visibility.Visible : Visibility.Hidden;

            // Display current location name and description

            rtbLocation.Document.Blocks.Clear();
            rtbLocation.Document.Blocks.Add(new Paragraph(new Run(newLocation.Name)));
            rtbLocation.AppendText(Environment.NewLine);
            rtbLocation.AppendText(newLocation.Description + Environment.NewLine);

            // Completely heal the player
            _player.CurrentHitPoints = _player.MaximumHitPoints;

            // Does the location have a quest?
            if (newLocation.LevelPresent != null)
            {
                // See if the player already has the quest, and if they've completed it
                bool playerAlreadyHasQuest = _player.HasThisQuest(newLocation.LevelPresent);
                bool playerAlreadyCompletedQuest = _player.CompletedThisQuest(newLocation.LevelPresent);

                // See if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                    // If the player has not completed the quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        // See if the player has all the items needed to complete the quest
                        bool playerHasAllItemsToCompleteQuest = _player.HasAllQuestCompletionItems(newLocation.LevelPresent);

                        // The player has all items required to complete the quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            // Display message
                            rtbMessages.AppendText(Environment.NewLine);
                            rtbMessages.AppendText("You complete the '" + newLocation.LevelPresent.Name + "' quest." + Environment.NewLine);

                            // Remove quest items from inventory
                            _player.RemoveQuestCompletionItems(newLocation.LevelPresent);

                            // Give quest rewards
                            rtbMessages.AppendText("You receive: " + Environment.NewLine);
                            rtbMessages.AppendText(newLocation.LevelPresent.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine);
                            rtbMessages.AppendText(newLocation.LevelPresent.RewardGold.ToString() + " gold" + Environment.NewLine);
                            rtbMessages.AppendText(newLocation.LevelPresent.RewardItem.Name + Environment.NewLine);
                            rtbMessages.AppendText(Environment.NewLine);

                            _player.AddExperiencePoints(newLocation.LevelPresent.RewardExperiencePoints);
                            _player.Gold += newLocation.LevelPresent.RewardGold;

                            // Add the reward item to the player's inventory
                            _player.AddItemToInventory(newLocation.LevelPresent.RewardItem);

                            // Mark the quest as completed
                            _player.MarkQuestCompleted(newLocation.LevelPresent);
                        }
                    }
                }
                else
                {
                    // The player does not already have the quest

                    // Display the messages
                    rtbMessages.AppendText("You receive the " + newLocation.LevelPresent.Name + " quest." + Environment.NewLine);
                    rtbMessages.AppendText(newLocation.LevelPresent.Description + Environment.NewLine);
                    rtbMessages.AppendText("To complete it, return with:" + Environment.NewLine);
                    foreach (QuestReward qci in newLocation.LevelPresent.QuestReward)
                    {
                        if (qci.Quantity == 1)
                        {
                            rtbMessages.AppendText(qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine);
                        }
                        else
                        {
                            rtbMessages.AppendText(qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine);
                        }
                    }
                    rtbMessages.AppendText(Environment.NewLine);

                    // Add the quest to the player's quest list
                    _player.Quests.Add(new Quest(newLocation.LevelPresent));
                }
            }

            // Does the location have a monster?
            if (newLocation.EnemyPresent != null)
            {
                rtbMessages.AppendText("You see a " + newLocation.EnemyPresent.Name + Environment.NewLine);

                // Make a new monster, using the values from the standard monster in the World.Monster list
                Enemy standardMonster = World.EnemyByID(newLocation.EnemyPresent.ID);

                _currentEnemy = new Enemy(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                    standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

                foreach (Loot lootItem in standardMonster.LootTable)
                {
                    _currentEnemy.LootTable.Add(lootItem);
                }

                cboWeapons.Visibility = Visibility.Visible;
                cboPotions.Visibility = Visibility.Visible;
                btnUseWeapon.Visibility = Visibility.Visible;
                btnUsePotion.Visibility = Visibility.Visible;
            }
            else
            {
                _currentEnemy = null;

                cboWeapons.Visibility = Visibility.Collapsed;
                cboPotions.Visibility = Visibility.Collapsed;
                btnUseWeapon.Visibility = Visibility.Collapsed;
                btnUsePotion.Visibility = Visibility.Collapsed;
            }

            // Refresh player's inventory list
            UpdateInventoryListInUI();

            // Refresh player's quest list
            UpdateQuestListInUI();

            // Refresh player's weapons combobox
            UpdateWeaponListInUI();

            // Refresh player's potions combobox
            UpdatePotionListInUI();
        }

        private void UpdateInventoryListInUI()
        {
            dgvInventory.AutoGenerateColumns = false;
            dgvInventory.Columns.Clear();

            // Create column for Name
            var nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Name";
            nameColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            nameColumn.Binding = new Binding("Detail.Name");
            dgvInventory.Columns.Add(nameColumn);

            // Create column for Quantity
            var quantityColumn = new DataGridTextColumn();
            quantityColumn.Header = "Quantity";
            quantityColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            quantityColumn.Binding = new Binding("Quantity");
            dgvInventory.Columns.Add(quantityColumn);

            dgvInventory.ItemsSource = _player.Inventory.Where(item => item.Quantity > 0);
        }



        private void UpdateQuestListInUI()
        {
            dgvQuests.AutoGenerateColumns = false;
            dgvQuests.Columns.Clear();

            // Create column for Name
            var nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Name";
            nameColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            nameColumn.Binding = new Binding("Details.Name");
            dgvQuests.Columns.Add(nameColumn);

            // Create column for Done?
            var doneColumn = new DataGridTextColumn();
            doneColumn.Header = "Done?";
            doneColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            doneColumn.Binding = new Binding("IsCompleted");
            dgvQuests.Columns.Add(doneColumn);

            dgvQuests.ItemsSource = _player.Quests;
        }

        private void UpdateWeaponListInUI()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Detail is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Detail);
                    }
                }
            }


            if (weapons.Count == 0)
            {
                // The player doesn't have any weapons, so hide the weapon combobox and "Use" button
                cboWeapons.Visibility = Visibility.Collapsed;
                btnUseWeapon.Visibility = Visibility.Collapsed;
            }
            else
            {
                cboWeapons.SelectionChanged -= cboWeapons_SelectionChanged;
                cboWeapons.ItemsSource = weapons;
                cboWeapons.SelectionChanged += cboWeapons_SelectionChanged;
                cboWeapons.DisplayMemberPath = "Name";
                cboWeapons.SelectedValuePath = "ID";

                if (_player.CurrentWeapon != null)
                {
                    cboWeapons.SelectedItem = _player.CurrentWeapon;
                }
                else
                {
                    cboWeapons.SelectedIndex = 0;
                }
            }
        }

        private void UpdatePotionListInUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Detail is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Detail);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                // The player doesn't have any potions, so hide the potion combobox and "Use" button
                cboPotions.Visibility = Visibility.Collapsed;
                btnUsePotion.Visibility = Visibility.Collapsed;
            }
            else
            {
                cboPotions.ItemsSource = healingPotions;
                cboPotions.DisplayMemberPath = "Name";
                cboPotions.SelectedValuePath = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }

        private void btnUseWeapon_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected weapon from the cboWeapons ComboBox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            // Determine the amount of damage to do to the monster
            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);

            // Apply the damage to the monster's CurrentHitPoints
            _currentEnemy.CurrentHitPoints -= damageToMonster;

            // Display message
            rtbMessages.Document.Blocks.Add(new Paragraph(new Run("You hit the " + _currentEnemy.Name + " for " + damageToMonster.ToString() + " points.")));
            rtbMessages.AppendText(Environment.NewLine);

            // Check if the monster is dead
            if (_currentEnemy.CurrentHitPoints <= 0)
            {
                // Monster is dead
                rtbMessages.AppendText(Environment.NewLine);
                rtbMessages.AppendText("You defeated the " + _currentEnemy.Name + Environment.NewLine);

                // Give player experience points for killing the monster
                _player.AddExperiencePoints(_currentEnemy.RewardExperiencePoints);
                rtbMessages.AppendText("You receive " + _currentEnemy.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine);

                // Give player gold for killing the monster
                _player.Gold += _currentEnemy.RewardGold;
                rtbMessages.AppendText("You receive " + _currentEnemy.RewardGold.ToString() + " gold" + Environment.NewLine);

                // Get random loot items from the monster
                List<InventoryItem> lootedItems = new List<InventoryItem>();

                // Add items to the lootedItems list, comparing a random number to the drop percentage
                foreach (Loot lootItem in _currentEnemy.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }

                // If no items were randomly selected, then add the default loot item(s).
                if (lootedItems.Count == 0)
                {
                    foreach (Loot lootItem in _currentEnemy.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }

                // Add the looted items to the player's inventory
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    _player.AddItemToInventory(inventoryItem.Detail);

                    if (inventoryItem.Quantity == 1)
                    {
                        rtbMessages.AppendText("You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Detail.Name + Environment.NewLine);
                    }
                    else
                    {
                        rtbMessages.AppendText("You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Detail.NamePlural + Environment.NewLine);
                    }
                }

                // Refresh player information and inventory controls
                lbGold.Content = _player.Gold.ToString();
                lbExperience.Content = _player.ExperiencePoints.ToString();
                lbLevel.Content = _player.Level.ToString();

                UpdateInventoryListInUI();
                UpdateWeaponListInUI();
                UpdatePotionListInUI();

                // Add a blank line to the messages box, just for appearance.
                rtbMessages.AppendText(Environment.NewLine); 

                // Move player to current location (to heal player and create a new monster to fight)
                MoveTo(_player.CurrentLocation);
            }
            else
            {
                // Monster is still alive

                // Determine the amount of damage the monster does to the player
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentEnemy.MaximumDamage);

                // Display message
                rtbMessages.AppendText("The " + _currentEnemy.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine);

                // Subtract damage from player
                _player.CurrentHitPoints -= damageToPlayer;

                if (_player.CurrentHitPoints <= 0)
                {
                    // Display message
                    rtbMessages.AppendText("The " + _currentEnemy.Name + " killed you." + Environment.NewLine);

                    // Move player to "Home"
                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
            }
        }

        private void btnUsePotion_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;
            // Add healing amount to the player's current hit points
            _player.CurrentHitPoints = (_player.CurrentHitPoints + potion.AmountToHeal);
            // CurrentHitPoints cannot exceed player's MaximumHitPoints
            if (_player.CurrentHitPoints > _player.MaximumHitPoints)
            {
                _player.CurrentHitPoints = _player.MaximumHitPoints;
            }
            // Remove the potion from the player's inventory
            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Detail.ID == potion.ID)
                {
                    ii.Quantity--;
                    break;
                }
            }
            // Display message
            rtbMessages.AppendText("You drink a " + potion.Name + Environment.NewLine);
            // Monster gets their turn to attack
            // Determine the amount of damage the monster does to the player
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentEnemy.MaximumDamage);
            // Display message
            rtbMessages.AppendText("The " + _currentEnemy.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine);
            // Subtract damage from player
            _player.CurrentHitPoints -= damageToPlayer;
            if (_player.CurrentHitPoints <= 0)
            {
                // Display message
                rtbMessages.AppendText("The " + _currentEnemy.Name + " killed you." + Environment.NewLine);
                // Move player to "Home"
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }
            UpdateInventoryListInUI();
            UpdatePotionListInUI();
           
        }

        private void rtbMessages_TextChanged(object sender, TextChangedEventArgs e)
        {
            // scroll it automatically
            rtbMessages.ScrollToEnd();
        }

        private void MyGame_Closed(object sender, EventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
        }


       private void cboWeapons_SelectionChanged(object sender, SelectionChangedEventArgs e)
              {
                  _player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
              }
    }

}
