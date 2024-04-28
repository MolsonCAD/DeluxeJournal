using StardewValley;

namespace DeluxeJournal.Events
{
    public class SalableEventArgs : EventArgs
    {
        /// <summary>The player handling the <see cref="ISalable"/>.</summary>
        public Farmer Player { get; }

        /// <summary>The target <see cref="ISalable"/>.</summary>
        public ISalable Salable { get; }

        /// <summary>Amount associated with the <see cref="ISalable"/>.</summary>
        public int Amount { get; }

        public SalableEventArgs(Farmer player, ISalable salable, int amount)
        {
            Player = player;
            Salable = salable;
            Amount = amount;
        }
    }
}
