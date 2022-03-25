using StardewModdingAPI.Events;
using DeluxeJournal.Events;

namespace DeluxeJournal.Framework.Events
{
    internal class TaskEvents : ITaskEvents
    {
        private readonly TaskEventManager _eventManager;

        public event EventHandler<ItemReceivedEventArgs> ItemCollected
        {
            add => _eventManager.ItemCollected += value;
            remove => _eventManager.ItemCollected -= value;
        }

        public event EventHandler<ItemReceivedEventArgs> ItemCrafted
        {
            add => _eventManager.ItemCrafted += value;
            remove => _eventManager.ItemCrafted -= value;
        }

        public event EventHandler<GiftEventArgs> ItemGifted
        {
            add => _eventManager.ItemGifted += value;
            remove => _eventManager.ItemGifted -= value;
        }

        public IModEvents ModEvents => _eventManager.ModEvents;

        public TaskEvents(TaskEventManager eventManager)
        {
            _eventManager = eventManager;
        }
    }
}
