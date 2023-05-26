using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Enemy : Entity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaximumDamage { get; set; }
        public int RewardExperiencePoints { get; set; }
        public int RewardGold { get; set; }

        // All possible items (with percentages) that this type of monster could have
        public List<Loot> LootTable { get; set; }

        // The items this instance of the monster has in their inventory
        internal List<InventoryItem> LootItems { get; }

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

        internal Enemy NewInstanceOfEnemy()
        {
            Enemy newEnemy =
                new Enemy(ID, Name, MaximumDamage, RewardExperiencePoints, RewardGold, CurrentHitPoints, MaximumHitPoints);

            // Add items to the lootedItems list, comparing a random number to the drop percentage
            foreach (Loot lootItem in LootTable.Where(lootItem => RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage))
            {
                newEnemy.LootItems.Add(new InventoryItem(lootItem.Details, 1));
            }

            // If no items were randomly selected, add the default loot item(s).
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