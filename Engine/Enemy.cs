using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Enemy : Entity
    {
        public int ID { get; set; } // Unique identifier for the enemy
        public string Name { get; set; } // Name of the enemy
        public int MaximumDamage { get; set; } // Maximum damage the enemy can inflict
        public int RewardExperiencePoints { get; set; } // Experience points rewarded when the enemy is defeated
        public int RewardGold { get; set; } // Gold rewarded when the enemy is defeated

        public List<Loot> LootTable { get; set; } // List of possible items that the enemy can drop
        internal List<InventoryItem> LootItems { get; } // List of items the enemy has in its inventory

        public Enemy(int id, string name, int maximumDamage, int rewardExperiencePoints, int rewardGold, int currentHitPoints, int maximumHitPoints)
            : base(currentHitPoints, maximumHitPoints)
        {
            ID = id;
            Name = name;
            MaximumDamage = maximumDamage;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;

            LootTable = new List<Loot>();
            LootItems = new List<InventoryItem>();
        }

        /// <summary>
        /// Generates a new instance of the Enemy class with the same attributes as the current enemy.
        /// It randomly selects items from the LootTable based on their drop percentages and adds them to the LootItems list.
        /// If no items are randomly selected, it adds the default loot item(s) to the list.
        /// </summary>
        /// <returns>NewEnemy</returns>
        internal Enemy? NewInstanceOfEnemy()
        {
            Enemy? newEnemy = new Enemy(ID, Name, MaximumDamage, RewardExperiencePoints, RewardGold, CurrentHitPoints, MaximumHitPoints);

            // Add items to the LootItems list, comparing a random number to the drop percentage
            foreach (Loot lootItem in LootTable.Where(lootItem => RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage))
            {
                newEnemy.LootItems.Add(new InventoryItem(lootItem.Details, 1));
            }

            // If no items were randomly selected, add the default loot item(s)
            if (newEnemy.LootItems.Count == 0)
            {
                foreach (Loot lootItem in LootTable.Where(x => x.IsDefaultItem))
                {
                    newEnemy.LootItems.Add(new InventoryItem(lootItem.Details, 1));
                }
            }

            return newEnemy;
        }
    }
}
