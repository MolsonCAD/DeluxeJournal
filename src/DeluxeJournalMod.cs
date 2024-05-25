using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Api;
using DeluxeJournal.Framework;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Framework.Events;
using DeluxeJournal.Framework.Task;
using DeluxeJournal.Menus;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Patching;
using DeluxeJournal.Task;
using DeluxeJournal.Task.Tasks;

namespace DeluxeJournal
{
    /// <summary>The mod entry point.</summary>
    internal class DeluxeJournalMod : Mod
    {
        /// <summary>Data key for the notes save data.</summary>
        public const string NotesDataKey = "notes-data";

        /// <summary>Data key for the tasks save data.</summary>
        public const string TasksDataKey = "tasks-data";

        /// <summary>Data assets file path.</summary>
        public const string DataPath = "assets/data";

        /// <summary>Default color data file path.</summary>
        public const string ColorDataPath = "assets/data/colors-default.json";

        /// <summary>UI spirte sheet texture.</summary>
        public static Texture2D? UiTexture { get; private set; }

        /// <summary>Animal icon spirte sheet texture.</summary>
        public static Texture2D? AnimalIconsTexture { get; private set; }

        /// <summary>Building icon spirte sheet texture.</summary>
        public static Texture2D? BuildingIconsTexture { get; private set; }

        /// <summary>Transparency mask for a colored task entry.</summary>
        public static Texture2D? ColoredTaskMask { get; private set; }

        /// <summary>Loaded color schema data.</summary>
        public static IList<ColorSchema> ColorSchemas { get; private set; } = Array.Empty<ColorSchema>();

        /// <summary>The color of the task entry border.</summary>
        public static Color TaskBorderColor { get; private set; } = new(68, 18, 28);

        /// <summary>
        /// Check if this is the main screen. Returns <c>false</c> if this is a co-op player
        /// while playing in split-screen mode, and <c>true</c> otherwise.
        /// </summary>
        public static bool IsMainScreen => !Context.IsSplitScreen || Context.ScreenId == 0;

        /// <summary>The mod instance.</summary>
        public static DeluxeJournalMod? Instance { get; private set; }

        /// <summary>Translation helper.</summary>
        public static ITranslationHelper? Translation { get; private set; }

        /// <summary>Notes save data.</summary>
        private NotesData? NotesData { get; set; }

        /// <summary>Configuration settings.</summary>
        public Config? Config { get; private set; }

        /// <summary>Event manager for handling event subscriptions.</summary>
        public EventManager? EventManager { get; private set; }

        /// <summary>Task manager.</summary>
        public TaskManager? TaskManager { get; private set; }

        /// <summary>Page manager for accessing journal pages.</summary>
        public PageManager? PageManager { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Translation = helper.Translation;

            RuntimeHelpers.RunClassConstructor(typeof(TaskTypes).TypeHandle);

            UiTexture = helper.ModContent.Load<Texture2D>("assets/ui.png");
            AnimalIconsTexture = helper.ModContent.Load<Texture2D>("assets/animal-icons.png");
            BuildingIconsTexture = helper.ModContent.Load<Texture2D>("assets/building-icons.png");
            ColoredTaskMask = helper.ModContent.Load<Texture2D>("assets/colored-task-mask.png");
            SmartIconComponent.AnimalIconIds = helper.ModContent.Load<Dictionary<string, int>>("assets/data/animal-icons.json");
            SmartIconComponent.BuildingIconData = helper.ModContent.Load<Dictionary<string, BuildingIconData>>("assets/data/building-icons.json");
            Config = helper.ReadConfig<Config>();
            NotesData = helper.Data.ReadGlobalData<NotesData>(NotesDataKey) ?? new NotesData();

            EventManager = new EventManager(helper.Events, helper.Multiplayer, Monitor);
            TaskManager = new TaskManager(new TaskEvents(EventManager), helper.Data, Config, ModManifest.Version);
            PageManager = new PageManager();

            PageManager.RegisterPage("quests", (bounds) => new QuestLogPage("quests", bounds, UiTexture, helper.Translation), 999);
            PageManager.RegisterPage("tasks", (bounds) => new TasksPage("tasks", bounds, UiTexture, helper.Translation), 998);
            PageManager.RegisterPage("notes", (bounds) => new NotesPage("notes", bounds, UiTexture, helper.Translation), 997);
            PageManager.RegisterPage("overlays", (bounds) => new OverlaysPage("overlays", bounds, UiTexture, helper.Translation), 996);

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;

            Patcher.Apply(new Harmony(ModManifest.UniqueID), Monitor,
                new QuestLogPatch(Monitor),
                new FarmerPatch(EventManager, Monitor),
                new CarpenterMenuPatch(EventManager, Monitor),
                new PurchaseAnimalsMenuPatch(EventManager, Monitor),
                new ShopMenuPatch(EventManager, Monitor)
            );

            ConsoleCommands.AddCommands(helper.ConsoleCommands);
        }

