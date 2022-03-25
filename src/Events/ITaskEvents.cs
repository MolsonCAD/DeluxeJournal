using StardewModdingAPI.Events;

namespace DeluxeJournal.Events
{
    /// <summary>Events provided to every ITask.</summary>
    public interface ITaskEvents
    {
        /// <summary>
        /// An Item was collected for the first time, i.e. Item.HasBeenInInventory is false.
        /// Note: Does not fire for Furniture or big craftables.
        /// </summary>
        event EventHandler<ItemReceivedEventArgs> ItemCollected;

        /// <summary>An Item has been crafted from the crafting menu.</summary>
        event EventHandler<ItemReceivedEventArgs> ItemCrafted;

        /// <summary>An Item has been given to an NPC.</summary>
        event EventHandler<GiftEventArgs> ItemGifted;

        /// <summary>SMAPI mod events bus.</summary>
        IModEvents ModEvents { get; }
    }
}
