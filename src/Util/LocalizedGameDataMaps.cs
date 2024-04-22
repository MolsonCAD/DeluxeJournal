using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Characters;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Tools;
using StardewValley.GameData.Weapons;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TokenizableStrings;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Util
{
    /// <summary>Provides a collection of <see cref="LocalizedGameDataMap{T}"/> objects.</summary>
    public class LocalizedGameDataMaps
    {
        /// <inheritdoc cref="LocalizedItemMap"/>
        public LocalizedGameDataMap LocalizedItems { get; }

        /// <inheritdoc cref="LocalizedNpcMap"/>
        public LocalizedGameDataMap LocalizedNpcs { get; }

        /// <inheritdoc cref="LocalizedBuildingMap"/>
        public LocalizedGameDataMap LocalizedBuildings { get; }

        /// <inheritdoc cref="LocalizedFarmAnimalMap"/>
        public LocalizedGameDataMap LocalizedFarmAnimals { get; }

        public LocalizedGameDataMaps(ITranslationHelper translation) : this(translation, new())
        {
        }

        public LocalizedGameDataMaps(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor = null)
        {
            LocalizedItems = new LocalizedItemMap(translation, settings, monitor);
            LocalizedNpcs = new LocalizedNpcMap(translation, settings, monitor);
            LocalizedBuildings = new LocalizedBuildingMap(translation, settings, monitor);
            LocalizedFarmAnimals = new LocalizedFarmAnimalMap(translation, settings, monitor);
        }

        /// <summary>Maps localized item names to qualified item ID's.</summary>
        private class LocalizedItemMap : LocalizedGameDataMap
        {
            private static readonly HashSet<string> IgnoredItemIds = new() { "73", "858", "892", "922", "923", "924", "925", "927", "929", "930", "DriedFruit", "DriedMushrooms" };
            private static readonly HashSet<string> IgnoredItemTypes = new() { "Litter", "Quest", "asdf", "interactive" };

            public LocalizedItemMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
                : base("alias.items.", translation, settings, monitor)
            {
            }

            protected override void PopulateDataMap()
            {
                if (Settings.AllItemsEnabled || Settings.SetItemCategoryObject || Settings.SetItemCategoryCraftable)
                {
                    HashSet<string> roeFishNames = DataLoader.FishPondData(Game1.content)
                        .Where(data => data.ProducedItems.Any(reward => reward.ItemId == "(O)812"))
                        .Select(data => data.Id)
                        .ToHashSet();

                    foreach (KeyValuePair<string, ObjectData> pair in Game1.objectData)
                    {
                        if (!IgnoredItemIds.Contains(pair.Key) && !IgnoredItemTypes.Contains(pair.Value.Type))
                        {
                            if (!Settings.SetItemCategoryObject && Settings.SetItemCategoryCraftable
                                && !CraftingRecipe.craftingRecipes.ContainsKey(pair.Value.Name))
                            {
                                continue;
                            }

                            string parsedName = TokenParser.ParseText(pair.Value.DisplayName);

                            if (pair.Value.Category == SObject.artisanGoodsCategory)
                            {
                                Add(parsedName, ItemRegistry.type_object + pair.Key);
                            }
                            else
                            {
                                AddPlural(parsedName, ItemRegistry.type_object + pair.Key);
                            }

                            AddFlavored(parsedName, pair.Key, pair.Value, roeFishNames);
                            AddConvenienceAlternates(ItemRegistry.type_object + pair.Key, pair.Value);
                        }
                    }
                }

                if (Settings.AllItemsEnabled || Settings.SetItemCategoryBigCraftable)
                {
                    foreach (KeyValuePair<string, BigCraftableData> pair in Game1.bigCraftableData)
                    {
                        AddPlural(TokenParser.ParseText(pair.Value.DisplayName), ItemRegistry.type_bigCraftable + pair.Key);
                    }
                }

                if (Settings.AllItemsEnabled || Settings.SetItemCategoryTool)
                {
                    foreach (string toolId in Game1.toolData.Keys)
                    {
                        string toolQid = ItemRegistry.type_tool + toolId;
                        ParsedItemData itemData = ItemRegistry.GetData(toolQid);

                        if (itemData.RawData is ToolData toolData
                            && toolData.ApplyUpgradeLevelToDisplayName
                            && toolData.UpgradeLevel > 0
                            && ItemRegistry.Create(toolId) is Tool tool)
                        {
                            Add(tool.DisplayName.ToLower(), toolQid);
                        }
                        else
                        {
                            Add(itemData.DisplayName.ToLower(), toolQid);
                        }
                    }

                    Add(TokenParser.ParseText("[LocalizedText Strings\\1_6_Strings:PanToolBaseName]"), ItemRegistry.type_tool + "Pan");
                    Add(TokenParser.ParseText("[LocalizedText Strings\\\\StringsFromCSFiles:TrashCan]"), ItemRegistry.type_tool + "CopperTrashCan");
                }

                if (Settings.AllItemsEnabled)
                {
                    IDictionary<string, string> bootsData = DataLoader.Boots(Game1.content);
                    IDictionary<string, string> hatsData = DataLoader.Hats(Game1.content);

                    foreach (KeyValuePair<string, WeaponData> pair in Game1.weaponData)
                    {
                        Add(TokenParser.ParseText(pair.Value.DisplayName), ItemRegistry.type_weapon + pair.Key);
                    }

                    foreach (KeyValuePair<string, string> pair in bootsData)
                    {
                        string[] fields = pair.Value.Split('/');
                        Add(fields[fields.Length < 7 ? 0 : 6], ItemRegistry.type_boots + pair.Key);
                    }

                    foreach (KeyValuePair<string, string> pair in hatsData)
                    {
                        Add(pair.Value.Split('/')[5], ItemRegistry.type_hat + pair.Key);
                    }
                }
            }

            /// <summary>Populate data map with flavored items.</summary>
            /// <param name="localizedName">Localized ingredient name.</param>
            /// <param name="itemId">Unqualified item ID.</param>
            /// <param name="objectData">Object data.</param>
            /// <param name="roeFishNames">Set of fish (internal) names that may produce roe.</param>
            private void AddFlavored(string localizedName, string itemId, ObjectData objectData, IReadOnlySet<string> roeFishNames)
            {
                // Wild Honey
                if (itemId == "340")
                {
                    Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12750"), ItemRegistry.type_object + itemId);
                    return;
                }

                switch (objectData.Category)
                {
                    case SObject.FruitsCategory:
                        // Jelly
                        Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12739", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)344", itemId));

                        // Wine
                        AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12730", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)348", itemId));

                        // Dried Fruit (not grapes)
                        if (itemId != "398")
                        {
                            AddPlural(Game1.content.LoadString("Strings\\1_6_Strings:DriedFruit_DisplayName", localizedName),
                                FlavoredItemHelper.EncodeFlavoredItemId("(O)DriedFruit", itemId),
                                true);
                        }
                        break;
                    case SObject.GreensCategory when objectData.ContextTags.Contains("edible_mushroom"):
                        //Dried Mushrooms
                        AddPlural(Game1.content.LoadString("Strings\\1_6_Strings:DriedFruit_DisplayName", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)DriedMushrooms", itemId),
                            true);
                        break;
                    case SObject.GreensCategory when objectData.ContextTags.Contains("preserves_pickle"):
                        // Special Pickles
                        Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12735", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)342", itemId));
                        break;
                    case SObject.VegetableCategory:
                        // Pickles
                        Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12735", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)342", itemId));

                        // Juice
                        AddPlural(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12726", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)350", itemId));
                        break;
                    case SObject.flowersCategory:
                        // Honey
                        Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12760", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)340", itemId));
                        break;
                    case SObject.FishCategory:
                        if (roeFishNames.Contains(objectData.Name))
                        {
                            // Roe
                            Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Roe_DisplayName", localizedName),
                                FlavoredItemHelper.EncodeFlavoredItemId("(O)812", itemId));

                            // Aged Roe
                            Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:AgedRoe_DisplayName", localizedName),
                                FlavoredItemHelper.EncodeFlavoredItemId("(O)447", itemId));
                        }

                        // Bait
                        Add(Game1.content.LoadString("Strings\\1_6_Strings:SpecificBait_DisplayName", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)SpecificBait", itemId));

                        // Smoked Fish
                        Add(Game1.content.LoadString("Strings\\1_6_Strings:SmokedFish_DisplayName", localizedName),
                            FlavoredItemHelper.EncodeFlavoredItemId("(O)SmokedFish", itemId));
                        break;
                }
            }

            /// <summary>Add alternative item ID matches to specific translation groups for convenience.</summary>
            /// <param name="itemId">Qualified item ID.</param>
            /// <param name="objectData">Object data.</param>
            private void AddConvenienceAlternates(string itemId, ObjectData objectData)
            {
                switch (objectData.Category)
                {
                    case SObject.EggCategory when objectData.ContextTags.Contains("large_egg_item"):
                        Add(TokenParser.ParseText("[LocalizedText Strings\\\\Objects:WhiteEgg_Name]"), itemId);
                        break;
                    case SObject.MilkCategory when objectData.ContextTags.Contains("cow_milk_item") && objectData.ContextTags.Contains("large_milk_item"):
                        Add(TokenParser.ParseText("[LocalizedText Strings\\\\Objects:Milk_Name]"), itemId);
                        break;
                    case SObject.MilkCategory when objectData.ContextTags.Contains("goat_milk_item") && objectData.ContextTags.Contains("large_milk_item"):
                        Add(TokenParser.ParseText("[LocalizedText Strings\\\\Objects:GoatMilk_Name]"), itemId);
                        break;
                }
            }
        }

        /// <summary>Maps localized character names to their internal names.</summary>
        private class LocalizedNpcMap : LocalizedGameDataMap
        {
            public LocalizedNpcMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
                : base("alias.npcs.", translation, settings, monitor)
            {
            }

            protected override void PopulateDataMap()
            {
                foreach (KeyValuePair<string, CharacterData> pair in Game1.characterData)
                {
                    if (pair.Value.CanReceiveGifts && Game1.NPCGiftTastes.ContainsKey(pair.Key))
                    {
                        Add(TokenParser.ParseText(pair.Value.DisplayName), pair.Key);
                    }
                }
            }
        }

        /// <summary>Maps localized building names to their internal names.</summary>
        private class LocalizedBuildingMap : LocalizedGameDataMap
        {
            private static readonly HashSet<string> IgnoredBuildingNames = new() { "Greenhouse" };

            public LocalizedBuildingMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
                : base("alias.buildings.", translation, settings, monitor)
            {
            }

            protected override void PopulateDataMap()
            {
                foreach (KeyValuePair<string, BuildingData> pair in Game1.buildingData)
                {
                    if (!IgnoredBuildingNames.Contains(pair.Key))
                    {
                        string localizedKey = TokenParser.ParseText(pair.Value.Name);

                        AddPlural(localizedKey, pair.Key);
                    }
                }
            }
        }

        /// <summary>Maps localized farm animal shop names to their internal names, including alterative variants.</summary>
        private class LocalizedFarmAnimalMap : LocalizedGameDataMap
        {
            public LocalizedFarmAnimalMap(ITranslationHelper translation, TaskParserSettings settings, IMonitor? monitor)
                : base("alias.animalshop.", translation, settings, monitor)
            {
            }

            protected override void PopulateDataMap()
            {
                foreach (KeyValuePair<string, FarmAnimalData> pair in Game1.farmAnimalData)
                {
                    if (pair.Value.ShopDisplayName is string shopDisplayName)
                    {
                        string localizedKey = TokenParser.ParseText(shopDisplayName).ToLower();

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
