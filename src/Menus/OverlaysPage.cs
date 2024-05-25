using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Menus
{
    public class OverlaysPage : PageBase
    {
        public OverlaysPage(string name, Rectangle bounds, Texture2D tabTexture, ITranslationHelper translation)
            : this(name, translation.Get("ui.tab.overlays"), bounds.X, bounds.Y, bounds.Width, bounds.Height, tabTexture, new Rectangle(48, 0, 16, 16))
        {
        }

        public OverlaysPage(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect)
            : base(name, title, x, y, width, height, tabTexture, tabSourceRect)
        {
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (!isWithinBounds(x, y))
            {
                Game1.activeClickableMenu?.exitThisMenu(playSound);
            }
        }
    }
}
