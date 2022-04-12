using StardewValley;

namespace DeluxeJournal.Events
{
    public class SalablePurchasedEventArgs : EventArgs
    {
        /// <summary>The player who purchased the ISalable.</summary>
        public Farmer Player { get; }

        /// <summary>ISalable purchased.</summary>
        public ISalable Salable { get; }

        /// <summary>Amount purchased.</summary>
        public int Amount { get; }

        public SalablePurchasedEventArgs(Farmer player, ISalable salable, int amount)
        {
            Player = player;
            Salable = salable;
            Amount = amount;
        }
    }
}
