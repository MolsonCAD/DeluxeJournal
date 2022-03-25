using DeluxeJournal.Events;
using StardewValley;

namespace DeluxeJournal.Framework.Listeners
{
    /// <summary>Listens for Quest.type_crafting completion checks.</summary>
    internal class CraftingListener : QuestEventListener
    {
        public event EventHandler<ItemReceivedEventArgs>? ItemCrafted;

        public CraftingListener() : base(type_crafting)
        {
        }

        protected override void OnChecked(NPC? npc, int index, int count, Item? item, string? str)
        {
            if (item is SObject crafted)
            {
                ItemCrafted?.Invoke(null, new ItemReceivedEventArgs(crafted, 1));
            }
        }
    }
}
