using StardewValley;

namespace DeluxeJournal.Events
{
    public class SalableEventArgs(Farmer player, ISalable salable, int amount) : EventArgs
    {
        /// <summary>The player handling the <see cref="ISalable"/>.</summary>
        public Farmer Player { get; } = player;

        /// <summary>The target <see cref="ISalable"/>.</summary>
        public ISalable Salable { get; } = salable;

        /// <summary>Amount associated with the <see cref="ISalable"/>.</summary>
        public int Amount { get; } = amount;
    }
}
