namespace DeluxeJournal.Framework.Events
{
    internal interface IManagedEvent
    {
        /// <summary>Unique human-readable name for the event.</summary>
        string EventName { get; }
    }
}
