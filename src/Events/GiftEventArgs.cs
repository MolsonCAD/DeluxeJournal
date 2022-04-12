using StardewValley;

namespace DeluxeJournal.Events
{
    public class GiftEventArgs : EventArgs
    {
        /// <summary>The player gifting the item.</summary>
        public Farmer Player { get; }

        /// <summary>The NPC receiving the item.</summary>
        public NPC NPC { get; }

        /// <summary>The Item given to the NPC.</summary>
        public Item Item { get; }

        public GiftEventArgs(Farmer player, NPC npc, Item item)
        {
            Player = player;
            NPC = npc;
            Item = item;
        }
    }
}
