using StardewValley;

namespace DeluxeJournal.Events
{
    public class ItemReceivedEventArgs(Farmer player, Item item, int count) : EventArgs
    {
        /// <summary>The player who received the item stack.</summary>
        public Farmer Player { get; } = player;

        /// <summary>Item received.</summary>
        public Item Item { get; } = item;

        /// <summary>Number of items received. May differ from Item.Stack if it was split between stacks or the inventory was full.</summary>
        public int Count { get; } = count;
    }
}
