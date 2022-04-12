using StardewValley;
using DeluxeJournal.Events;

namespace DeluxeJournal.Framework.Listeners
{
    /// <summary>Listens for Quest.type_itemDelivery completion checks.</summary>
    [Obsolete]
    internal class ItemDeliveryListener : QuestEventListener
    {
        public event EventHandler<GiftEventArgs>? ItemGifted;

        public ItemDeliveryListener() : base(type_itemDelivery)
        {
        }

        protected override void OnChecked(NPC? npc, int index, int count, Item? item, string? str)
        {
            if (npc != null && item != null)
            {
                ItemGifted?.Invoke(null, new GiftEventArgs(Game1.player, npc, item));
            }
        }
    }
}
