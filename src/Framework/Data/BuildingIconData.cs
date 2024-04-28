namespace DeluxeJournal.Framework.Data
{
    internal class BuildingIconData
    {
        /// <summary>Index of the building icon in the sprite sheet.</summary>
        public int SpriteIndex { get; set; }

        /// <summary>Building upgrade tier.</summary>
        /// <remarks>
        /// 0 = none,
        /// 1 = the base variant,
        /// 2 = the "Big" variant,
        /// 3 = the "Deluxe" variant.
        /// </remarks>
        public int Tier { get; set; }
    }
}
