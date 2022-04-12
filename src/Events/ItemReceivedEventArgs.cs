using StardewValley;

namespace DeluxeJournal.Events
{
    public class ItemReceivedEventArgs : EventArgs
    {
        /// <summary>The player who received the item stack.</summary>
        public Farmer Player { get; }

        /// <summary>Item received.</summary>
        public SObject Item { get; }

        /// <summary>Number of items received (equivalent to Item.Stack).</summary>
        public int Count { get; }

        public ItemReceivedEventArgs(Farmer player, SObject item, int count)
        {
            Player = player;
            Item = item;
            Count = count;
        }
    }
}
