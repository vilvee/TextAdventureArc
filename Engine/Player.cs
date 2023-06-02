using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Xml;
using System.Xml.Linq;


namespace Engine
{


    /// <summary>
    /// Represents the player in the game.
    /// </summary>
    public class Player : Entity
    {
        private int _gold;
        private int _experiencePoints;
        private Location _currentLocation;
        private string _characterName;
        private string _imagePath;

        public string PlayerData { get; set; }

        public string CharacterImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged("CharacterImagePath");
            }
        }


    public string CharacterName
    {
            get => _characterName;
            set
            {
                _characterName = value;
                OnPropertyChanged("CharacterName");
            }
        }

        /// <summary>
        /// Gets or sets the amount of gold the player has.
        /// </summary>
        public int Gold
        {
            get => _gold;
            set
            {
                _gold = value;
                OnPropertyChanged("Gold");
            }
        }

        /// <summary>
        /// Gets or sets the experience points of the player.
        /// </summary>
        public int ExperiencePoints
        {
            get => _experiencePoints;
            private set
            {
                _experiencePoints = value;
                OnPropertyChanged("ExperiencePoints");
                OnPropertyChanged("Quest");
            }
        }

        /// <summary>
        /// Gets the level of the player based on the experience points.
        /// </summary>
        public int Quest => ((ExperiencePoints / 100) + 1);

        /// <summary>
        /// Gets or sets the current location of the player.
        /// </summary>
        public Location CurrentLocation
        {
            get => _currentLocation;
            set
            {
                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }

        /// <summary>
        /// Gets or sets the current weapon of the player.
        /// </summary>
        public Weapon CurrentWeapon { get; set; }

        /// <summary>
        /// Gets or sets the current weapon of the player.
        /// </summary>
        public Potion CurrentPotion { get; set; }

       /// <summary>
        /// Gets the inventory of the player.
        /// </summary>
        public BindingList<Inventory> Inventory { get; set; }

        /// <summary>
        /// Gets the weapons in the player's inventory.
        /// </summary>
        public List<Weapon> Weapons
        {
            get { return Inventory.Where(x => x.Detail is Weapon).Select(x => x.Detail as Weapon).ToList(); }
        }

        /// <summary>
        /// Gets the healing potions in the player's inventory.
        /// </summary>
        public List<Potion> Potions
        {
            get { return Inventory.Where(x => x.Detail is Potion).Select(x => x.Detail as Potion).ToList(); }
        }

        /// <summary>
        /// Gets the quests of the player.
        /// </summary>
        public BindingList<ActiveQuest> Quests { get; set; }

        /// <summary>
        /// Gets or sets the locations visited by the player.
        /// </summary>
        public List<int> LocationsVisited { get; set; }


        private Player(string name, string imagePath, int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints)
        {
            CharacterName = name;
            CharacterImagePath = imagePath;
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Inventory = new BindingList<Inventory>();
            Quests = new BindingList<ActiveQuest>();
            LocationsVisited = new List<int>();
            PlayerData = CreatePlayerDataFile();

        }

        public string CreatePlayerDataFile()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = $"PlayerData{CharacterName}.xml";
            string filePath = Path.Combine(baseDirectory, fileName);

            // Create the player data file using the filePath

            return filePath;
        }

        /// <summary>
        /// Creates a default player.
        /// </summary>
        /// <returns>The default player instance.</returns>
        public static Player CreateDefaultPlayer(string name, string imagePath)
        {
            Player player = new Player(name, imagePath,10, 10, 20, 0);
            player.Inventory.Add(new Inventory(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);
            return player;
        }
/*
        /// <summary>
        /// Creates a Player object from database values.
        /// </summary>
        /// <param name="currentHitPoints">The current hit points of the player.</param>
        /// <param name="maximumHitPoints">The maximum hit points of the player.</param>
        /// <param name="gold">The amount of gold the player has.</param>
        /// <param name="experiencePoints">The experience points of the player.</param>
        /// <param name="currentLocationID">The ID of the current location of the player.</param>
        /// <returns>The Player object created from the database values.</returns>
        public static Player CreatePlayerFromDatabase(string name, int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints, int currentLocationID)
        {
            Player player = new Player(name, currentHitPoints, maximumHitPoints, gold, experiencePoints);
            Navigation.MoveTo(player,World.LocationByID(currentLocationID));
            return player;
        }*/

        /// <summary>
        /// Raises the inventory changed event for a specific item type.
        /// </summary>
        /// <param name="item">The item that caused the inventory change.</param>
        public void RaiseInventoryChangedEvent(Item item)
        {
            if (item is Weapon)
            {
                OnPropertyChanged("Weapons");
            }

            if (item is HealingPotion)
            {
                OnPropertyChanged("Potions");
            }
        }

        /// <summary>
        /// Adds an item to the player's inventory with the specified quantity.
        /// </summary>
        /// <param name="itemToAdd">The item to add to the inventory.</param>
        /// <param name="quantity">The quantity of the item to add (default: 1).</param>
        public void AddItemToInventory(Item itemToAdd, int quantity = 1)
        {
            Inventory existingInInventory = Inventory.SingleOrDefault(ii => ii.Detail.ID == itemToAdd.ID);

            if (existingInInventory == null)
            {
                Inventory.Add(new Inventory(itemToAdd, quantity));
            }
            else
            {
                existingInInventory.Quantity += quantity;
            }

            RaiseInventoryChangedEvent(itemToAdd);
        }

        /// <summary>
        /// Removes an item from the player's inventory with the specified quantity.
        /// </summary>
        /// <param name="itemToRemove">The item to remove from the inventory.</param>
        /// <param name="quantity">The quantity of the item to remove (default: 1).</param>
        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            Inventory item = Inventory.SingleOrDefault(ii => ii.Detail.ID == itemToRemove.ID && ii.Quantity >= quantity);

            if (item != null)
            {
                item.Quantity -= quantity;

                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }

                RaiseInventoryChangedEvent(itemToRemove);
            }
        }

        /// <summary>
        /// Adds experience points to the player and updates the maximum hit points based on the player's level.
        /// </summary>
        /// <param name="experiencePointsToAdd">The amount of experience points to add.</param>
        public void AddExperiencePoints(int experiencePointsToAdd)
        {
            ExperiencePoints += experiencePointsToAdd;
            MaximumHitPoints = (Quest * 10);
        }

        // SERIALIZATION?

        /// <summary>
        /// Converts the player data to an XML string.
        /// </summary>
        /// <returns>The player data represented as an XML string.</returns>
        public string ToXmlString()
        {
            XmlDocument playerData = new XmlDocument();

            // Create the top-level XML node
            XmlNode player = playerData.CreateElement("Player");
            playerData.AppendChild(player);

            // Create the "Stats" child node to hold the other player statistics nodes
            XmlNode stats = playerData.CreateElement("Stats");
            player.AppendChild(stats);

            // Create the child nodes for the "Stats" node
            CreateNewChildXmlNode(playerData, stats, "CharacterImagePath", CharacterImagePath);
            CreateNewChildXmlNode(playerData, stats, "CharacterName", CharacterName);
            CreateNewChildXmlNode(playerData, stats, "CurrentHitPoints", CurrentHitPoints);
            CreateNewChildXmlNode(playerData, stats, "MaximumHitPoints", MaximumHitPoints);
            CreateNewChildXmlNode(playerData, stats, "Gold", Gold);
            CreateNewChildXmlNode(playerData, stats, "ExperiencePoints", ExperiencePoints);
            CreateNewChildXmlNode(playerData, stats, "CurrentLocation", CurrentLocation.ID);

            if (CurrentWeapon != null)
            {
                CreateNewChildXmlNode(playerData, stats, "CurrentWeapon", CurrentWeapon.ID);
            }

            if (CurrentPotion != null)
            {
                CreateNewChildXmlNode(playerData, stats, "CurrentPotion", CurrentPotion.ID);
            }

            // Create the "LocationsVisited" child node to hold each LocationVisited node
            XmlNode locationsVisited = playerData.CreateElement("LocationsVisited");
            player.AppendChild(locationsVisited);

            // Create an "LocationVisited" node for each item in the player's visited locations
            foreach (int locationID in LocationsVisited)
            {
                XmlNode locationVisited = playerData.CreateElement("LocationVisited");
                AddXmlAttributeToNode(playerData, locationVisited, "ID", locationID);
                locationsVisited.AppendChild(locationVisited);
            }

            // Create the "InventoryItems" child node to hold each Inventory node
            XmlNode inventoryItems = playerData.CreateElement("InventoryItems");
            player.AppendChild(inventoryItems);

            // Create an "Inventory" node for each item in the player's inventory
            foreach (Inventory item in Inventory)
            {
                XmlNode inventoryItem = playerData.CreateElement("Inventory");
                AddXmlAttributeToNode(playerData, inventoryItem, "ID", item.Detail.ID);
                AddXmlAttributeToNode(playerData, inventoryItem, "Quantity", item.Quantity);
                inventoryItems.AppendChild(inventoryItem);
            }

            // Create the "PlayerQuests" child node to hold each PlayerQuest node
            XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
            player.AppendChild(playerQuests);

            // Create a "PlayerQuest" node for each quest the player has acquired
            foreach (ActiveQuest quest in Quests)
            {
                XmlNode playerQuest = playerData.CreateElement("PlayerQuest");
                AddXmlAttributeToNode(playerData, playerQuest, "ID", quest.Details.ID);
                AddXmlAttributeToNode(playerData, playerQuest, "IsCompleted", quest.IsCompleted);
                playerQuests.AppendChild(playerQuest);
            }

            return playerData.InnerXml; // The XML document, as a string, so we can save the data to disk
        }


        /// <summary>
        /// Creates a new child XML node with the specified element name and value, and appends it to the parent node.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="parentNode">The parent XML node.</param>
        /// <param name="elementName">The name of the new child element.</param>
        /// <param name="value">The value of the new child element.</param>
        private void CreateNewChildXmlNode(XmlDocument document, XmlNode parentNode, string elementName, object value)
        {
            XmlNode node = document.CreateElement(elementName);
            node.AppendChild(document.CreateTextNode(value.ToString()));
            parentNode.AppendChild(node);
        }

        /// <summary>
            /// Adds an XML attribute with the specified attribute name and value to the XML node.
            /// </summary>
            /// <param name="document">The XML document.</param>
            /// <param name="node">The XML node.</param>
            /// <param name="attributeName">The name of the attribute.</param>
            /// <param name="value">The value of the attribute.</param>
            private void AddXmlAttributeToNode(XmlDocument document, XmlNode node, string attributeName, object value)
        {
            XmlAttribute attribute = document.CreateAttribute(attributeName);
            attribute.Value = value.ToString();
            node.Attributes.Append(attribute);
        }

        /// <summary>
        /// Creates a Player object from an XML string.
        /// </summary>
        /// <param name="xmlPlayerData">The XML string containing player data.</param>
        /// <returns>The Player object created from the XML string, or a default player object if there was an error with the XML data.</returns>
        public static Player CreatePlayerFromXmlString(string xmlPlayerData)
        {
            try
            {
                XmlDocument playerData = new XmlDocument();
                playerData.LoadXml(xmlPlayerData);


                string characterImagePath = playerData.SelectSingleNode($"/Player/Stats/CharacterImagePath")?.InnerText;
                string characterName = playerData.SelectSingleNode($"/Player/Stats/CharacterName")?.InnerText;
                int currentHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentHitPoints").InnerText);
                int maximumHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/MaximumHitPoints").InnerText);
                int gold = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/Gold").InnerText);
                int experiencePoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/ExperiencePoints").InnerText);

                Player player = new Player(characterName, characterImagePath,  currentHitPoints, maximumHitPoints, gold, experiencePoints);

                int currentLocationID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentLocation").InnerText);
                player.CurrentLocation = World.LocationByID(currentLocationID);

                if (playerData.SelectSingleNode("/Player/Stats/CurrentWeapon") != null)
                {
                    int currentWeaponID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentWeapon").InnerText);
                    player.CurrentWeapon = (Weapon)World.ItemByID(currentWeaponID);
                }

                if (playerData.SelectSingleNode("/Player/Stats/CurrentPotion") != null)
                {
                    int currentPotionID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentPotion").InnerText);
                    player.CurrentPotion = (Potion)World.ItemByID(currentPotionID);
                }

                foreach (XmlNode node in playerData.SelectNodes("/Player/LocationsVisited/LocationVisited"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    player.LocationsVisited.Add(id);
                }

                foreach (XmlNode node in playerData.SelectNodes("/Player/InventoryItems/Inventory"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    int quantity = Convert.ToInt32(node.Attributes["Quantity"].Value);

                    for (int i = 0; i < quantity; i++)
                    {
                        player.AddItemToInventory(World.ItemByID(id));
                    }
                }

                foreach (XmlNode node in playerData.SelectNodes("/Player/PlayerQuests/PlayerQuest"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    bool isCompleted = Convert.ToBoolean(node.Attributes["IsCompleted"].Value);

                    ActiveQuest playerActiveQuest = new ActiveQuest(World.QuestByID(id));
                    playerActiveQuest.IsCompleted = isCompleted;
                    player.Quests.Add(playerActiveQuest);
                }

                return player;
            }
            catch
            {
                /*// If there was an error with the XML data, return a default player object
                return Player.CreateDefaultPlayer();*/
                return null;
            }
        }

        // END SERIALIZATION?




    }
}
