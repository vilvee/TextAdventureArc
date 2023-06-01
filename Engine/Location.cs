using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Engine
{
    /// <summary>
    /// Represents a location in the game.
    /// </summary>
    public class Location
    {
        private readonly SortedList<int, int> _enemiesAtLocation = new SortedList<int, int>();

        /// <summary>
        /// Gets or sets the ID of the location.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the location.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the item required to enter the location.
        /// </summary>
        public Item ItemRequiredToEnter { get; set; }

        /// <summary>
        /// Gets or sets the quest available at the location.
        /// </summary>
        public Level QuestAvailableHere { get; set; }

        /// <summary>
        /// Gets or sets the vendor working at the location.
        /// </summary>
        public Vendor VendorWorkingHere { get; set; }

        /// <summary>
        /// Gets or sets the location to the north of this location.
        /// </summary>
        public Location LocationToNorth { get; set; }

        /// <summary>
        /// Gets or sets the location to the east of this location.
        /// </summary>
        public Location LocationToEast { get; set; }

        /// <summary>
        /// Gets or sets the location to the south of this location.
        /// </summary>
        public Location LocationToSouth { get; set; }

        /// <summary>
        /// Gets or sets the location to the west of this location.
        /// </summary>
        public Location LocationToWest { get; set; }

        /// <summary>
        /// Determines if the location has an enemy.
        /// </summary>
        public bool HasAEnemy => _enemiesAtLocation.Count > 0;

        /// <summary>
        /// Determines if the location has a quest.
        /// </summary>
        public bool HasAQuest => QuestAvailableHere != null;

        /// <summary>
        /// Determines if the location does not have an item required to enter.
        /// </summary>
        public bool DoesNotHaveAnItemRequiredToEnter => ItemRequiredToEnter == null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class with the specified values.
        /// </summary>
        /// <param name="id">The ID of the location.</param>
        /// <param name="name">The name of the location.</param>
        /// <param name="description">The description of the location.</param>
        /// <param name="itemRequiredToEnter">The item required to enter the location.</param>
        /// <param name="questAvailableHere">The quest available at the location.</param>
        public Location(int id, string name, string description,
            Item itemRequiredToEnter = null, Level questAvailableHere = null)
        {
            ID = id;
            Name = name;
            Description = description;
            ItemRequiredToEnter = itemRequiredToEnter;
            QuestAvailableHere = questAvailableHere;
        }

        /// <summary>
        /// Adds an enemy to the location with a specified percentage of appearance.
        /// </summary>
        /// <param name="monsterID">The ID of the enemy.</param>
        /// <param name="percentageOfAppearance">The percentage chance of the enemy appearing.</param>
        public void AddEnemy(int monsterID, int percentageOfAppearance)
        {
            if (_enemiesAtLocation.ContainsKey(monsterID))
            {
                _enemiesAtLocation[monsterID] = percentageOfAppearance;
            }
            else
            {
                _enemiesAtLocation.Add(monsterID, percentageOfAppearance);
            }
        }

        /// <summary>
        /// Creates a new instance of an enemy living in this location.
        /// </summary>
        /// <returns>A new instance of the enemy.</returns>
        public Enemy NewInstanceOfEnemyLivingHere()
        {
            if (!HasAEnemy)
            {
                return null;
            }

            // Total the percentages of all monsters at this location.
            int totalPercentages = _enemiesAtLocation.Values.Sum();

            // Select a random number between 1 and the total (in case the total of percentages is not 100).
            int randomNumber = RandomNumberGenerator.NumberBetween(1, totalPercentages);

            // Loop through the monster list, 
            // adding the monster's percentage chance of appearing to the runningTotal variable.
            // When the random number is lower than the runningTotal,
            // that is the monster to return.
            int runningTotal = 0;

            foreach (KeyValuePair<int, int> monsterKeyValuePair in _enemiesAtLocation)
            {
                runningTotal += monsterKeyValuePair.Value;

                if (randomNumber <= runningTotal)
                {
                    return World.EnemyByID(monsterKeyValuePair.Key).NewInstanceOfEnemy();
                }
            }

            // In case there was a problem, return the last monster in the list.
            return World.EnemyByID(_enemiesAtLocation.Keys.Last()).NewInstanceOfEnemy();
        }
    }
}
