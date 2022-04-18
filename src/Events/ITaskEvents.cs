using StardewModdingAPI.Events;

namespace DeluxeJournal.Events
{
    /// <summary>Events provided to every ITask.</summary>
    public interface ITaskEvents
    {
        /// <summary>An Item was collected for the first time, i.e. Item.HasBeenInInventory is false.</summary>
        event EventHandler<ItemReceivedEventArgs> ItemCollected;

        /// <summary>An Item has been crafted from the crafting menu.</summary>
        event EventHandler<ItemReceivedEventArgs> ItemCrafted;

        /// <summary>An Item has been given to an NPC.</summary>
        event EventHandler<GiftEventArgs> ItemGifted;

        /// <summary>A ISalable has been purchased.</summary>
        event EventHandler<SalablePurchasedEventArgs> SalablePurchased;

        /// <summary>An ISalable has been sold.</summary>
        event EventHandler<SalableSoldEventArgs> SalableSold;

        /// <summary>A Building has been constructed. Fires for both new and upgraded buildings.</summary>
        /// <remarks>Upgrades are currently only detected via a CarpenterMenu (i.e. Robin or the Wizard).</remarks>
        event EventHandler<BuildingConstructedEventArgs> BuildingConstructed;

        /// <summary>SMAPI mod events bus.</summary>
        IModEvents ModEvents { get; }
    }
}
