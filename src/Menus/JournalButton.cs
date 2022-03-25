using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Framework;

namespace DeluxeJournal.Menus
{
    /// <summary>The replacement journal button for when there are no more active quests.</summary>
    public class JournalButton : IClickableMenu
    {
        public readonly ClickableTextureComponent taskButton;

        private readonly ITranslationHelper _translation;
        private readonly PageManager _pageManager;
        private string _hoverText;

        internal JournalButton(PageManager pageManager, ITranslationHelper translation) :
            base(Game1.uiViewport.Width - 88, 248, 44, 46)
        {
            _translation = translation;
            _pageManager = pageManager;
            _hoverText = "";

            taskButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height),
                DeluxeJournalMod.UiTexture,
                new Rectangle(0, 16, 11, 14),
                4f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.player.visibleQuestCount == 0 && taskButton.containsPoint(x, y) &&
                Game1.player.CanMove && !Game1.dialogueUp && !Game1.eventUp && Game1.farmEvent == null)
            {
                Game1.activeClickableMenu = new DeluxeJournalMenu(_pageManager);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            UpdatePosition();
        }

        public override void performHoverAction(int x, int y)
        {
            UpdatePosition();

            if (Game1.player.visibleQuestCount == 0 && taskButton.containsPoint(x, y))
            {
                _hoverText = _translation.Get("ui.taskbutton.hover").Tokens(new { key = Game1.options.journalButton[0].ToString() });
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (Game1.player.visibleQuestCount == 0)
            {
                taskButton.draw(b);
            }

            if (_hoverText.Length > 0 && isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                drawHoverText(b, _hoverText, Game1.dialogueFont);
            }
        }

        private void UpdatePosition()
        {
            xPositionOnScreen = Game1.uiViewport.Width - 88;

            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                int mapWidth = Game1.currentLocation.map.Layers[0].LayerWidth;
                xPositionOnScreen = Math.Min(xPositionOnScreen, mapWidth * 64 - Game1.uiViewport.X - 88);
            }

            taskButton.bounds = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);
        }
    }
}
