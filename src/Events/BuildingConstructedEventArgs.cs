using StardewValley;
using StardewValley.Buildings;

namespace DeluxeJournal.Events
{
    public class BuildingConstructedEventArgs(GameLocation location, Building building, bool isUpgrade) : EventArgs
    {
        /// <summary>Building location.</summary>
        public GameLocation Location { get; } = location;

        /// <summary>The building being constructed.</summary>
        public Building Building { get; } = building;

        /// <summary>Is this an upgrade? False if the building is new.</summary>
        public bool IsUpgrade { get; } = isUpgrade;

        /// <summary>The building name after construction is complete.</summary>
        public string NameAfterConstruction => IsUpgrade ? Building.upgradeName.Value : Building.buildingType.Value;
    }
}
