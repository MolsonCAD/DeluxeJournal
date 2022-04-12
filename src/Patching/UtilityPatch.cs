using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;

namespace DeluxeJournal.Patching
{
    /// <summary>Patches for <see cref="Utility"/>.</summary>
    internal class UtilityPatch : PatchBase<UtilityPatch>
    {
        private EventManager EventManager { get; }

        public UtilityPatch(EventManager eventManager, IMonitor monitor) : base(monitor)
        {
            EventManager = eventManager;
            Instance = this;
        }

        private static bool Prefix_checkItemFirstInventoryAdd(Item item)
        {
            try
            {
                if (item is SObject obj && !obj.HasBeenInInventory)
                {
                    Instance.EventManager.ItemCollected.Raise(null, new ItemReceivedEventArgs(Game1.player, obj, obj.Stack));
                }
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Prefix_checkItemFirstInventoryAdd));
            }

            return true;
        }

        public override void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.checkItemFirstInventoryAdd)),
                prefix: new HarmonyMethod(typeof(UtilityPatch), nameof(Prefix_checkItemFirstInventoryAdd))
            );
        }
    }
}
