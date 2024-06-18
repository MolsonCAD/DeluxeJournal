using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Task;

namespace DeluxeJournal.Menus.Components
{
    public class ColorPickerComponent : IClickableComponentSupplier
    {
        private const int SliderWidth = 168;
        private const int SliderHeight = 20;
        private const int ColorIconSpacing = 64;

        private readonly ColorSliderBar _hueBar;
        private readonly ColorSliderBar _saturationBar;
        private readonly ColorSliderBar _valueBar;
        private readonly ColorSliderBar _alphaBar;

        private Rectangle _bounds;
        private ColorSliderBar? _heldSlider;
        private bool _enableAlphaSlider;

        /// <summary>Bounding box for this component.</summary>
        public Rectangle Bounds => _bounds;

        /// <summary>The color with RGB values normalized by the alpha channel for alpha blend mode.</summary>
        public Color AlphaBlendColor
        {
            get => ColorSchema.HSVToColor(_hueBar.Value * 360f, _saturationBar.Value, _valueBar.Value) * (_alphaBar.Value * MaxAlpha);

            set
            {
                ColorSchema.ColorToHSV(value * (255f / value.A), out float hue, out float saturation, out float _value);

                _hueBar.Value = hue / 360f;
                _saturationBar.Value = saturation;
                _valueBar.Value = _value;
                _alphaBar.Value = Math.Min(value.A / 255f, MaxAlpha);
            }
        }

        /// <summary>Whether the alpha channel slider bar is enabled.</summary>
        public bool EnableAlphaSlider
        {
            get => _enableAlphaSlider;

            set
            {
                _enableAlphaSlider = value;
                _alphaBar.visible = value;
                _bounds.Height = _hueBar.bounds.Height * (value ? 4 : 3);
            }
        }

        /// <summary>Maximum alpha value for the alpha slider range.</summary>
        public float MaxAlpha { get; set; } = 1f;

        public ColorPickerComponent(int x, int y, int myId = -500)
        {
            _bounds = new(x, y, SliderWidth + ColorIconSpacing, SliderHeight * 3);
            _hueBar = new ColorSliderBar(x + ColorIconSpacing, y, SliderWidth, SliderHeight);
            _saturationBar = new ColorSliderBar(x + ColorIconSpacing, y + SliderHeight, SliderWidth, SliderHeight);
            _valueBar = new ColorSliderBar(x + ColorIconSpacing, y + SliderHeight * 2, SliderWidth, SliderHeight);
            _alphaBar = new ColorSliderBar(x + ColorIconSpacing, y + SliderHeight * 3, SliderWidth, SliderHeight, value: 1f)
            {
                visible = false
            };
        }

        public IEnumerable<ClickableComponent> GetClickableComponents()
        {
            yield return _hueBar;
            yield return _saturationBar;
            yield return _valueBar;
            yield return _alphaBar;
        }

        public void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_hueBar.containsPoint(x, y))
            {
                _hueBar.ReceiveLeftClick(x, y);
                _heldSlider = _hueBar;
            }
            else if (_saturationBar.containsPoint(x, y))
            {
                _saturationBar.ReceiveLeftClick(x, y);
                _heldSlider = _saturationBar;
            }
            else if (_valueBar.containsPoint(x, y))
            {
                _valueBar.ReceiveLeftClick(x, y);
                _heldSlider = _valueBar;
            }
            else if (_alphaBar.containsPoint(x, y))
            {
                _alphaBar.ReceiveLeftClick(x, y);
                _heldSlider = _alphaBar;
            }
        }

        public void LeftClickHeld(int x, int y)
        {
            _heldSlider?.LeftClickHeld(x, y);
        }

        public void ReleaseLeftClick(int x, int y)
        {
            _heldSlider = null;
        }

        public void Draw(SpriteBatch b, bool greyed = false)
        {
            float hue = _hueBar.Value * 360f;
            float saturation = _saturationBar.Value;
            float value = _valueBar.Value;
            float alphaEffect = greyed ? 0.5f : 1f;
            Color color = AlphaBlendColor;

            Utility.drawWithShadow(b, DeluxeJournalMod.UiTexture, new(_bounds.X, _bounds.Y + 6), new(64, 64, 12, 12), greyed ? Color.DarkGray : Color.White, 0f, Vector2.Zero, 4f);
            b.Draw(Game1.staminaRect, new Rectangle(_bounds.X + 8, _bounds.Y + 14, 32, 32), Color.Gray);

            if (color.A < 255)
            {
                b.Draw(DeluxeJournalMod.UiTexture, new Rectangle(_bounds.X + 8, _bounds.Y + 14, 32, 32), new(56, 72, 8, 8), Color.White * alphaEffect);
            }

            b.Draw(Game1.staminaRect, new Rectangle(_bounds.X + 8, _bounds.Y + 14, 32, 32), color * alphaEffect);

            _hueBar.DrawHueBar(b, alphaEffect, greyed);
            _saturationBar.DrawSaturationBar(b, hue, value, alphaEffect, greyed);
            _valueBar.DrawValueBar(b, hue, saturation, alphaEffect, greyed);

            if (_alphaBar.visible)
            {
                _alphaBar.DrawAlphaBar(b, hue, saturation, value, MaxAlpha, greyed);
            }
        }
    }
}
