using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Represents a level in the game.
    /// </summary>
    public class Quest
    {
        /// <summary>
        /// Gets or sets the ID of the level.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the level.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the level.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the reward experience points of the level.
        /// </summary>
        public int RewardExperiencePoints { get; set; }

        /// <summary>
        /// Gets or sets the reward gold of the level.
        /// </summary>
        public int RewardGold { get; set; }

        /// <summary>
        /// Gets or sets the list of quest rewards for the level.
        /// </summary>
        public List<QuestReward> QuestReward { get; set; }

        /// <summary>
        /// Gets or sets the reward item of the level.
        /// </summary>
        public Item RewardItem { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quest"/> class with the specified values.
        /// </summary>
        /// <param name="id">The ID of the level.</param>
        /// <param name="name">The name of the level.</param>
        /// <param name="description">The description of the level.</param>
        /// <param name="rewardExperiencePoints">The reward experience points of the level.</param>
        /// <param name="rewardGold">The reward gold of the level.</param>
        public Quest(int id, string name, string description, int rewardExperiencePoints, int rewardGold)
        {
            ID = id;
            Name = name;
            Description = description;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;
            QuestReward = new List<QuestReward>();
        }

        /// <summary>
        /// Checks if the player has not completed a specific quest.
        /// </summary>
        /// <param name="player">The quest to check.</param>
        /// <returns>True if the player has not completed the quest, false otherwise.</returns>
        private bool PlayerHasNotCompleted(Player player)
        {
            //aq = ActiveQuest
            return player.Quests.Any(aq => aq.Details.ID == ID && !aq.IsCompleted);
        }

        /// <summary>
        /// Gives a quest to the player.
        /// </summary>
        /// <param name="player">The quest to give to the player.</param>
        public void GiveQuestToPlayer(Player player, Location location)
        {
           
                if (player.Quests.Any(aq => aq.Details.ID == ID))
                {
                    // The player already has the quest, so don't give it to them again
                    PlayerHasAllQuestCompletionItemsFor(player);
                    GivePlayerQuestRewards(player);
                    return;
                }

                MessageHandler.RaiseMessage("You receive the " + Name + " quest.");
                MessageHandler.RaiseMessage(Description);
                MessageHandler.RaiseMessage("To complete it, return with:");

                foreach (QuestReward qci in QuestReward)
                {
                    MessageHandler.RaiseMessage(string.Format("{0} {1}", qci.Quantity,
                        qci.Quantity == 1 ? qci.Details.Name : qci.Details.NamePlural));
                }

                MessageHandler.RaiseMessage("");

                player.Quests.Add(new ActiveQuest(this));
            
        }

        /// <summary>
        /// Checks if the player has all the quest completion items for a specific quest.
        /// </summary>
        /// <param name="quest">The quest to check.</param>
        /// <returns>True if the player has all the quest completion items, false otherwise.</returns>
        private bool PlayerHasAllQuestCompletionItemsFor(Player player)
        {
            // See if the player has all the items needed to complete the quest here
            foreach (QuestReward qci in QuestReward)
            {
                // Check each item in the player's inventory, to see if they have it, and enough of it
                if (!player.Inventory.Any(ii => ii.Detail.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
            }

            // If we got here, then the player must have all the required items, and enough of them, to complete the quest.
            return true;
        }

        /// <summary>
        /// Removes the quest completion items from the player's inventory for a specific quest.
        /// </summary>
        /// <param name="quest">The quest for which to remove the completion items.</param>
        private void RemoveQuestCompletionItems(Player player)
        {
            foreach (QuestReward qci in QuestReward)
            {
                Inventory item = player.Inventory.SingleOrDefault(ii => ii.Detail.ID == qci.Details.ID);

                if (item != null)
                {
                    player.RemoveItemFromInventory(item.Detail, qci.Quantity);
                }
            }
        }


        /// <summary>
        /// Gives rewards to the player for completing a quest.
        /// </summary>
        /// <param name="quest">The quest completed by the player.</param>
        private void GivePlayerQuestRewards(Player player)
        {
            MessageHandler.RaiseMessage("");
            MessageHandler.RaiseMessage("You complete the '" + Name + "' quest.");
            MessageHandler.RaiseMessage("You receive: ");
            MessageHandler.RaiseMessage(RewardExperiencePoints + " experience points");
            MessageHandler.RaiseMessage(RewardGold + " gold");
            MessageHandler.RaiseMessage(RewardItem.Name, true);

            player.AddExperiencePoints(RewardExperiencePoints);
            player.Gold += RewardGold;
            RemoveQuestCompletionItems(player);
            player.AddItemToInventory(RewardItem);
            MarkPlayerQuestCompleted(player);
        }

        /// <summary>
        /// Marks a quest as completed for the player.
        /// </summary>
        /// <param name="quest">The quest to mark as completed.</param>
        private void MarkPlayerQuestCompleted(Player player)
        {
            ActiveQuest playerActiveQuest = player.Quests.SingleOrDefault(pq => pq.Details.ID == ID);

            if (playerActiveQuest != null)
            {
                playerActiveQuest.IsCompleted = true;
            }
        }
    }

}
