using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

using static DeluxeJournal.Task.ColorSchema;

namespace DeluxeJournal.Menus.Components
{
    public class ColorSliderBar : ClickableComponent
    {
        private const int CheckeredTileWidth = 5;

        private readonly int _barHeight;
        private float _value;

        /// <summary>Number of color samples to draw.</summary>
        public int Samples { get; set; } = 24;

        /// <summary>Percentage slider value in the range <c>[0f,1f]</c>.</summary>
        public float Value
        {
            get => _value;
            set => _value = Math.Clamp(value, 0f, 1f);
        }

        public ColorSliderBar(int x, int y, int width, int height, int barHeight = 8, float value = 0.5f)
            : base(new(x, y, width, height), string.Empty)
        {
            _barHeight = barHeight;
            Value = value;
        }

        public virtual void ReceiveLeftClick(int x, int y)
        {
            if (containsPoint(x, y))
            {
                Value = (x - bounds.X) / (float)bounds.Width;
            }
        }

        public virtual void LeftClickHeld(int x, int y)
        {
            Value = (x - bounds.X) / (float)bounds.Width;
        }

        public virtual void ReleaseLeftClick(int x, int y)
        {
        }

        public void DrawHueBar(SpriteBatch b, float alpha = 1f, bool greyed = false)
        {
            DrawSliderBar(b, -1f, 0.9f, 0.9f, alpha, greyed ? Game1.unselectedOptionColor : Game1.textColor);
        }

        public void DrawSaturationBar(SpriteBatch b, float hue, float value, float alpha = 1f, bool greyed = false)
        {
            DrawSliderBar(b, hue, -1f, value, alpha, greyed ? Game1.unselectedOptionColor : Game1.textColor);
        }

        public void DrawValueBar(SpriteBatch b, float hue, float saturation, float alpha = 1f, bool greyed = false)
        {
            DrawSliderBar(b, hue, saturation, -1f, alpha, greyed ? Game1.unselectedOptionColor : Game1.textColor);
        }

        public void DrawAlphaBar(SpriteBatch b, float hue, float saturation, float value, float maxAlpha = 1f, bool greyed = false)
        {
            DrawSliderBar(b, hue, saturation, value, maxAlpha, greyed ? Game1.unselectedOptionColor : Game1.textColor);
        }

        private void DrawSliderBar(SpriteBatch b, float hue, float saturation, float value, float alpha, Color textColor)
        {
            int sampleWidth = bounds.Width / Samples;
            int samplesInCheckeredTile = Math.Max(CheckeredTileWidth * Samples / bounds.Width, 1);
            Rectangle sampleBounds = new Rectangle(bounds.X, bounds.Y + (bounds.Height - _barHeight) / 2, sampleWidth, _barHeight);

            for (int i = 0; i < Samples; i++)
            {
                float sampleValue = i / (float)Samples;
                Color color;

                if (hue < 0)
                {
                    color = HSVToColor(sampleValue * 360f, saturation, value);
                }
                else if (saturation < 0)
                {
                    color = HSVToColor(hue, sampleValue, value);
                }
                else if (value < 0)
                {
                    color = HSVToColor(hue, saturation, sampleValue);
                }
                else
                {
                    color = HSVToColor(hue, saturation, value);
                    color *= sampleValue * alpha;

                    if (color.A < 255)
                    {
                        b.Draw(DeluxeJournalMod.UiTexture, sampleBounds, new(56 + i / samplesInCheckeredTile % 2 * 2, 72, 2, 4), Color.White);
                    }

                    goto DrawSample;
                }

                if (alpha < 1f)
                {
                    color *= alpha;
                }

            DrawSample:
                b.Draw(Game1.staminaRect, sampleBounds, color);
                sampleBounds.X += sampleWidth;
            }

            b.Draw(Game1.mouseCursors, new Vector2(bounds.X + bounds.Width * Value, bounds.Y), new(64, 256, 32, 20), Color.White, 0f, new(16f, 0f), 1f, SpriteEffects.None, 0.86f);
            Utility.drawTextWithShadow(b, ((int)(Value * 100f)).ToString(), Game1.smallFont, new(bounds.Right + 16, bounds.Bottom - Game1.smallFont.LineSpacing), textColor);
        }
    }
}
