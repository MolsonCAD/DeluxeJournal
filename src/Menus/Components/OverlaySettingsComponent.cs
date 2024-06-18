using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace DeluxeJournal.Menus.Components
{
    public class OverlaySettingsComponent : IClickableComponentSupplier
    {
        private readonly ButtonComponent _visibleCheckBox;
        private readonly ButtonComponent _visibilityLockButton;
        private readonly ButtonComponent _colorCheckBox;
        private readonly ColorPickerComponent _colorPicker;

        private Rectangle _bounds;
        private bool _hovering;

        public Rectangle Bounds => _bounds;

        public IOverlay Overlay { get; set; }

        public string Label { get; set; }

        public OverlaySettingsComponent(IOverlay overlay, Rectangle bounds, string label, int myId)
        {
            _bounds = bounds;
            Overlay = overlay;
            Label = label;

            _visibleCheckBox = new ButtonComponent(
                new(bounds.X + 24, bounds.Y + 32, 36, 36),
                DeluxeJournalMod.UiTexture!,
                new(16, 16, 9, 9),
                4f)
            {
                Selected = Overlay.IsVisible,
                SoundCueName = "tinyWhip",
                SoundPitch = (self) => self.Selected ? 1000 : 2000,
                OnClick = (self) => Overlay.IsVisible = self.Toggle()
            };

            _visibilityLockButton = new ButtonComponent(
                new(bounds.X + 68, bounds.Y + 28, 28, 40),
                DeluxeJournalMod.UiTexture!,
                new(71, 16, 7, 10),
                4f)
            {
                Selected = Overlay.IsVisibilityLocked,
                SoundCueName = "tinyWhip",
                SoundPitch = (self) => self.Selected ? 1000 : 2000,
                OnClick = (self) => Overlay.IsVisibilityLocked = self.Toggle()
            };

            _colorCheckBox = new ButtonComponent(
                new(bounds.Right - 348, bounds.Y + 32, 36, 36),
                DeluxeJournalMod.UiTexture!,
                new(16, 16, 9, 9),
                4f)
            {
                visible = Overlay.IsColorOptional,
                Selected = Overlay.IsColorSelected,
                SoundCueName = "tinyWhip",
                SoundPitch = (self) => self.Selected ? 1000 : 2000,
                OnClick = (self) => Overlay.IsColorSelected = self.Toggle()
            };

            _colorPicker = new ColorPickerComponent(bounds.Right - 304, bounds.Y + 20)
            {
                AlphaBlendColor = Overlay.CustomColor
            };

            _bounds.Height = _colorPicker.Bounds.Height + 40;
        }

        public IEnumerable<ClickableComponent> GetClickableComponents()
        {
            yield return _visibleCheckBox;
            yield return _visibilityLockButton;
            yield return _colorCheckBox;

            foreach (var cc in _colorPicker.GetClickableComponents())
            {
                yield return cc;
            }
        }

        public void ApplySettings()
        {
            Overlay.CustomColor = _colorPicker.AlphaBlendColor;
        }

        public void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_visibleCheckBox.containsPoint(x, y))
            {
                _visibleCheckBox.ReceiveLeftClick(x, y, playSound);
            }
            else if (_visibilityLockButton.containsPoint(x, y))
            {
                _visibilityLockButton.ReceiveLeftClick(x, y, playSound);
            }
            else if (_colorCheckBox.containsPoint(x, y))
            {
                _colorCheckBox.ReceiveLeftClick(x, y, playSound);
            }
            else if (Overlay.IsColorSelected && _colorPicker.Bounds.Contains(x, y))
            {
                _colorPicker.ReceiveLeftClick(x, y, playSound);
            }
        }

        public void LeftClickHeld(int x, int y)
        {
            _colorPicker.LeftClickHeld(x, y);
        }

        public void ReleaseLeftClick(int x, int y)
        {
            _colorPicker.ReleaseLeftClick(x, y);
        }

        public bool TryHover(int x, int y)
        {
            return _hovering = Bounds.Contains(x, y);
        }

        public void Draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b,
                Game1.mouseCursors,
                new(384, 396, 15, 15),
                _bounds.X,
                _bounds.Y,
                _bounds.Width,
                _bounds.Height,
                _hovering ? Color.Wheat : Color.White,
                4f,
                false);

            SpriteText.drawString(b, Label, _visibilityLockButton.bounds.Right + 32, _visibilityLockButton.bounds.Y);

            _visibleCheckBox.draw(b, Color.White, 0.86f, _visibleCheckBox.Selected ? 1 : 0);
            _visibilityLockButton.draw(b, Color.White, 0.86f, _visibilityLockButton.Selected ? 1 : 0);
            _colorCheckBox.draw(b, Color.White, 0.86f, _colorCheckBox.Selected ? 1 : 0);

            if (Overlay.IsColorSelected || Overlay.IsColorOptional)
            {
                _colorPicker.Draw(b, !Overlay.IsColorSelected);
            }
        }
    }
}
