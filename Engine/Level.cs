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
    public class Level
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
        /// Initializes a new instance of the <see cref="Level"/> class with the specified values.
        /// </summary>
        /// <param name="id">The ID of the level.</param>
        /// <param name="name">The name of the level.</param>
        /// <param name="description">The description of the level.</param>
        /// <param name="rewardExperiencePoints">The reward experience points of the level.</param>
        /// <param name="rewardGold">The reward gold of the level.</param>
        public Level(int id, string name, string description, int rewardExperiencePoints, int rewardGold)
        {
            ID = id;
            Name = name;
            Description = description;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;
            QuestReward = new List<QuestReward>();
        }
    }
}
