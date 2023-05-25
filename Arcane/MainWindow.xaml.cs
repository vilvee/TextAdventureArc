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

namespace Arcane
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Player _player;
        private Enemy _currentEnemy;

        public MainWindow()
        {
            InitializeComponent();

            _player = new Player(20, 0, 1, 10, 10);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));

            lbHitPoints.Content = _player.CurrentHitPoints.ToString();
            lbGold.Content = _player.Gold.ToString();
            lbExperience.Content = _player.ExperiencePoints.ToString();
            lbLevel.Content = _player.Level.ToString();
        }

        private void btnbtnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void MoveTo(Location newLocation)
        {
            if (newLocation.ItemRequiredToEnter != null)
            {
                // See if the player has the required item in their inventory
                bool playerHasRequiredItem = false;

                foreach (InventoryItem ii in _player.Inventory)
                {
                    if (ii.Detail.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        // We found the required item
                        playerHasRequiredItem = true;
                        break; // Exit out of the foreach loop
                    }
                }


                _player.CurrentLocation = newLocation;
                // Show/hide available movement buttons
                btnNorth.Visibility = (newLocation.LocationToNorth != null) ? Visibility.Visible : Visibility.Collapsed;
                btnEast.Visibility = (newLocation.LocationToEast != null) ? Visibility.Visible : Visibility.Collapsed;
                btnSouth.Visibility = (newLocation.LocationToSouth != null) ? Visibility.Visible : Visibility.Collapsed;
                btnWest.Visibility = (newLocation.LocationToWest != null) ? Visibility.Visible : Visibility.Collapsed;

                // Display current location name and description
                Paragraph paragraph3 = new Paragraph();
                paragraph3.Inlines.Add(new Run(newLocation.Name));
                paragraph3.Inlines.Add(new LineBreak());
                paragraph3.Inlines.Add(new Run(newLocation.Description));

                //Clear the previous content and add new content
                rtbLocation.Document.Blocks.Clear();
                rtbLocation.Document.Blocks.Add(paragraph3);


                _player.CurrentHitPoints = _player.MaximumHitPoints;

                lbHitPoints.Content = _player.CurrentHitPoints.ToString();

                // Does the location have a quest?
                if (newLocation.LevelPresent != null)
                {
                    // See if the player already has the quest, and if they've completed it
                    bool playerAlreadyHasQuest = false;
                    bool playerAlreadyCompletedQuest = false;

                    foreach (Quest playerQuest in _player.Quests)
                    {
                        if (playerQuest.Details.ID == newLocation.LevelPresent.ID)
                        {
                            playerAlreadyHasQuest = true;

                            if (playerQuest.IsCompleted)
                            {
                                playerAlreadyCompletedQuest = true;
                            }
                        }
                    }

                    // See if the player already has the quest
                    if (playerAlreadyHasQuest)
                    {
                        // If the player has not completed the quest yet
                        if (!playerAlreadyCompletedQuest)
                        {
                            // See if the player has all the items needed to complete the quest
                            bool playerHasAllItemsToCompleteQuest = true;

                            foreach (QuestReward qci in newLocation.LevelPresent.QuestReward)
                            {
                                bool foundItemInPlayersInventory = false;

                                // Check each item in the player's inventory, to see if they have it, and enough of it
                                foreach (InventoryItem ii in _player.Inventory)
                                {
                                    // The player has this item in their inventory
                                    if (ii.Detail.ID == qci.Details.ID)
                                    {
                                        foundItemInPlayersInventory = true;

                                        if (ii.Quantity < qci.Quantity)
                                        {
                                            // The player does not have enough of this item to complete the quest
                                            playerHasAllItemsToCompleteQuest = false;

                                            // There is no reason to continue checking for the other quest completion items
                                            break;
                                        }

                                        // We found the item, so don't check the rest of the player's inventory
                                        break;
                                    }
                                }

                                // If we didn't find the required item, set our variable and stop looking for other items
                                if (!foundItemInPlayersInventory)
                                {
                                    // The player does not have this item in their inventory
                                    playerHasAllItemsToCompleteQuest = false;

                                    // There is no reason to continue checking for the other quest completion items
                                    break;
                                }
                            }

                            // The player has all items required to complete the quest
                            if (playerHasAllItemsToCompleteQuest)
                            {
                                // Display message
                                Paragraph paragraph1 = new Paragraph();
                                paragraph1.Inlines.Add(new LineBreak());
                                paragraph1.Inlines.Add(
                                    "You complete the '" + newLocation.LevelPresent.Name + "' quest.");
                                paragraph1.Inlines.Add(new LineBreak());

                                rtbMessages.Document.Blocks.Add(paragraph1);

                                // Remove quest items from inventory
                                foreach (QuestReward qci in newLocation.LevelPresent.QuestReward)
                                {
                                    foreach (InventoryItem ii in _player.Inventory)
                                    {
                                        if (ii.Detail.ID == qci.Details.ID)
                                        {
                                            // Subtract the quantity from the player's inventory that was needed to complete the quest
                                            ii.Quantity -= qci.Quantity;
                                            break;
                                        }
                                    }
                                }
                            }

                            // Give quest rewards
                            Paragraph rewardsParagraph = new Paragraph();
                            rewardsParagraph.Inlines.Add("You receive: ");
                            rewardsParagraph.Inlines.Add(new LineBreak());
                            rewardsParagraph.Inlines.Add(newLocation.LevelPresent.RewardExperiencePoints.ToString() +
                                                         " experience points");
                            rewardsParagraph.Inlines.Add(new LineBreak());
                            rewardsParagraph.Inlines.Add(newLocation.LevelPresent.RewardGold.ToString() + " gold");
                            rewardsParagraph.Inlines.Add(new LineBreak());
                            rewardsParagraph.Inlines.Add(newLocation.LevelPresent.RewardItem.Name);
                            rewardsParagraph.Inlines.Add(new LineBreak());
                            rewardsParagraph.Inlines.Add(new LineBreak());

                            rtbMessages.Document.Blocks.Add(rewardsParagraph);


                            _player.ExperiencePoints += newLocation.LevelPresent.RewardExperiencePoints;
                            _player.Gold += newLocation.LevelPresent.RewardGold;

                            // Add the reward item to the player's inventory
                            bool addedItemToPlayerInventory = false;

                            foreach (InventoryItem ii in _player.Inventory)
                            {
                                if (ii.Detail.ID == newLocation.LevelPresent.RewardItem.ID)
                                {
                                    // They have the item in their inventory, so increase the quantity by one
                                    ii.Quantity++;

                                    addedItemToPlayerInventory = true;

                                    break;
                                }
                            }

                            // They didn't have the item, so add it to their inventory, with a quantity of 1
                            if (!addedItemToPlayerInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.LevelPresent.RewardItem,
                                    1));
                            }

                            // Mark the quest as completed
                            // Find the quest in the player's quest list
                            foreach (Quest pq in _player.Quests)
                            {
                                if (pq.Details.ID == newLocation.LevelPresent.ID)
                                {
                                    // Mark it as completed
                                    pq.IsCompleted = true;

                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // The player does not already have the quest

                    // Display the messages
                    Paragraph paragraph = new Paragraph();
                    paragraph.Inlines.Add("You receive the " + newLocation.LevelPresent.Name + " quest.");
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(newLocation.LevelPresent.Description);
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add("To complete it, return with:");
                    paragraph.Inlines.Add(new LineBreak());
                    foreach (QuestReward qci in newLocation.LevelPresent.QuestReward)
                    {
                        if (qci.Quantity == 1)
                        {
                            paragraph.Inlines.Add(qci.Quantity.ToString() + " " + qci.Details.Name);
                            paragraph.Inlines.Add(new LineBreak());
                        }
                        else
                        {
                            paragraph.Inlines.Add(qci.Quantity.ToString() + " " + qci.Details.NamePlural);
                            paragraph.Inlines.Add(new LineBreak());
                        }
                    }

                    paragraph.Inlines.Add(new LineBreak());

                    rtbMessages.Document.Blocks.Add(paragraph);

                    // Add the quest to the player's quest list
                    _player.Quests.Add(new Quest(newLocation.LevelPresent));

                    // Does the location have a monster?
                    if (newLocation.EnemyPresent != null)
                    {
                        Paragraph enemyParagraph = new Paragraph();
                        enemyParagraph.Inlines.Add("You see a " + newLocation.EnemyPresent.Name);
                        enemyParagraph.Inlines.Add(new LineBreak());

                        rtbMessages.Document.Blocks.Add(enemyParagraph);

                        // Make a new monster, using the values from the standard monster in the World.Enemy list
                        Enemy standardEnemy = World.EnemyByID(newLocation.EnemyPresent.ID);

                        _currentEnemy = new Enemy(standardEnemy.ID, standardEnemy.Name,
                            standardEnemy.MaximumDamage,
                            standardEnemy.RewardExperiencePoints, standardEnemy.RewardGold,
                            standardEnemy.CurrentHitPoints, standardEnemy.MaximumHitPoints);



                        foreach (Loot lootItem in standardEnemy.LootTable)
                        {
                            _currentEnemy.LootTable.Add(lootItem);
                        }
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
                    dgvInventory.HeadersVisibility = DataGridHeadersVisibility.Column;

                    dgvInventory.Columns.Clear();
                    dgvInventory.Columns.Add(new DataGridTextColumn
                        { Header = "Name", Width = new DataGridLength(197), Binding = new Binding("Name") });
                    dgvInventory.Columns.Add(new DataGridTextColumn
                        { Header = "Quantity", Binding = new Binding("Quantity") });

                    dgvInventory.Items.Clear();


                    // Refresh player's inventory list
                    dgvInventory.HeadersVisibility = DataGridHeadersVisibility.Column;

                    dgvInventory.Columns.Clear();
                    dgvInventory.Columns.Add(new DataGridTextColumn
                        { Header = "Name", Width = new DataGridLength(197), Binding = new Binding("Detail.Name") });
                    dgvInventory.Columns.Add(new DataGridTextColumn
                        { Header = "Quantity", Binding = new Binding("Quantity") });

                    dgvInventory.Items.Clear();

                    foreach (InventoryItem inventoryItem in _player.Inventory)
                    {
                        if (inventoryItem.Quantity > 0)
                        {
                            dgvInventory.Items.Add(inventoryItem);
                        }
                    }

                    // Refresh player's quest list
                    dgvQuests.HeadersVisibility = DataGridHeadersVisibility.Column;

                    dgvQuests.Columns.Clear();
                    dgvQuests.Columns.Add(new DataGridTextColumn
                        { Header = "Name", Width = new DataGridLength(197), Binding = new Binding("Details.Name") });
                    dgvQuests.Columns.Add(new DataGridTextColumn
                        { Header = "Done?", Binding = new Binding("IsCompleted") });

                    dgvQuests.Items.Clear();

                    foreach (Quest playerQuest in _player.Quests)
                    {
                        dgvQuests.Items.Add(playerQuest);
                    }


                    // Refresh player's weapons combobox
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
                        cboWeapons.Visibility = Visibility.Hidden;
                        btnUseWeapon.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        cboWeapons.ItemsSource = weapons;
                        var selectedWeaponID = cboWeapons.SelectedValue;
                        cboWeapons.SelectedIndex = 0;
                    }

                    // Refresh player's potions combobox
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
                        cboPotions.Visibility = Visibility.Hidden;
                        btnUsePotion.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        cboPotions.ItemsSource = healingPotions;
                        var selectedPotionID = cboPotions.SelectedValue;

                        cboPotions.SelectedIndex = 0;
                    }
                }

            }
        }

        private void btnUseWeapon_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
