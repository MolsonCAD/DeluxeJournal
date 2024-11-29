using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Characters;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Weapons;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TokenizableStrings;
using DeluxeJournal.Task;

namespace DeluxeJournal.Util
{
    /// <summary>Provides a collection of <see cref="LocalizedGameDataMap{T}"/> objects.</summary>
    public sealed class LocalizedGameDataMaps(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor = null)
    {
        /// <inheritdoc cref="LocalizedItemMap"/>
        public LocalizedGameDataMap LocalizedItems { get; } = new LocalizedItemMap(translation, settings, monitor);

        /// <inheritdoc cref="LocalizedNpcMap"/>
        public LocalizedGameDataMap LocalizedNpcs { get; } = new LocalizedNpcMap(translation, settings, monitor);

        /// <inheritdoc cref="LocalizedBuildingMap"/>
        public LocalizedGameDataMap LocalizedBuildings { get; } = new LocalizedBuildingMap(translation, settings, monitor);

        /// <inheritdoc cref="LocalizedFarmAnimalMap"/>
        public LocalizedGameDataMap LocalizedFarmAnimals { get; } = new LocalizedFarmAnimalMap(translation, settings, monitor);

        public LocalizedGameDataMaps(ITranslationHelper translation, IMonitor? monitor = null)
            : this(translation, new(), monitor)
        {
        }

