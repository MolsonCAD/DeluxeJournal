using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Task;

namespace DeluxeJournal.Menus.Components
{
    public class ProgressBar : ClickableComponent
    {
        public const int DefaultHeight = 48;

        public static readonly Color DefaultCompleteColor = new Color(38, 192, 32);
        public static readonly Color DefaultProgressColor = new Color(255, 145, 5);

        private readonly int _sections;
        private Color _progressColor;
        private Color _completeColor;

        public ProgressBar(Rectangle bounds, int sections)
            : this(bounds, sections, DefaultProgressColor, DefaultCompleteColor)
        {
        }

        public ProgressBar(Rectangle bounds, int sections, Color progressColor, Color completeColor)
            : base(bounds, string.Empty)
        {
            _sections = sections;
            _progressColor = progressColor;
            _completeColor = completeColor;
        }

        private static Color DarkenColor(Color color)
        {
            return new Color((int)(color.R * 0.8f) - 20, (int)(color.G * 0.8f) - 20, (int)(color.B * 0.8f) - 20, color.A);
        }

        public void Draw(SpriteBatch b, SpriteFont font, Color textColor, ColorSchema colorSchema, int currentCount, int maxCount)
        {
            float progress = (float)currentCount / maxCount;
            Color barColor = (progress >= 1f) ? _completeColor : _progressColor;

            Draw(b, font, textColor, barColor, colorSchema, currentCount, maxCount);
        }

        public void Draw(SpriteBatch b, SpriteFont font, Color textColor, Color barColor, ColorSchema colorSchema, int currentCount, int maxCount)
        {
            float progress = MathHelper.Clamp((float)currentCount / maxCount, 0f, 1f);
            int sections = (maxCount < _sections) ? maxCount : _sections;
            string text = currentCount + "/" + maxCount;
            Vector2 textPosition = new(bounds.X + ((int)(bounds.Width / 2 - font.MeasureString(currentCount + "/").X) / 4 * 4) + 12, bounds.Y + 8);
            Rectangle barBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);

            b.Draw(DeluxeJournalMod.UiTexture, new Rectangle(barBounds.X, barBounds.Y, 24, barBounds.Height), new(0, 64, 6, 14), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            b.Draw(DeluxeJournalMod.UiTexture, new Rectangle(barBounds.X + 24, barBounds.Y, barBounds.Width - 48, barBounds.Height), new(6, 64, 34, 14), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            b.Draw(DeluxeJournalMod.UiTexture, new Rectangle(barBounds.Right - 24, barBounds.Y, 24, barBounds.Height), new(40, 64, 6, 14), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);

            barBounds.X += 12;
            barBounds.Y += 12;
            barBounds.Width -= 24;
            barBounds.Height -= 24;

            b.Draw(Game1.staminaRect, barBounds, colorSchema.Main);
            b.Draw(DeluxeJournalMod.ColoredTaskMask, new Rectangle(barBounds.X, barBounds.Y, 8, barBounds.Height), new(0, 16, 2, 8), colorSchema.Shadow);
            b.Draw(DeluxeJournalMod.ColoredTaskMask, new Rectangle(barBounds.X + 8, barBounds.Y, barBounds.Width - 16, barBounds.Height), new(2, 16, 36, 8), colorSchema.Shadow);
            b.Draw(DeluxeJournalMod.ColoredTaskMask, new Rectangle(barBounds.Right - 8, barBounds.Y, 8, barBounds.Height), new(38, 16, 2, 8), colorSchema.Shadow);

            for (int i = 1; i < sections; i++)
            {
                b.Draw(DeluxeJournalMod.UiTexture, new Vector2(barBounds.X + barBounds.Width / sections * i, barBounds.Y), new(47, 67, 1, 8), colorSchema.Shadow * 0.8f, 0, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
            }

            barBounds.Width = (int)Math.Ceiling((barBounds.Width - 4) * progress / 4f) * 4;
            b.Draw(DeluxeJournalMod.UiTexture, barBounds, new(47, 67, 1, 8), barColor, 0, Vector2.Zero, SpriteEffects.None, 0.005f);

            if (progress > 0)
            {
                barBounds.X += barBounds.Width;
                barBounds.Width = 4;
                b.Draw(DeluxeJournalMod.UiTexture, barBounds, new(47, 67, 1, 8), DarkenColor(barColor), 0, Vector2.Zero, SpriteEffects.None, 0.005f);
            }

            b.DrawString(font, text, textPosition, textColor);
        }
    }
}
