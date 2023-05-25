using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Engine
{
    public class Player : Entity
    {
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public int Gold { get; set; }
        public Location CurrentLocation { get; set; }
        public List<InventoryItem> Inventory { get; set; }
        public List<Quest> Quests { get; set; }

        public Player(int level, int exp,  int gold, int maxHit, int currentHit) : base(maxHit, currentHit)
        {
           Level = level;
           ExperiencePoints = exp;
           Gold = gold;
           Inventory = new List<InventoryItem>();
           Quests = new List<Quest>();

        }

    }
}
