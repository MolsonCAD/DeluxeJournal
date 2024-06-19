using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Menus;
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework
{
    internal class OverlayManager
    {
        /// <summary>Data key for the overlay settings.</summary>
        public const string OverlaySettingsKey = "overlay-settings";

        private readonly IDataHelper _dataHelper;
        private readonly Config _config;
        private readonly Dictionary<string, IOverlay> _overlays;

        private Dictionary<string, OverlaySettings>? _settings;
        private bool _toggleVisible;

        /// <summary>Active <see cref="IOverlay"/> menus by their registered page IDs.</summary>
        public IReadOnlyDictionary<string, IOverlay> Overlays => _overlays;

        /// <summary>Mapping of <see cref="OverlaySettings"/> by their registered page IDs.</summary>
        /// <remarks>Lazy instantiation to allow <see cref="Game1.options"/> to load first.</remarks>
        private Dictionary<string, OverlaySettings> Settings
        {
            get
            {
                if (_settings == null)
                {
                    Point defaultSize = new(500);
                    Point notesPosition = new(0, defaultSize.Y);

                    if (Game1.options is Options options)
                    {
                        notesPosition.Y = (int)Math.Ceiling(Game1.viewport.Height * options.zoomLevel / options.uiScale) - defaultSize.Y;
                    }

                    _settings = _dataHelper.ReadGlobalData<Dictionary<string, OverlaySettings>>(OverlaySettingsKey) ?? new()
                    {
                        { "tasks", new(new(Point.Zero, defaultSize), true, false, false, Color.White) },
                        { "notes", new(new(notesPosition, defaultSize), false, true, true, Color.White) }
                    };
                }

                return _settings;
            }
        }

        public OverlayManager(IModEvents events, IDataHelper dataHelper, Config config)
        {
            _dataHelper = dataHelper;
            _config = config;
            _overlays = new(2);

            if (ColorSchema.HexToColor(config.OverlayBackgroundColor) is Color backgroundColor)
            {
                IOverlay.BackgroundColor = backgroundColor;
            }

            events.Display.WindowResized += OnWindowResized;
            events.GameLoop.SaveLoaded += OnSaveLoaded;
            events.GameLoop.ReturnedToTitle += OnReturnToTitle;
            events.Input.ButtonsChanged += OnButtonsChanged;
        }

        /// <summary>Set the overlay background color.</summary>
        public void SetBackgroundColor(Color color)
        {
            IOverlay.BackgroundColor = color;
            _config.OverlayBackgroundColor = ColorSchema.ColorToHex(color, true);
            _config.Save();
        }

        /// <summary>Set the <see cref="IOverlay.IsEditing"/> flag in each visible overlay.</summary>
        /// <param name="editing">Whether the overlays are in edit-mode.</param>
        public void SetEditing(bool editing)
        {
            foreach (IOverlay overlay in _overlays.Values)
            {
                if (overlay.IsVisible)
                {
                    overlay.IsEditing = editing;
                }
            }
        }

        /// <summary>Save the overlay settings.</summary>
        public void SaveSettings()
        {
            UpdateSettings();
            _dataHelper.WriteGlobalData(OverlaySettingsKey, _settings);
        }

        /// <summary>Restore the state of each overlay to their saved settings.</summary>
        public void RestoreSettings()
        {
            foreach (string pageId in _overlays.Keys)
            {
                if (Settings.TryGetValue(pageId, out OverlaySettings? settings))
                {
                    settings.Apply(_overlays[pageId]);
                }
            }
        }

        /// <summary>Update the overlay settings with the state of each active overlay.</summary>
        private void UpdateSettings()
        {
            _toggleVisible = false;

            foreach (string pageId in _overlays.Keys)
            {
                if (!Settings.TryGetValue(pageId, out OverlaySettings? settings))
                {
                    settings = OverlaySettings.NewDefault();
                }

                IOverlay overlay = _overlays[pageId];
                settings.Update(overlay);
                Settings.TryAdd(pageId, settings);

                if (overlay.IsVisible && !overlay.IsVisibilityLocked)
                {
                    _toggleVisible = true;
                }
            }
        }

        private void OnWindowResized(object? sender, WindowResizedEventArgs e)
        {
            UpdateSettings();
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (Game1.activeClickableMenu == null && _config.ToggleOverlaysKeybind.JustPressed())
            {
                _toggleVisible = !_toggleVisible;

                foreach (var overlay in _overlays.Values)
                {
                    if (!overlay.IsVisibilityLocked)
                    {
                        overlay.IsVisible = _toggleVisible;
                    }
                }

                Game1.playSound(_toggleVisible ? "breathin" : "breathout", 1800);
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            DisposeOverlays();
            _toggleVisible = false;

            foreach (string pageId in PageRegistry.Keys)
            {
                if (!Settings.TryGetValue(pageId, out OverlaySettings? settings))
                {
                    settings = OverlaySettings.NewDefault();
                }

                if (PageRegistry.CreateOverlay(pageId, settings) is IOverlay overlay)
                {
                    Settings.TryAdd(pageId, settings);
                    _overlays.Add(pageId, overlay);
                    Game1.onScreenMenus.Add(overlay);

                    if (overlay.IsVisible && !overlay.IsVisibilityLocked)
                    {
                        _toggleVisible = true;
                    }
                }
            }
        }

        private void OnReturnToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            SaveSettings();
            DisposeOverlays();
        }

        private void DisposeOverlays()
        {
            foreach (IDisposable overlay in _overlays.Values)
            {
                overlay.Dispose();
            }

            _overlays.Clear();
        }
    }
}
