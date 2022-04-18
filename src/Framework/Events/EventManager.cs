using System.Reflection;
using StardewModdingAPI;
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

        public BuildingConstructedEvent BuildingConstructed { get; }

        public IModEvents ModEvents { get; }

        public IMonitor Monitor { get; }

        public EventManager(IModEvents modEvents, IMultiplayerHelper multiplayer, IMonitor monitor)
        {
            ModEvents = modEvents;
            Monitor = monitor;

            ItemCollected = new ManagedEvent<ItemReceivedEventArgs>(nameof(ItemCollected));
            ItemCrafted = new ManagedEvent<ItemReceivedEventArgs>(nameof(ItemCrafted));
            ItemGifted = new ManagedEvent<GiftEventArgs>(nameof(ItemGifted));
            SalablePurchased = new ManagedEvent<SalablePurchasedEventArgs>(nameof(SalablePurchased));
            SalableSold = new ManagedEvent<SalableSoldEventArgs>(nameof(SalableSold));
            BuildingConstructed = new BuildingConstructedEvent(nameof(BuildingConstructed), multiplayer);
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == DeluxeJournalMod.Instance?.ModManifest.UniqueID)
            {
                if (GetType().GetProperty(e.Type, BindingFlags.Public | BindingFlags.Instance)?.GetValue(this) is IManagedNetEvent managed)
                {
                    try
                    {
                        managed.RaiseFromMessage(sender, e);
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log(string.Format("Failed to raise broadcasted event '{0}'. See log file for details.", e.Type), LogLevel.Error);
                        Monitor.Log(ex.ToString(), LogLevel.Trace);
                    }
                }
            }
        }
    }
}
