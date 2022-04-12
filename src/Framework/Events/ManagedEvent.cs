namespace DeluxeJournal.Framework.Events
{
    internal class ManagedEvent<TEventArgs> where TEventArgs : EventArgs
    {
        private event EventHandler<TEventArgs>? Event;

        /// <summary>Add an event handler.</summary>
        public void Add(EventHandler<TEventArgs> handler)
        {
            Event += handler;
        }

        /// <summary>Remove an event handler.</summary>
        public void Remove(EventHandler<TEventArgs> handler)
        {
            Event -= handler;
        }

        /// <summary>Raise this event.</summary>
        /// <param name="invoker">Object that raised this event.</param>
        /// <param name="args">Event args.</param>
        public void Raise(object? invoker, TEventArgs args)
        {
            Event?.Invoke(invoker, args);
        }
    }
}