        /// <summary>Maps localized item names to qualified item ID's.</summary>
        private class LocalizedItemMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
            : LocalizedGameDataMap("alias.items.", translation, settings, monitor)
        {
            private static readonly HashSet<string> IgnoredItemIds = ["30", "73", "858", "892", "922", "923", "924", "925", "927", "929", "930", "DriedFruit", "DriedMushrooms", "SmokedFish"];
            private static readonly HashSet<string> IgnoredItemTypes = ["Litter", "Quest", "asdf", "interactive"];

            protected override void PopulateDataMap()
            {
                if (Settings.AllItemsEnabled || Settings.SetItemCategoryObject || Settings.SetItemCategoryCraftable)
                {
                    AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12852"), SObject.FishCategory.ToString());

                    foreach (KeyValuePair<string, ObjectData> pair in Game1.objectData)
                    {
                        if (!IgnoredItemIds.Contains(pair.Key) && pair.Value != null && !IgnoredItemTypes.Contains(pair.Value.Type))
                        {
                            if (!Settings.SetItemCategoryObject && Settings.SetItemCategoryCraftable
                                && (pair.Value.Name == null || !CraftingRecipe.craftingRecipes.ContainsKey(pair.Value.Name)))
                            {
                                continue;
                            }

                            if (TokenParser.ParseText(pair.Value.DisplayName) is string parsedName)
                            {
                                string itemId = ItemRegistry.type_object + pair.Key;

                                AddPlural(parsedName, itemId);
                                AddFlavored(parsedName, pair.Key, pair.Value);
                                AddConvenienceAlternates(itemId, pair.Value);
                            }
                        }
                    }
                }

                if (Settings.AllItemsEnabled || Settings.SetItemCategoryBigCraftable)
                {
                    foreach (KeyValuePair<string, BigCraftableData> pair in Game1.bigCraftableData)
                    {
                        AddPlural(TokenParser.ParseText(pair.Value?.DisplayName), ItemRegistry.type_bigCraftable + pair.Key);
                    }
                }

                if (Settings.AllItemsEnabled || Settings.SetItemCategoryTool)
                {
                    foreach (string toolId in Game1.toolData.Keys)
                    {
                        string toolQid = ItemRegistry.type_tool + toolId;

                        if (ItemRegistry.GetData(toolQid) is ParsedItemData itemData)
                        {
                            Add(itemData.DisplayName.ToLower(), toolQid);
                        }
                    }

                    Add(Game1.content.LoadString("Strings\\Tools:TrashCan_Name"), "(T)CopperTrashCan");
                }

                if (Settings.AllItemsEnabled)
                {
                    foreach (KeyValuePair<string, WeaponData> pair in Game1.weaponData)
                    {
                        Add(TokenParser.ParseText(pair.Value?.DisplayName), ItemRegistry.type_weapon + pair.Key);
                    }

                    foreach (KeyValuePair<string, string> pair in DataLoader.Boots(Game1.content))
                    {
                        string[] fields = pair.Value.Split('/');
                        Add(ArgUtility.Get(fields, fields.Length < 7 ? 0 : 6), ItemRegistry.type_boots + pair.Key);
                    }

                    foreach (KeyValuePair<string, string> pair in DataLoader.Hats(Game1.content))
                    {
                        Add(ArgUtility.Get(pair.Value.Split('/'), 5), ItemRegistry.type_hat + pair.Key);
                    }
                }
            }

            /// <summary>Populate data map with flavored items.</summary>
            /// <param name="localizedName">Localized ingredient name.</param>
            /// <param name="unqualifiedId">Unqualified item ID.</param>
            /// <param name="objectData">Object data.</param>
            private void AddFlavored(string localizedName, string unqualifiedId, ObjectData objectData)
            {
                int category = objectData.Category;

                // Wild Honey
                if (unqualifiedId == "340")
                {
                    Add(Game1.content.LoadString("Strings\\Objects:Honey_Name"), ItemRegistry.type_object + unqualifiedId);
                    Add(Game1.content.LoadString("Strings\\Objects:Honey_Wild_Name"), ItemRegistry.type_object + unqualifiedId);
                    return;
                }

                // Sea Urchin
                if (unqualifiedId == "397")
                {
                    category = SObject.FishCategory;
                }

                switch (category)
                {
                    case SObject.FruitsCategory:
                        // Jelly
                        AddPlural(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Jelly_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:Jelly_Flavored_Name", localizedName, localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)344", unqualifiedId));

                        // Wine
                        AddPlural(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Wine_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:Wine_Flavored_Name", localizedName, localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)348", unqualifiedId));

                        // Dried Fruit (not grapes)
                        if (unqualifiedId != "398")
                        {
                            AddPlural(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:DriedFruit_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                                ?? Game1.content.LoadString("Strings\\Objects:DriedFruit_Flavored_Name", localizedName, localizedName),
                                FlavoredItemHelper.EncodeFlavoredItemId("(O)DriedFruit", unqualifiedId));
                        }
                        break;
                    case SObject.GreensCategory when objectData.ContextTags?.Contains("edible_mushroom") == true:
                        //Dried Mushrooms
                        AddPlural(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:DriedFruit_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:DriedFruit_Flavored_Name", localizedName, localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)DriedMushrooms", unqualifiedId));
                        break;
                    case SObject.GreensCategory when objectData.ContextTags?.Contains("preserves_pickle") == true:
                        // Special Pickles
                        AddPlural(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Pickles_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:Pickles_Flavored_Name", localizedName, localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)342", unqualifiedId));
                        break;
                    case SObject.VegetableCategory:
                        // Pickles
                        AddPlural(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Pickles_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:Pickles_Flavored_Name", localizedName, localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)342", unqualifiedId));

                        // Juice
                        AddPlural(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Juice_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:Juice_Flavored_Name", localizedName, localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)350", unqualifiedId));
                        break;
                    case SObject.flowersCategory:
                        // Honey
                        AddPlural(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Honey_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:Honey_Flavored_Name", localizedName, localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)340", unqualifiedId));
                        break;
                    case SObject.FishCategory:
                        // Roe
                        Add(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:Roe_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:Roe_Flavored_Name", localizedName.TrimEnd('鱼'), localizedName.TrimEnd('鱼')),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)812", unqualifiedId));

                        // Aged Roe (not sturgeon)
                        if (unqualifiedId != "698")
                        {
                            Add(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:AgedRoe_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                                ?? Game1.content.LoadString("Strings\\Objects:AgedRoe_Flavored_Name", localizedName.TrimEnd('鱼'), localizedName.TrimEnd('鱼')),
                                FlavoredItemHelper.EncodeFlavoredItemId("(O)447", unqualifiedId));
                        }

                        // Bait
                        Add(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:SpecificBait_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:SpecificBait_Flavored_Name", localizedName, localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)SpecificBait", unqualifiedId));

                        // Smoked Fish
                        AddPlural(Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:SmokedFish_Flavored_(O){unqualifiedId}_Name", localizedName, localizedName, localeFallback: false)
                            ?? Game1.content.LoadString("Strings\\Objects:SmokedFish_Flavored_Name", localizedName, localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)SmokedFish", unqualifiedId));
                        break;
                }
            }

            /// <summary>Add alternative item ID matches to specific translation groups for convenience.</summary>
            /// <param name="itemId">Qualified item ID.</param>
            /// <param name="objectData">Object data.</param>
            private void AddConvenienceAlternates(string itemId, ObjectData objectData)
            {
                if (objectData.ContextTags is not List<string> contextTags)
                {
                    return;
                }

                switch (objectData.Category)
                {
                    case SObject.EggCategory when contextTags.Contains("large_egg_item"):
                        Add(Game1.content.LoadString("Strings\\Objects:WhiteEgg_Name"), itemId);
                        break;
                    case SObject.MilkCategory when contextTags.Contains("cow_milk_item") && contextTags.Contains("large_milk_item"):
                        Add(Game1.content.LoadString("Strings\\Objects:Milk_Name"), itemId);
                        break;
                    case SObject.MilkCategory when contextTags.Contains("goat_milk_item") && contextTags.Contains("large_milk_item"):
                        Add(Game1.content.LoadString("Strings\\Objects:GoatMilk_Name"), itemId);
                        break;
                }
            }
        }

        /// <summary>Maps localized character names to their internal names.</summary>
        private class LocalizedNpcMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
            : LocalizedGameDataMap("alias.npcs.", translation, settings, monitor)
        {
            protected override void PopulateDataMap()
            {
                foreach (KeyValuePair<string, CharacterData> pair in Game1.characterData)
                {
                    if (pair.Value?.CanReceiveGifts == true && Game1.NPCGiftTastes.ContainsKey(pair.Key))
                    {
                        Add(TokenParser.ParseText(pair.Value.DisplayName) ?? pair.Key, pair.Key);
                    }
                }
            }
        }

        /// <summary>Maps localized building names to their internal names.</summary>
        private class LocalizedBuildingMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
            : LocalizedGameDataMap("alias.buildings.", translation, settings, monitor)
        {
            private static readonly HashSet<string> IgnoredBuildingNames = ["Greenhouse"];

            protected override void PopulateDataMap()
            {
                foreach (KeyValuePair<string, BuildingData> pair in Game1.buildingData)
                {
                    if (!IgnoredBuildingNames.Contains(pair.Key))
                    {
                        AddPlural(TokenParser.ParseText(pair.Value?.Name) ?? pair.Key, pair.Key);
                    }
                }
            }
        }

        /// <summary>Maps localized farm animal shop names to their internal names, including alterative variants.</summary>
        private class LocalizedFarmAnimalMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
            : LocalizedGameDataMap("alias.animalshop.", translation, settings, monitor)
        {
            protected override void PopulateDataMap()
            {
                foreach (KeyValuePair<string, FarmAnimalData> pair in Game1.farmAnimalData)
                {
                    if (pair.Value?.ShopDisplayName is string shopDisplayName)
                    {
                        string localizedKey = TokenParser.ParseText(shopDisplayName) ?? pair.Key;

                        AddPlural(localizedKey, pair.Key);

                        if (pair.Value.AlternatePurchaseTypes is List<AlternatePurchaseAnimals> alternates)
                        {
                            foreach (var alternate in alternates)
                            {
                                foreach (string id in alternate.AnimalIds)
                                {
                                    AddPlural(localizedKey, id);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
