﻿using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;

namespace DeluxeJournal.Patching
{
    /// <summary>Patches for <see cref="CarpenterMenu"/>.</summary>
    internal class CarpenterMenuPatch : PatchBase<CarpenterMenuPatch>
    {
        private EventManager EventManager { get; }

        public CarpenterMenuPatch(EventManager eventManager, IMonitor monitor) : base(monitor)
        {
            EventManager = eventManager;
            Instance = this;
        }

        private static void Postfix_returnToCarpentryMenuAfterSuccessfulBuild(CarpenterMenu __instance, bool ___upgrading)
        {
            try
            {
                Farm farm = Game1.getFarm();

                // Only handle upgrades, since ModEvents.World.BuildingListChanged is used to broadly detect additions
                if (___upgrading && Game1.GetBuildingUnderConstruction() is Building building)
                {
                    Instance.EventManager.BuildingConstructed.Broadcast(new BuildingConstructedEventArgs(farm, building, true));
                }
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Postfix_returnToCarpentryMenuAfterSuccessfulBuild));
            }
        }

        public override void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild)),
                postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(Postfix_returnToCarpentryMenuAfterSuccessfulBuild))
            );
        }
    }
}
