using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class NPC : Entity
    {
        public int ID { get; set; } // Unique identifier for the enemy
        public string Name { get; set; } // Name of the enemy
        public List<Inventory> Shop { get; set; } // List of items the enemy has in its inventory

        public NPC(int id, string name, int currentHitPoints, int maximumHitPoints)
            : base(currentHitPoints, maximumHitPoints)
        {
            ID = id;
            Name = name;
        }

        public NPC(int id, string name, int currentHitPoints, int maximumHitPoints, List<Inventory> shop)
            : base(currentHitPoints, maximumHitPoints)
        {
            ID = id;
            Name = name;
            CreateShop(shop);
        }

        public void CreateShop(List<Inventory> shop)
        {
            Shop = shop;
        }

        //TODO: Add a way to update the shop
        public void UpdateShop()
        {

        }
    }
}
