using StardewModdingAPI.Events;

namespace DeluxeJournal.Framework.Events
{
    internal interface IManagedNetEvent : IManagedEvent
    {
        /// <summary>Raise this event from a broadcast message.</summary>
        /// <param name="invoker">Object that raised this event.</param>
        /// <param name="args"><see cref="IMultiplayerEvents.ModMessageReceived"/> event args.</param>
        void RaiseFromMessage(object? invoker, ModMessageReceivedEventArgs args);
    }
}
