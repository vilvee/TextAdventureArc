using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class Navigation
    {

        /// <summary>
        /// Moves the player to a specified location.
        /// </summary>
        /// <param name="location">The location to move to.</param>
        public static void MoveTo(Player player, Location location)
        {
            if (location.PlayerDoesNotHaveTheRequiredItemToEnter(player))
            {
                location.MessageHandler.RaiseMessage("You must have a " + location.ItemRequiredToEnter.Name + " to enter this location.");
                return;
            }

            // The player can enter this location
            player.CurrentLocation = location;

            if (!player.LocationsVisited.Contains(player.CurrentLocation.ID))
            {
                player.LocationsVisited.Add(player.CurrentLocation.ID);
            }

            player.CompletelyHeal();

            if (location.HasAQuest)
            {
                if (player.PlayerDoesNotHaveThisQuest(location.QuestAvailableHere))
                {
                    GiveQuestToPlayer(location.QuestAvailableHere);
                }
                else
                {
                    if (PlayerHasNotCompleted(location.QuestAvailableHere) &&
                        PlayerHasAllQuestCompletionItemsFor(location.QuestAvailableHere))
                    {
                        GivePlayerQuestRewards(location.QuestAvailableHere);
                    }
                }
            }
        }

        /// <summary>
        /// Moves the player to the location to the north if it exists.
        /// </summary>
        public static void MoveNorth(Player player)
        {
            if (player.CurrentLocation.LocationToNorth != null)
            {
                MoveTo( player, player.CurrentLocation.LocationToNorth);
            }
        }

        /// <summary>
        /// Moves the player to the location to the east if it exists.
        /// </summary>
        public static void MoveEast(Player player)
        {
            if (player.CurrentLocation.LocationToEast != null)
            {
                MoveTo(player, player.CurrentLocation.LocationToEast);
            }
        }

        /// <summary>
        /// Moves the player to the location to the south if it exists.
        /// </summary>
        public static void MoveSouth(Player player)
        {
            if (player.CurrentLocation.LocationToSouth != null)
            {
                MoveTo(player, player.CurrentLocation.LocationToSouth);
            }
        }

        /// <summary>
        /// Moves the player to the location to the west if it exists.
        /// </summary>
        public static void MoveWest(Player player)
        {
            if (player.CurrentLocation.LocationToWest != null)
            {
                MoveTo(player, player.CurrentLocation.LocationToWest);
            }
        }

        /// <summary>
        /// Moves the player back to their home location.
        /// </summary>
        public static void MoveHome(Player player)
        {
            MoveTo(player, World.LocationByID(World.LOCATION_ID_HOME));
        }

    }
}
