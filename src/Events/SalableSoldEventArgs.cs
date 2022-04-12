using StardewValley;

namespace DeluxeJournal.Events
{
    public class SalableSoldEventArgs : EventArgs
    {
        /// <summary>The player who sold the ISalable.</summary>
        public Farmer Player { get; }

        /// <summary>ISalable sold.</summary>
        public ISalable Salable { get; }

        /// <summary>Amount sold.</summary>
        public int Amount { get; }

        public SalableSoldEventArgs(Farmer player, ISalable salable, int amount)
        {
            Player = player;
            Salable = salable;
            Amount = amount;
        }
    }
}
