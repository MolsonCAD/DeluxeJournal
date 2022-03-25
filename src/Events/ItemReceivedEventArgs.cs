using StardewValley;

namespace DeluxeJournal.Events
{
    public class ItemReceivedEventArgs : EventArgs
    {
        /// <summary>Item received.</summary>
        public SObject Item { get; }

        /// <summary>Number of items received (equivalent to Item.Stack).</summary>
        public int Count { get; }

        public ItemReceivedEventArgs(SObject item, int count)
        {
            Item = item;
            Count = count;
        }
    }
}
