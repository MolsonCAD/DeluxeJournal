using StardewModdingAPI.Events;

namespace DeluxeJournal.Framework.Events
{
    internal interface IReceivableNetEvent
    {
        /// <summary>Raise this event from a broadcast message.</summary>
        /// <param name="invoker">Object that raised this event.</param>
        /// <param name="args"><see cref="IMultiplayerEvents.ModMessageReceived"/> event args.</param>
        /// /// <exception cref="InvalidOperationException">Thrown when message payload cannot be deserialized.</exception>
        void RaiseFromMessage(object? invoker, ModMessageReceivedEventArgs args);
    }
}
