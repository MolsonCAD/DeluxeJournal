using StardewValley;

namespace DeluxeJournal.Events
{
    public class GiftEventArgs : EventArgs
    {
        /// <summary>The NPC receiving the item.</summary>
        public NPC NPC { get; }

        /// <summary>The Item given to the NPC.</summary>
        public Item Item { get; }

        public GiftEventArgs(NPC npc, Item item)
        {
            NPC = npc;
            Item = item;
        }
    }
}
