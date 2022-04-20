using System.Runtime.CompilerServices;
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
using DeluxeJournal.Framework.Tasks;
using DeluxeJournal.Menus;
using DeluxeJournal.Patching;

namespace DeluxeJournal
{
    /// <summary>The mod entry point.</summary>
    internal class DeluxeJournalMod : Mod
    {
        public const string NOTES_DATA_KEY = "notes-data";
        public const string TASKS_DATA_KEY = "tasks-data";

        private static DeluxeJournalMod? _instance;

        public static DeluxeJournalMod? Instance => _instance;

        public static Texture2D? UiTexture { get; private set; }

        public static Texture2D? CharacterIconsTexture { get; private set; }

        public static bool IsMainScreen => !Context.IsSplitScreen || Context.ScreenId == 0;

        private NotesData? _notesData;

        public Config? Config { get; private set; }

        public EventManager? EventManager { get; private set; }

        public TaskManager? TaskManager { get; private set; }

        public PageManager? PageManager { get; private set; }

        public override void Entry(IModHelper helper)
        {
            _instance = this;

            RuntimeHelpers.RunClassConstructor(typeof(TaskTypes).TypeHandle);

            UiTexture = helper.Content.Load<Texture2D>("assets/ui.png");
            CharacterIconsTexture = helper.Content.Load<Texture2D>("assets/character-icons.png");
            Config = helper.ReadConfig<Config>();
            _notesData = helper.Data.ReadGlobalData<NotesData>(NOTES_DATA_KEY) ?? new NotesData();

            EventManager = new EventManager(helper.Events, helper.Multiplayer, Monitor);
            TaskManager = new TaskManager(new TaskEvents(EventManager), helper.Data);
            PageManager = new PageManager();

            PageManager.RegisterPage("quests", (bounds) => new QuestLogPage(bounds, UiTexture, helper.Translation), 102);
            PageManager.RegisterPage("tasks", (bounds) => new TasksPage(bounds, UiTexture, helper.Translation), 101);
            PageManager.RegisterPage("notes", (bounds) => new NotesPage(bounds, UiTexture, helper.Translation), 100);

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;

            Patcher.Apply(new Harmony(ModManifest.UniqueID), Monitor,
                new FarmerPatch(EventManager, Monitor),
                new UtilityPatch(EventManager, Monitor),
                new CarpenterMenuPatch(EventManager, Monitor),
                new QuestLogPatch(Monitor)
            );
        }

        public override object GetApi()
        {
            return new DeluxeJournalApi(this);
        }

        public string GetNotes()
        {
            if (_notesData != null && _notesData.Text.ContainsKey(Constants.SaveFolderName))
            {
                return _notesData.Text[Constants.SaveFolderName];
            }

            return string.Empty;
        }

        public void SaveNotes(string text)
        {
            if (_notesData != null)
            {
                _notesData.Text[Constants.SaveFolderName] = text;
                Helper.Data.WriteGlobalData(NOTES_DATA_KEY, _notesData);
            }
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

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (IsMainScreen)
            {
                TaskManager?.Load();
            }

            if (PageManager != null && !Game1.onScreenMenus.OfType<JournalButton>().Any())
            {
                Game1.onScreenMenus.Add(new JournalButton(Helper.Translation));
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
                TaskManager?.Save();
            }
        }
    }
}
