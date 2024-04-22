using StardewValley;
using StardewValley.GameData.FarmAnimals;

namespace DeluxeJournal.Events
{
    public class FarmAnimalEventArgs : EventArgs
    {
        /// <summary>The player interacting with the <see cref="FarmAnimal"/>.</summary>
        public Farmer Player { get; }

        /// <summary>
        /// The farm animal type. Also the key for the associated data object in
        /// <see cref="Game1.farmAnimalData"/>.
        /// </summary>
        public string FarmAnimalType { get; }

        /// <summary>
        /// The <see cref="StardewValley.GameData.FarmAnimals.FarmAnimalData"/> of the target farm animal.
        /// </summary>
        public FarmAnimalData FarmAnimalData { get; }

        public FarmAnimalEventArgs(Farmer player, string farmAnimalType, FarmAnimalData farmAnimalData)
        {
            Player = player;
            FarmAnimalType = farmAnimalType;
            FarmAnimalData = farmAnimalData;
        }
    }
}
