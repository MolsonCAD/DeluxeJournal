using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using DeluxeJournal.Events;

namespace DeluxeJournal.Framework.Events
{
    internal class EventManager
    {
        public IManagedEvent<TaskListChangedArgs> TaskListChanged { get; }

        public IManagedEvent<TaskStatusChangedArgs> TaskStatusChanged { get; }

        public IManagedEvent<ItemReceivedEventArgs> ItemCollected { get; }

        public IManagedEvent<ItemReceivedEventArgs> ItemCrafted { get; }

        public IManagedEvent<GiftEventArgs> ItemGifted { get; }

        public IManagedEvent<SalableEventArgs> SalablePurchased { get; }

        public IManagedEvent<SalableEventArgs> SalableSold { get; }

        public IManagedEvent<FarmAnimalEventArgs> FarmAnimalPurchased { get; }

        public IManagedEvent<FarmAnimalEventArgs> FarmAnimalSold { get; }

        public BuildingConstructedEvent BuildingConstructed { get; }

        public IModEvents ModEvents { get; }

        public IMonitor Monitor { get; }

        public EventManager(IModEvents modEvents, IMultiplayerHelper multiplayer, IMonitor monitor)
        {
            ModEvents = modEvents;
            Monitor = monitor;

            TaskListChanged = new ManagedEvent<TaskListChangedArgs>(nameof(TaskListChanged));
            TaskStatusChanged = new ManagedEvent<TaskStatusChangedArgs>(nameof(TaskStatusChanged));
            ItemCollected = new ManagedEvent<ItemReceivedEventArgs>(nameof(ItemCollected));
            ItemCrafted = new ManagedEvent<ItemReceivedEventArgs>(nameof(ItemCrafted));
            ItemGifted = new ManagedEvent<GiftEventArgs>(nameof(ItemGifted));
            SalablePurchased = new ManagedEvent<SalableEventArgs>(nameof(SalablePurchased));
            SalableSold = new ManagedEvent<SalableEventArgs>(nameof(SalableSold));
            FarmAnimalPurchased = new ManagedEvent<FarmAnimalEventArgs>(nameof(FarmAnimalPurchased));
            FarmAnimalSold = new ManagedEvent<FarmAnimalEventArgs>(nameof(FarmAnimalSold));
            BuildingConstructed = new BuildingConstructedEvent(nameof(BuildingConstructed), multiplayer);

            ModEvents.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == DeluxeJournalMod.Instance?.ModManifest.UniqueID)
            {
                if (GetType().GetProperty(e.Type, BindingFlags.Public | BindingFlags.Instance)?.GetValue(this) is IReceivableNetEvent netEvent)
                {
                    try
                    {
                        netEvent.RaiseFromMessage(sender, e);
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
