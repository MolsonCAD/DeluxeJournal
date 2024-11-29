using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;

namespace DeluxeJournal.Patching
{
    /// <summary>Patches for <see cref="CraftingPage"/>.</summary>
    internal class CraftingPagePatch : PatchBase<CraftingPagePatch>
    {
        private EventManager EventManager { get; }

        public CraftingPagePatch(EventManager eventManager, IMonitor monitor) : base(monitor)
        {
            EventManager = eventManager;
            Instance = this;
        }

        private static void OnRecipeCrafted(Item crafted)
        {
            try
            {
                if (crafted is SObject || crafted is Ring)
                {
                    Farmer player = Game1.player;
                    Instance.EventManager.ItemCrafted.Raise(player, new ItemReceivedEventArgs(player, crafted, crafted.Stack));
                }
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(OnRecipeCrafted));
            }
        }

        /// <summary>
        /// Inject a call to <see cref="OnRecipeCrafted(Item)"/> to raise the <see cref="EventManager.ItemCrafted"/> event.
        /// </summary>
        private static IEnumerable<CodeInstruction> Transpiler_clickCraftingRecipe(IEnumerable<CodeInstruction> instructions)
        {
            // NOTE: The compiler generates a closure class to pass to Farmer.NotifyQuests(Func<Quest, bool>, bool)
            // which contains the "crafted" item as a field (rather than a local variable, as the C# code would
            // have you believe).
            foreach (Type nestedPrivateType in typeof(CraftingPage).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (nestedPrivateType.GetField("crafted") is not FieldInfo craftedField)
                {
                    continue;
                }

                CodeMatcher codeMatcher = new(instructions);
                MethodInfo notifyQuestsMethod = AccessTools.Method(typeof(Farmer), nameof(Farmer.NotifyQuests));
                MethodInfo onRecipeCraftedMethod = AccessTools.Method(typeof(CraftingPagePatch), nameof(OnRecipeCrafted));

                codeMatcher.MatchEndForward(
                        new CodeMatch(OpCodes.Callvirt, notifyQuestsMethod),
                        new CodeMatch(OpCodes.Pop)
                    )
                    .ThrowIfNotMatch($"Could not find entry point for {nameof(Transpiler_clickCraftingRecipe)}")
                    .Insert(
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldfld, craftedField),
                        new CodeInstruction(OpCodes.Call, onRecipeCraftedMethod)
                    );

                return codeMatcher.InstructionEnumeration();
            }

            Instance.LogError("Unable to find the 'crafted' item field.", nameof(Transpiler_clickCraftingRecipe));
            return instructions;
        }

        public override void Apply(Harmony harmony)
        {
            Patch(harmony,
                original: AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe"),
                transpiler: new HarmonyMethod(typeof(CraftingPagePatch), nameof(Transpiler_clickCraftingRecipe))
            );
        }
    }
}
