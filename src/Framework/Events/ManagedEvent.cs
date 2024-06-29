namespace DeluxeJournal.Framework.Events
{
    internal class ManagedEvent<TEventArgs> : IManagedEvent<TEventArgs> where TEventArgs : EventArgs
    {
        private event EventHandler<TEventArgs>? Event;

        public string EventName { get; }

        public bool HasEventListeners => Event != null;

        public ManagedEvent(string name)
        {
            EventName = name;
        }

        public void Add(EventHandler<TEventArgs> handler)
        {
            Event += handler;
        }

        public void Remove(EventHandler<TEventArgs> handler)
        {
            Event -= handler;
        }

        public void Raise(object? invoker, TEventArgs args)
        {
            Event?.Invoke(invoker, args);
        }
    }
}
