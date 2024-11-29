using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;

namespace DeluxeJournal.Patching
{
    /// <summary>Patches for <see cref="Farmer"/>.</summary>
    internal class FarmerPatch : PatchBase<FarmerPatch>
    {
        private EventManager EventManager { get; }

        public FarmerPatch(EventManager eventManager, IMonitor monitor) : base(monitor)
        {
            EventManager = eventManager;
            Instance = this;
        }

        private static void Prefix_OnItemReceived(Farmer __instance, Item item, int countAdded, Item mergedIntoStack, bool hideHudNotification = false)
        {
            try
            {
                if (__instance.IsLocalPlayer && ((item is SObject obj && !obj.HasBeenInInventory) || item is Ring))
                {
                    Instance.EventManager.ItemCollected.Raise(__instance, new ItemReceivedEventArgs(__instance, item, countAdded));
                }
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Prefix_OnItemReceived));
            }
        }

        private static void Postfix_onGiftGiven(Farmer __instance, NPC npc, SObject item)
        {
            try
            {
                Instance.EventManager.ItemGifted.Raise(__instance, new GiftEventArgs(__instance, npc, item));
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Postfix_onGiftGiven));
            }
        }

        public override void Apply(Harmony harmony)
        {
            Patch(harmony,
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.OnItemReceived)),
                prefix: new HarmonyMethod(typeof(FarmerPatch), nameof(Prefix_OnItemReceived))
            );

            Patch(harmony,
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.onGiftGiven)),
                postfix: new HarmonyMethod(typeof(FarmerPatch), nameof(Postfix_onGiftGiven))
            );
        }
    }
}
