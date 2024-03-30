using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Minigames;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static DeluxeJournal.Util.ItemOptions;

namespace DeluxeJournal.Util
{
    /// <summary>Provides a means of querying game objects/data by their corresponding localized display names.</summary>
    public class LocalizedObjects
    {
        private readonly PlainTextMap<string> _items;
        private readonly PlainTextMap<string> _npcs;
        private readonly PlainTextMap<ToolHelper.ToolDescription> _tools;
        private readonly PlainTextMap<BlueprintInfo> _blueprints;

        public LocalizedObjects(ITranslationHelper translation)
        {
            _items = CreateItemMap();
            _npcs = CreateNPCMap();
            _tools = CreateToolMap();
            _blueprints = CreateBlueprintMap();
        }

        /// <summary>Get an Item by display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <param name="fuzzy">Perform a fuzzy search if true, otherwise only return an Item with the exact name.</param>
        public Item? GetItem(string localizedName, bool fuzzy = false)
        {
            ListWithDefault<ToolHelper.ToolDescription>? toolNames = GetValue(_tools, localizedName, fuzzy);
            if (toolNames != null)
            {
                ListWithDefault<Item> tools = new ListWithDefault<Item>();
                foreach (ToolHelper.ToolDescription tool in toolNames.items)
                {
                    Item? new_tool = ToolHelper.GetToolFromDescription(tool.index, tool.upgradeLevel);
                    if (new_tool != null) tools += new_tool;
                }
                return tools.defaultItem;
            }

            ListWithDefault<string>? itemNames = GetValue(_items, localizedName, fuzzy);
            if (itemNames != null)
            {
                ListWithDefault<Item> items = new ListWithDefault<Item>();
                foreach (string item in itemNames.items)
                {
                    items += ItemRegistry.Create(item);
                }

                return items.defaultItem;
            }
            return null;
        }

        /// <summary>Get an NPC by display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <param name="fuzzy">Perform a fuzzy search if true, otherwise only return the NPC with the exact name.</param>
        public NPC? GetNPC(string localizedName, bool fuzzy = false)
        {
            ListWithDefault<string>? npcNames = GetValue(_npcs, localizedName, fuzzy);
            if (npcNames != null)
            {
                ListWithDefault<NPC> npcs = new ListWithDefault<NPC>();
                foreach (string npc in npcNames.items)
                {
                    npcs += Game1.getCharacterFromName(npc);
                }

                return npcs.defaultItem;
            }
            return null;
        }

        /// <summary>Get BlueprintInfo given the display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <param name="fuzzy">Perform a fuzzy search if true, otherwise only match a blueprint with the exact name.</param>
        public BlueprintInfo GetBlueprintInfo(string localizedName, bool fuzzy = false)
        {
            ListWithDefault<BlueprintInfo>? values = GetValue(_blueprints, localizedName, fuzzy);
            return values != null ? values.defaultItem : null;
        }

        private static ListWithDefault<T>? GetValue<T>(PlainTextMap<T> map, string key, bool fuzzy) where T : class
        {
            key = key.Trim().ToLowerInvariant();
            key = fuzzy ? Utility.fuzzySearch(key, map.Keys.ToList()) : key;

            if (key != null && map.ContainsKey(key))
            {
                return map[key];
            }

            return null;
        }

