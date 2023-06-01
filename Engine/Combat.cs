using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Combat 
    {
        public Enemy CurrentEnemy { get; set; }

        private bool usedMight = false;

        public Combat (Location location) {

            CurrentEnemy = location.SetTheCurrentEnemyForTheCurrentLocation();
            
        }

        /// <summary>
        /// Uses the specified weapon to attack the current enemy.
        /// </summary>
        /// <param name="weapon">The weapon to use.</param>
        public void UseWeapon(Player player, Weapon weapon)
        {
            

            if (usedMight)
            {
                weapon.MaximumDamage += 5;
            }

            int damage = RandomNumberGenerator.NumberBetween(weapon.MinimumDamage, weapon.MaximumDamage);

            if (damage == 0)
            {
                MessageHandler.RaiseMessage("You missed the " + CurrentEnemy.Name);
            }
            else
            {
                CurrentEnemy.CurrentHitPoints -= damage;
                MessageHandler.RaiseMessage("You hit the " + CurrentEnemy.Name + " for " + damage + " points.");
            }

            if (CurrentEnemy.IsDead)
            {
                LootTheCurrentEnemy(player);

                // "Move" to the current location, to refresh the current monster
                Navigation.MoveTo(player, player.CurrentLocation);
            }
            else
            {
                LetTheEnemyAttack(player);
            }
        }

        /// <summary>
        /// Loots the current enemy after defeating it, gaining experience points, gold, and items.
        /// </summary>
        private void LootTheCurrentEnemy(Player player)
        {
            MessageHandler.RaiseMessage("");
            MessageHandler.RaiseMessage("You defeated the " + CurrentEnemy.Name);
            MessageHandler.RaiseMessage("You receive " + CurrentEnemy.RewardExperiencePoints + " experience points");
            MessageHandler.RaiseMessage("You receive " + CurrentEnemy.RewardGold + " gold");

            player.AddExperiencePoints(CurrentEnemy.RewardExperiencePoints);
            player.Gold += CurrentEnemy.RewardGold;

            // Give monster's loot items to the player
            foreach (Inventory inventoryItem in CurrentEnemy.LootItems)
            {
                player.AddItemToInventory(inventoryItem.Detail);
                MessageHandler.RaiseMessage(string.Format("You loot {0} {1}", inventoryItem.Quantity, inventoryItem.Description));
            }

            MessageHandler.RaiseMessage("");
        }

        /// <summary>
        /// Uses a healing potion to heal the player, removes it from the inventory, and lets the enemy attack.
        /// </summary>
        /// <param name="potion">The healing potion to use.</param>
        public void UsePotion(Player player, HealingPotion healPotion = null, MightPotion mightPotion = null)
        {
            if (healPotion != null && mightPotion == null)
            {
                MessageHandler.RaiseMessage("You drink a " + healPotion.Name);
                player.Heal(healPotion.AmountToHeal);
                player.RemoveItemFromInventory(healPotion);
            }
            else if (mightPotion != null)
            {
                MessageHandler.RaiseMessage("You drink a " + mightPotion.Name);
                MightPlayer(mightPotion.BonusHit);
                player.RemoveItemFromInventory(mightPotion);
            }


            // The player used their turn to drink the potion, so let the monster attack now
            LetTheEnemyAttack(player);
        }

        public void MightPlayer(int might)
        {
            usedMight = true;
            MessageHandler.RaiseMessage("You gain " + might + " might.");
        }

        /// <summary>
        /// Lets the current enemy attack the player.
        /// </summary>
        private void LetTheEnemyAttack(Player player)
        {
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, CurrentEnemy.MaximumDamage);
            MessageHandler.RaiseMessage("The " + CurrentEnemy.Name + " did " + damageToPlayer + " points of damage.");
            player.CurrentHitPoints -= damageToPlayer;

            if (player.IsDead)
            {
                MessageHandler.RaiseMessage("The " + CurrentEnemy.Name + " killed you.");
                Navigation.MoveHome(player);
            }
        }



    }
}
