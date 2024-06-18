namespace DeluxeJournal.Framework.Events
{
    internal interface IManagedNetEvent<TEventArgs> : IReceivableNetEvent, IManagedEvent<TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>Broadcast this event to all peers via multiplayer message.</summary>
        /// <param name="args">Event arguments.</param>
        /// <param name="sendToSelf">Raise event locally, since multiplayer messages are not sent back to the sender.</param>
        /// <exception cref="InvalidOperationException">Thrown when broadcasting before mod initialization.</exception>
        void Broadcast(TEventArgs args, bool sendToSelf = true);
    }
}
