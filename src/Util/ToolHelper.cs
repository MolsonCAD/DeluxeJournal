using StardewValley;
using StardewValley.Tools;

namespace DeluxeJournal.Util
{
    public static class ToolHelper
    {
        /// <summary>Extract a ToolDescription from a Tool.</summary>
        public static ToolDescription GetToolDescription(Tool tool)
        {
            return tool.GetType().Name switch
            {
                nameof(Axe) => new ToolDescription(0, (byte)tool.UpgradeLevel),
                nameof(Hoe) => new ToolDescription(1, (byte)tool.UpgradeLevel),
                nameof(Pickaxe) => new ToolDescription(2, (byte)tool.UpgradeLevel),
                nameof(WateringCan) => new ToolDescription(3, (byte)tool.UpgradeLevel),
                nameof(FishingRod) => new ToolDescription(4, (byte)tool.UpgradeLevel),
                nameof(Pan) => new ToolDescription(5, (byte)tool.UpgradeLevel),
                nameof(Shears) => new ToolDescription(6, (byte)tool.UpgradeLevel),
                nameof(MilkPail) => new ToolDescription(7, (byte)tool.UpgradeLevel),
                nameof(Wand) => new ToolDescription(8, (byte)tool.UpgradeLevel),
                _ => new ToolDescription(0, 0),
            };
        }

        /// <summary>Create a Tool from a ToolDescription.</summary>
        public static Tool? GetToolFromDescription(byte index, byte upgradeLevel)
        {
            return index switch
            {
                0 => new Axe() { UpgradeLevel = upgradeLevel },
                1 => new Hoe() { UpgradeLevel = upgradeLevel },
                2 => new Pickaxe() { UpgradeLevel = upgradeLevel },
                3 => new WateringCan() { UpgradeLevel = upgradeLevel },
                4 => new FishingRod() { UpgradeLevel = upgradeLevel },
                5 => new Pan() { UpgradeLevel = upgradeLevel },
                6 => new Shears() { UpgradeLevel = upgradeLevel },
                7 => new MilkPail() { UpgradeLevel = upgradeLevel },
                8 => new Wand() { UpgradeLevel = upgradeLevel },
                _ => null
            };
        }

        /// <summary>Get the upgrade level for a tool owned by a given player.</summary>
        /// <remarks>
        /// "Ownership" here is defined as the last player to use a tool or  the tool in a
        /// player's inventory. Enabling the guess flag prioritizes unused tools to fallback on
        /// and always grabs the tool of the lowest level when breaking ties.
        /// </remarks>
        /// <param name="name">The base name of the tool.</param>
        /// <param name="player">The player that owns the tool.</param>
        /// <param name="guess">Make a guess if a definitive owner could not be found.</param>
        /// <returns>The upgrade level for the tool, or the base level if not found.</returns>
        public static int GetToolUpgradeLevelForPlayer(string name, Farmer player, bool guess = true)
        {
            int level = -1;
            int fallback = 0;
            bool foundUnownedFallback = false;

            if (player.toolBeingUpgraded.Value is Tool upgraded && upgraded.BaseName == name)
            {
                return upgraded.UpgradeLevel;
            }
            else if (player.getToolFromName(name) is Tool held)
            {
                return held.UpgradeLevel;
            }

            Utility.iterateAllItems(searchForTool);
            Utility.iterateChestsAndStorage(searchForTool);

            if (level == -1)
            {
                return fallback;
            }

            return level;

            void searchForTool(Item item)
            {
                if (level == -1 && item is Tool tool && tool.BaseName == name)
                {
                    Farmer lastPlayer = tool.getLastFarmerToUse();

                    if (lastPlayer != null && lastPlayer.UniqueMultiplayerID == player.UniqueMultiplayerID)
                    {
                        level = tool.UpgradeLevel;
                    }
                    else if (!guess)
                    {
                        return;
                    }
                    else if (lastPlayer == null && (!foundUnownedFallback || tool.UpgradeLevel < fallback))
                    {
                        fallback = tool.UpgradeLevel;
                        foundUnownedFallback = true;
                    }
                    else if (!foundUnownedFallback && tool.UpgradeLevel < fallback)
                    {
                        fallback = tool.UpgradeLevel;
                    }
                }
            }
        }

        /// <summary>Get the price for a Tool upgrade (at the blacksmith shop) for the given upgrade level.</summary>
        public static int PriceForToolUpgradeLevel(int level)
        {
            return level switch
            {
                1 => 2000,
                2 => 5000,
                3 => 10000,
                4 => 25000,
                _ => 2000,
            };
        }
    }
}
