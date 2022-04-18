using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace DeluxeJournal.Framework.Events
{
    /// <summary>ManagedNetEvent concrete class for a simple JSON serializable <see cref="TEventArgs"/>.</summary>
    internal class ManagedNetEvent<TEventArgs> : ManagedNetEvent<TEventArgs, TEventArgs> where TEventArgs : EventArgs
    {
        public ManagedNetEvent(string name, IMultiplayerHelper multiplayer) :
            base(name, multiplayer)
        {
        }

        protected override TEventArgs EventArgsToMessage(TEventArgs args)
        {
            return args;
        }

        protected override TEventArgs MessageToEventArgs(TEventArgs message)
        {
            return message;
        }
    }

    internal abstract class ManagedNetEvent<TEventArgs, TMessage> : ManagedEvent<TEventArgs>, IManagedNetEvent where TEventArgs : EventArgs
    {
        private IMultiplayerHelper Multiplayer { get; }

        public ManagedNetEvent(string name, IMultiplayerHelper multiplayer) : base(name)
        {
            Multiplayer = multiplayer;
        }

        /// <summary>Construct a <see cref="TMessage"/> from args.</summary>
        protected abstract TMessage EventArgsToMessage(TEventArgs args);

        /// <summary>Construct <see cref="TEventArgs"/> from a broadcasted message.</summary>
        protected abstract TEventArgs MessageToEventArgs(TMessage message);

        /// <summary>Raise this event from a broadcast message.</summary>
        /// <param name="invoker">Object that raised this event.</param>
        /// <param name="args"><see cref="IMultiplayerEvents.ModMessageReceived"/> event args.</param>
        /// <exception cref="ArgumentException">Thrown when message payload cannot be deserialized as a <see cref="TMessage"/>.</exception>
        public void RaiseFromMessage(object? invoker, ModMessageReceivedEventArgs args)
        {
            if (args.Type == EventName && args.ReadAs<TMessage>() is TMessage message)
            {
                Raise(invoker, MessageToEventArgs(message));
            }
        }

        /// <summary>Broadcast this event to all peers via multiplayer message.</summary>
        public void Broadcast(TEventArgs args, bool sendToSelf = true)
        {
            Multiplayer.SendMessage(
                EventArgsToMessage(args),
                EventName,
                modIDs: new[] { DeluxeJournalMod.Instance?.ModManifest.UniqueID }
            );

            // Messages are not sent back to the mod that sent them, so raise this event locally
            if (sendToSelf)
            {
                Raise(null, args);
            }
        }
    }
}
