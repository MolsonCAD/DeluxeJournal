using StardewModdingAPI.Events;
using DeluxeJournal.Events;

namespace DeluxeJournal.Framework.Events
{
    internal class EventManager
    {
        public ManagedEvent<ItemReceivedEventArgs> ItemCollected { get; }

        public ManagedEvent<ItemReceivedEventArgs> ItemCrafted { get; }

        public ManagedEvent<GiftEventArgs> ItemGifted { get; }

        public ManagedEvent<SalablePurchasedEventArgs> SalablePurchased { get; }

        public ManagedEvent<SalableSoldEventArgs> SalableSold { get; }

        public IModEvents ModEvents { get; }

        public EventManager(IModEvents modEvents)
        {
            ModEvents = modEvents;

            ItemCollected = new ManagedEvent<ItemReceivedEventArgs>();
            ItemCrafted = new ManagedEvent<ItemReceivedEventArgs>();
            ItemGifted = new ManagedEvent<GiftEventArgs>();
            SalablePurchased = new ManagedEvent<SalablePurchasedEventArgs>();
            SalableSold = new ManagedEvent<SalableSoldEventArgs>();
        }
    }
}