        public override object GetApi()
        {
            return new DeluxeJournalApi(this);
        }

        /// <summary>Get the stored notes page text.</summary>
        public string GetNotes()
        {
            if (NotesData != null && Constants.SaveFolderName != null && NotesData.Text.ContainsKey(Constants.SaveFolderName))
            {
                return NotesData.Text[Constants.SaveFolderName];
            }

            return string.Empty;
        }

        /// <summary>Save the notes page text.</summary>
        public void SaveNotes(string text)
        {
            if (NotesData != null && Constants.SaveFolderName != null)
            {
                NotesData.Text[Constants.SaveFolderName] = text;
                Helper.Data.WriteGlobalData(NotesDataKey, NotesData);
            }
        }

        /// <summary>Load the color schemas.</summary>
        /// <param name="relativePath">Optional file path relative to the mod folder. If <c>null</c>, attempts to load a custom file first, then falls back on the default.</param>
        public void LoadColorSchemas(string? relativePath = null)
        {
            if (relativePath == null)
            {
                IEnumerable<string> paths = Directory.GetFiles(Helper.DirectoryPath + "/" + DataPath, "colors-*")
                    .Select(f => $"{DataPath}/{Path.GetFileName(f)}")
                    .Where(f => f != ColorDataPath);

                foreach (string path in paths)
                {
                    if (Helper.Data.ReadJsonFile<ColorData>(path)?.Colors is IList<ColorSchema> customColors)
                    {
                        ColorSchemas = customColors;
                        goto ExtractDefault;
                    }
                    else
                    {
                        Monitor.Log($"Unable to load color data from '{path}' ... skipping", LogLevel.Warn);
                    }
                }

                relativePath = ColorDataPath;
            }

            if (Helper.Data.ReadJsonFile<ColorData>(relativePath)?.Colors is IList<ColorSchema> loadedColors)
            {
                ColorSchemas = loadedColors;
            }
            else
            {
                throw new ContentLoadException($"Could not load color data from file: {relativePath}");
            }

        ExtractDefault:
            if (Game1.mouseCursors != null)
            {
                ColorSchemas.Insert(0, ColorSchema.ExtractFromTextureBox(Game1.mouseCursors, new(384, 396, 15, 15), out Color border));
                TaskBorderColor = border;
            }
            else
            {
                ColorSchemas.Insert(0, new ColorSchema(Color.White, Color.LightGray, Color.DarkGray, Color.Black, Color.DarkGray));
                Monitor.Log("Color schemas loaded before game textures. Unable to extract default color schema.\n\tTry running the following command: dj_colors_load", LogLevel.Error);
            }
        }

        /// <summary>Save the color schemas.</summary>
        /// <param name="relativePath">Optional file path relative to the mod folder. Overwrites default file if <c>null</c>.</param>
        public void SaveColorSchemas(string? relativePath = null)
        {
            Helper.Data.WriteJsonFile(relativePath ?? ColorDataPath, new ColorData(ColorSchemas.Skip(1).ToList()));
        }

        [EventPriority(EventPriority.Low)]
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // Hijack QuestLog and replace it with DeluxeJournalMenu
            if (PageManager != null && Game1.activeClickableMenu is QuestLog questLog)
            {
                DeluxeJournalMenu deluxeJournalMenu = new DeluxeJournalMenu(PageManager);
                deluxeJournalMenu.SetQuestLog(questLog);
                Game1.activeClickableMenu = deluxeJournalMenu;
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (IsMainScreen)
            {
                LoadColorSchemas();
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (IsMainScreen)
            {
                TaskManager!.Load();
            }

            if (PageManager != null && !Game1.onScreenMenus.OfType<JournalButton>().Any())
            {
                Game1.onScreenMenus.Add(new JournalButton());
            }
        }

        private void OnSaving(object? sender, SavingEventArgs e)
        {
            if (Config != null)
            {
                Helper.WriteConfig(Config);
            }

            if (IsMainScreen)
            {
                TaskManager!.Save();
            }
        }
    }
}