        private static PlainTextMap<string> CreateItemMap()
        {
            IDictionary<string, string> furnitureData = DataLoader.Furniture(Game1.content);
            IDictionary<string, StardewValley.GameData.Weapons.WeaponData?> weaponData = DataLoader.Weapons(Game1.content);
            IDictionary<string, string> bootsData = DataLoader.Boots(Game1.content);
            IDictionary<string, string> hatsData = DataLoader.Hats(Game1.content);
            IDictionary<string, StardewValley.GameData.Objects.ObjectData?> objectsData = DataLoader.Objects(Game1.content);
            IDictionary<string, StardewValley.GameData.BigCraftables.BigCraftableData?> bigCraftablesData = DataLoader.BigCraftables(Game1.content);

            PlainTextMap<string> map = new PlainTextMap<string>();


            foreach (string itemID in furnitureData.Keys)
            {
                if (furnitureData[itemID] is string text)
                {
                    string plain_name = TokenParser.ParseText(text.Split('/')[7].Trim()).ToLowerInvariant();
                    string qualifiedID = "(F)" + itemID;

                    map.add(plain_name, qualifiedID);
                }
            }
            foreach (string itemID in weaponData.Keys)
            {
                if (weaponData[itemID] is StardewValley.GameData.Weapons.WeaponData data)
                {
                    if (TokenParser.ParseText(data.DisplayName) is string text)
                    {
                        string plain_name = text.ToLowerInvariant();
                        string qualifiedID = "(W)" + itemID;

                        map.add(plain_name, qualifiedID);
                    }
                }
            }
            foreach (string itemID in bootsData.Keys)
            {
                if (bootsData[itemID] is string text)
                {
                    string plain_name = TokenParser.ParseText(text.Split('/')[6].Trim()).ToLowerInvariant();
                    string qualifiedID = "(B)" + itemID;

                    map.add(plain_name, qualifiedID);
                }
            }
            foreach (string itemID in hatsData.Keys)
            {
                if (hatsData[itemID] is string text)
                {
                    string plain_name = TokenParser.ParseText(text.Split('/')[5].Trim()).ToLowerInvariant();
                    string qualifiedID = "(H)" + itemID;

                    map.add(plain_name, qualifiedID);
                }
            }
            foreach (string itemID in objectsData.Keys)
            {
                if (objectsData[itemID] is StardewValley.GameData.Objects.ObjectData data)
                {
                    if (TokenParser.ParseText(data.DisplayName) is string text)
                    {
                        string plain_name = text.ToLowerInvariant();
                        string qualifiedID = "(O)" + itemID;

                        if (qualifiedID == "(O)390")
                        {
                            // prioritise stone
                            map.addAsDefault(plain_name, qualifiedID);
                        }
                        else
                        {
                            map.add(plain_name, qualifiedID);
                        }

                    }
                }
            }
            foreach (string itemID in bigCraftablesData.Keys)
            {
                if (bigCraftablesData[itemID] is StardewValley.GameData.BigCraftables.BigCraftableData data){ 
                    if (TokenParser.ParseText(data.DisplayName) is string text)
                    {
                        string plain_name = text.ToLowerInvariant();
                        string qualifiedID = "(BC)" + itemID;

                        map.add(plain_name, qualifiedID);
                    }
                }
            }

            return map;
        }

        private static PlainTextMap<ToolHelper.ToolDescription> CreateToolMap()
        {
            // TODO: Change to a query
            PlainTextMap < ToolHelper.ToolDescription> map = new PlainTextMap<ToolHelper.ToolDescription>();
            Tool[] tools = {
                new Axe(),
                new Hoe(),
                new Pickaxe(),
                new WateringCan(),
                new FishingRod(),
                new Pan(),
                new Shears(),
                new MilkPail(),
                new Wand()
            };

            foreach (Tool tool in tools)
            {
                int maxLevel = 0;

                switch (tool.GetType().Name)
                {
                    case nameof(Axe):
                    case nameof(Hoe):
                    case nameof(Pickaxe):
                    case nameof(WateringCan):
                        maxLevel = Tool.iridium;
                        break;
                    case nameof(FishingRod):
                        maxLevel = 3;
                        break;
                }

                for (int level = 0; level <= maxLevel; level++)
                {
                    tool.UpgradeLevel = level;
                    map.add(tool.DisplayName.ToLowerInvariant(), ToolHelper.GetToolDescription(tool));
                }
            }

            return map;
        }

        private static PlainTextMap<string> CreateNPCMap()
        {
            // broken character mods make reading the data from them return null
            IDictionary<string, StardewValley.GameData.Characters.CharacterData?> npcData = DataLoader.Characters(Game1.content);
            PlainTextMap<string> map = new PlainTextMap<string> ();
            foreach (KeyValuePair<string, StardewValley.GameData.Characters.CharacterData?> pair in npcData)
            {
                if (pair.Value != null)
                {
                    map.add(TokenParser.ParseText(pair.Value.DisplayName).ToLowerInvariant(), pair.Key);
                }
            }

            return map;
        }

        private static PlainTextMap<BlueprintInfo> CreateBlueprintMap()
        {
            //TODO: Change this once I understand better how it uses the blueprintinfo
            IDictionary<string, StardewValley.GameData.Buildings.BuildingData?> blueprintData = DataLoader.Buildings(Game1.content);
            PlainTextMap<BlueprintInfo > map = new PlainTextMap<BlueprintInfo> ();
            foreach (KeyValuePair<string, StardewValley.GameData.Buildings.BuildingData?> pair in blueprintData)
            {
                if (pair.Value != null)
                {
                    map.add(TokenParser.ParseText(pair.Value.Name).ToLowerInvariant(), new BlueprintInfo(pair.Key, TokenParser.ParseText(pair.Value.Name), pair.Value.BuildingType, pair.Value.BuildCost));
                }
            }

            return map;
        }
    }
}
