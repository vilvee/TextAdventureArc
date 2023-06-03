using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    public static class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Enemy> Enemies = new List<Enemy>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();
        

        public const int UNSELLABLE_ITEM_PRICE = -1;

        // Item IDs
        public const int ITEM_ID_RUSTY_SWORD = 1;
        public const int ITEM_ID_RAT_TAIL = 2;
        public const int ITEM_ID_PIECE_OF_FUR = 3;
        public const int ITEM_ID_SNAKE_FANG = 4;
        public const int ITEM_ID_SNAKESKIN = 5;
        public const int ITEM_ID_CLUB = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_SPIDER_FANG = 8;
        public const int ITEM_ID_SPIDER_SILK = 9;
        public const int ITEM_ID_ADVENTURER_PASS = 10;
        public const int ITEM_ID_MIGHT_POTION = 11;

        // Enemy IDs
        public const int Enemy_ID_RAT = 1;
        public const int Enemy_ID_SNAKE = 2;
        public const int Enemy_ID_GIANT_SPIDER = 3;

        // Quest IDs
        public const int Quest_ID_CLEAR_ALCHEMIST_GARDEN = 1;
        public const int Quest_ID_CLEAR_FARMERS_FIELD = 2;

        // Location IDs
        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_ALCHEMISTS_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;

        // NPC IDs
        public const int LUCKY_ID = 1;

        static World()
        {
            PopulateItems(); // Populates the list of items.
            PopulateEnemies(); // Populates the list of enemies.
            PopulateQuests(); // Populates the list of levels.
            PopulateLocations(); // Populates the list of locations.
        }

        private static void PopulateItems()
        {
            //Weapons
            Items.Add(new Weapon(ITEM_ID_RUSTY_SWORD, "Rusty sword", "Rusty swords", 0, 5, 5));
            Items.Add(new Weapon(ITEM_ID_CLUB, "Club", "Clubs", 3, 10, 8));

            //Potions
            Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "Healing potion", "Healing potions", 5, 3));
            Items.Add(new MightPotion(ITEM_ID_MIGHT_POTION, "Might potion", "Might potions", 2, 3));


            //Loot
            Items.Add(new Item(ITEM_ID_RAT_TAIL, "Rat tail", "Rat tails", 1));
            Items.Add(new Item(ITEM_ID_PIECE_OF_FUR, "Piece of fur", "Pieces of fur", 1));
            Items.Add(new Item(ITEM_ID_SNAKE_FANG, "Snake fang", "Snake fangs", 1));
            Items.Add(new Item(ITEM_ID_SNAKESKIN, "Snakeskin", "Snakeskins", 2));
            Items.Add(new Item(ITEM_ID_SPIDER_FANG, "Spider fang", "Spider fangs", 1));
            Items.Add(new Item(ITEM_ID_SPIDER_SILK, "Spider silk", "Spider silks", 1));
            Items.Add(new Item(ITEM_ID_ADVENTURER_PASS, "Adventurer pass", "Adventurer passes", UNSELLABLE_ITEM_PRICE));
        }

        /// <summary>
        /// Populates the list of enemies.
        /// </summary>
        private static void PopulateEnemies()
        {
            // Create an enemy instance for a rat
            Enemy rat = new Enemy(Enemy_ID_RAT, "Rat", 5, 3, 10, 3, 3);

            // Add loot table entries for the rat enemy
            rat.LootTable.Add(new Loot(ItemByID(ITEM_ID_RAT_TAIL), 75, false));
            rat.LootTable.Add(new Loot(ItemByID(ITEM_ID_PIECE_OF_FUR), 75, true));

            // Create an enemy instance for a snake
            Enemy snake = new Enemy(Enemy_ID_SNAKE, "Snake", 5, 3, 10, 3, 3);

            // Add loot table entries for the snake enemy
            snake.LootTable.Add(new Loot(ItemByID(ITEM_ID_SNAKE_FANG), 75, false));
            snake.LootTable.Add(new Loot(ItemByID(ITEM_ID_SNAKESKIN), 75, true));

            // Create an enemy instance for a giant spider
            Enemy giantSpider = new Enemy(Enemy_ID_GIANT_SPIDER, "Giant spider", 20, 5, 40, 10, 10);

            // Add loot table entries for the giant spider enemy
            giantSpider.LootTable.Add(new Loot(ItemByID(ITEM_ID_SPIDER_FANG), 75, true));
            giantSpider.LootTable.Add(new Loot(ItemByID(ITEM_ID_SPIDER_SILK), 25, false));

            // Add the enemies to the Enemies list
            Enemies.Add(rat);
            Enemies.Add(snake);
            Enemies.Add(giantSpider);
        }


        /// <summary>
        /// Populates the list of levels.
        /// </summary>
        private static void PopulateQuests()
        {
            // Create a level instance for clearing the alchemist's garden
            Quest clearAlchemistGarden =
                new Quest(
                    Quest_ID_CLEAR_ALCHEMIST_GARDEN,
                    "Clear the alchemist's garden",
                    "Kill rats in the alchemist's garden and bring back 3 rat tails. You will receive a healing potion and 10 gold pieces.", 20, 10);

            // Add quest reward for collecting rat tails
            clearAlchemistGarden.QuestReward.Add(new QuestReward(ItemByID(ITEM_ID_RAT_TAIL), 3));

            // Set the reward item for clearing the alchemist's garden
            clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);

            // Create a level instance for clearing the farmer's field
            Quest clearFarmersField =
                new Quest(
                    Quest_ID_CLEAR_FARMERS_FIELD,
                    "Clear the farmer's field",
                    "Kill snakes in the farmer's field and bring back 3 snake fangs. You will receive an adventurer's pass and 20 gold pieces.", 20, 20);

            // Add quest reward for collecting snake fangs
            clearFarmersField.QuestReward.Add(new QuestReward(ItemByID(ITEM_ID_SNAKE_FANG), 3));

            // Set the reward item for clearing the farmer's field
            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURER_PASS);

            // Add the levels to the Quests list
            Quests.Add(clearAlchemistGarden);
            Quests.Add(clearFarmersField);
        }

        /// <summary>
        /// Populates the game world with different locations.
        /// </summary>
        private static void PopulateLocations()
        {
            // Create each location

            // Town Square
            Location townSquare = new (LOCATION_ID_TOWN_SQUARE, "Town square", "You see a fountain.");
            Vendor Lucky = new(LUCKY_ID,"Lucky");
            Lucky.AddItemToInventory(ItemByID(ITEM_ID_PIECE_OF_FUR), 5);
            Lucky.AddItemToInventory(ItemByID(ITEM_ID_RAT_TAIL), 3);
            townSquare.VendorWorkingHere = Lucky;

            // Home
            Location home = new (LOCATION_ID_HOME, "Home", "Your house. You really need to clean up the place.");

            // Alchemist's Hut
            Location alchemistHut = new (LOCATION_ID_ALCHEMIST_HUT, "Alchemist's hut", "There are many strange plants on the shelves.");
            alchemistHut.QuestAvailableHere = QuestByID(Quest_ID_CLEAR_ALCHEMIST_GARDEN);

            // Alchemist's Garden
            Location alchemistsGarden = new (LOCATION_ID_ALCHEMISTS_GARDEN, "Alchemist's garden", "Many plants are growing here.");
            alchemistsGarden.AddEnemy(Enemy_ID_RAT, 100);

            // Farmhouse
            Location farmhouse = new (LOCATION_ID_FARMHOUSE, "Farmhouse", "There is a small farmhouse, with a farmer in front.");
            farmhouse.QuestAvailableHere = QuestByID(Quest_ID_CLEAR_FARMERS_FIELD);

            // Farmer's Field
            Location farmersField = new (LOCATION_ID_FARM_FIELD, "Farmer's field", "You see rows of vegetables growing here.");
            farmersField.AddEnemy(Enemy_ID_SNAKE, 100);

            // Guard Post
            Location guardPost = new Location(LOCATION_ID_GUARD_POST, "Guard post", "There is a large, tough-looking guard here.", ItemByID(ITEM_ID_ADVENTURER_PASS));

            // Bridge
            Location bridge = new Location(LOCATION_ID_BRIDGE, "Bridge", "A stone bridge crosses a wide river.");

            // Spider Field
            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD, "Forest", "You see spider webs covering the trees in this forest.");
            spiderField.AddEnemy(Enemy_ID_GIANT_SPIDER, 100);

            // Link the locations together

            home.LocationToNorth = townSquare;

            townSquare.LocationToNorth = alchemistHut;
            townSquare.LocationToSouth = home;
            townSquare.LocationToEast = guardPost;
            townSquare.LocationToWest = farmhouse;

            farmhouse.LocationToEast = townSquare;
            farmhouse.LocationToWest = farmersField;

            farmersField.LocationToEast = farmhouse;

            alchemistHut.LocationToSouth = townSquare;
            alchemistHut.LocationToNorth = alchemistsGarden;

            alchemistsGarden.LocationToSouth = alchemistHut;

            guardPost.LocationToEast = bridge;
            guardPost.LocationToWest = townSquare;

            bridge.LocationToWest = guardPost;
            bridge.LocationToEast = spiderField;

            spiderField.LocationToWest = bridge;

            // Add the locations to the static list

            Locations.Add(home);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(alchemistsGarden);
            Locations.Add(farmhouse);
            Locations.Add(farmersField);
            Locations.Add(bridge);
            Locations.Add(spiderField);
        }


        /// <summary>
        /// Retrieves an item by its ID.
        /// </summary>
        /// <param name="id">The ID of the item.</param>
        /// <returns>The item with the specified ID, or null if not found.</returns>
        public static Item ItemByID(int id) => Items.SingleOrDefault(x => x.ID == id);

        /// <summary>
        /// Retrieves an enemy by its ID.
        /// </summary>
        /// <param name="id">The ID of the enemy.</param>
        /// <returns>The enemy with the specified ID, or null if not found.</returns>
        public static Enemy EnemyByID(int id) => Enemies.SingleOrDefault(x => x.ID == id);

        /// <summary>
        /// Retrieves a level by its ID.
        /// </summary>
        /// <param name="id">The ID of the level.</param>
        /// <returns>The level with the specified ID, or null if not found.</returns>
        public static Quest QuestByID(int id) => Quests.SingleOrDefault(x => x.ID == id);

        /// <summary>
        /// Retrieves a location by its ID.
        /// </summary>
        /// <param name="id">The ID of the location.</param>
        /// <returns>The location with the specified ID, or null if not found.</returns>
        public static Location LocationByID(int id) => Locations.SingleOrDefault(x => x.ID == id);

    }
}

