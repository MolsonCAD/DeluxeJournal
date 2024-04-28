using StardewValley;
using DeluxeJournal.Events;

namespace DeluxeJournal.Framework.Listeners
{
    /// <summary>Listens for Quest.type_harvest completion checks.</summary>
    [Obsolete]
    internal class ItemHarvestListener : QuestEventListener
    {
        public event EventHandler<ItemReceivedEventArgs>? ItemHarvested;

        public ItemHarvestListener() : base(type_harvest)
        {
        }

        protected override void OnChecked(NPC? npc, int index, int count, Item? item, string? str)
        {
            if (item is SObject collected && !collected.HasBeenInInventory)
            {
                ItemHarvested?.Invoke(null, new ItemReceivedEventArgs(Game1.player, collected, count));
            }
        }
    }
}
