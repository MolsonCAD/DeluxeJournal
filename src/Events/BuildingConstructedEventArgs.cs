using StardewValley;
using StardewValley.Buildings;

namespace DeluxeJournal.Events
{
    public class BuildingConstructedEventArgs : EventArgs
    {
        /// <summary>Building location.</summary>
        public GameLocation Location { get; }

        /// <summary>The building being constructed.</summary>
        public Building Building { get; }

        /// <summary>Is this an upgrade? False if the building is new.</summary>
        public bool IsUpgrade { get; }

        /// <summary>The building name after construction is complete.</summary>
        public string NameAfterConstruction => IsUpgrade ? Building.getNameOfNextUpgrade() : Building.buildingType.Value;

        public BuildingConstructedEventArgs(GameLocation location, Building building, bool isUpgrade)
        {
            Location = location;
            Building = building;
            IsUpgrade = isUpgrade;
        }
    }
}
