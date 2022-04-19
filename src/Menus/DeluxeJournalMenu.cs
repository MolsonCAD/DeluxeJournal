using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using DeluxeJournal.Framework;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>The active menu the replaces the vanilla QuestLog. Displays journal pages and tabs.</summary>
    /// <remarks>Custom pages should be registered using the API.</remarks>
    public class DeluxeJournalMenu : IClickableMenu
    {
        private const int ActiveTabOffset = 8;

        private static readonly PerScreen<int> ActiveTabPerScreen = new PerScreen<int>();

        public static int ActiveTab
        {
            get
            {
                return ActiveTabPerScreen.Value;
            }

            private set
            {
                ActiveTabPerScreen.Value = value;
            }
        }

        private readonly List<ClickableTextureComponent> _tabs;
        private readonly List<IPage> _pages;
        private string _hoverText;

        public IReadOnlyList<ClickableComponent> Tabs => _tabs;

        public IReadOnlyList<IPage> Pages => _pages;

        public IPage ActivePage => _pages[ActiveTab];

        private string HoverText
        {
            get
            {
                string pageHoverText = ActivePage.HoverText;
                return pageHoverText.Length > 0 ? pageHoverText : _hoverText;
            }

            set
            {
                _hoverText = value;
            }
        }

        internal DeluxeJournalMenu(PageManager pageManager) : base(0, 0, 832, 576, showUpperRightCloseButton: true)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ||
                LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                height += 64;
            }

            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height, 0, 32);
            xPositionOnScreen = (int)topLeft.X;
            yPositionOnScreen = (int)topLeft.Y;
            upperRightCloseButton.bounds = new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen - 8, 48, 48);

            _pages = pageManager.GetPages(new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height));
            _tabs = new List<ClickableTextureComponent>();
            _hoverText = "";

            foreach (IPage page in _pages)
            {
                _tabs.Add(page.GetTabComponent());
            }

            ChainNeighborsUpDown(_tabs);

            if (ActiveTab == 0 && Game1.player.visibleQuestCount == 0)
            {
                ActiveTab = 1;
            }
            else if (Game1.player.hasPendingCompletedQuests)
            {
                ActiveTab = 0;
            }
            
            _tabs[ActiveTab].bounds.X += ActiveTabOffset;
            ActivePage.populateClickableComponentList();
            ActivePage.OnVisible();
            AddTabsToClickableComponents(ActivePage);

            if (Game1.options.SnappyMenus)
            {
                snapToDefaultClickableComponent();
            }

            Game1.playSound("bigSelect");

            exitFunction = () => ActivePage.OnHidden();
        }

        public void ChangeTab(int tab, bool playSound = true)
        {
            if (!readyToClose() || tab == ActiveTab || tab < 0 || tab >= _tabs.Count)
            {
                return;
            }

            if (playSound)
            {
                Game1.playSound("smallSelect");
            }

            ActivePage.OnHidden();
            _tabs[ActiveTab].bounds.X -= ActiveTabOffset;
            ActiveTab = tab;

            _tabs[ActiveTab].bounds.X += ActiveTabOffset;
            ActivePage.populateClickableComponentList();
            ActivePage.OnVisible();
            AddTabsToClickableComponents(ActivePage);

            if (Game1.options.SnappyMenus)
            {
                if (ActivePage is PageBase page)
                {
                    page.SnapToActiveTabComponent();
                }
                else
                {
                    ActivePage.snapToDefaultClickableComponent();
                }
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
        }

        public void AddTabsToClickableComponents(IPage page)
        {
            page.allClickableComponents.AddRange(_tabs);
        }

        public override ClickableComponent getCurrentlySnappedComponent()
        {
            return GetActiveMenu().getCurrentlySnappedComponent();
        }

        public override void setCurrentlySnappedComponentTo(int id)
        {
            GetActiveMenu().setCurrentlySnappedComponentTo(id);
        }

        public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
        {
            GetActiveMenu().automaticSnapBehavior(direction, oldRegion, oldID);
        }

        public override void snapToDefaultClickableComponent()
        {
            GetActiveMenu().snapToDefaultClickableComponent();
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            GetActiveMenu().snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            GetActiveMenu().receiveGamePadButton(b);
        }

        public override void setUpForGamePadMode()
        {
            GetActiveMenu().setUpForGamePadMode();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (ActivePage.GetChildMenu() == null)
            {
                base.receiveLeftClick(x, y, playSound);

                for (int i = 0; i < _tabs.Count; ++i)
                {
                    if (_tabs[i].containsPoint(x, y))
                    {
                        ChangeTab(i);
                        return;
                    }
                }
            }

            GetActiveMenu().receiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            GetActiveMenu().leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            GetActiveMenu().releaseLeftClick(x, y);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            GetActiveMenu().receiveRightClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            GetActiveMenu().receiveScrollWheelAction(direction);
        }

        public override void receiveKeyPress(Keys key)
        {
            GetActiveMenu().receiveKeyPress(key);

            if (ActivePage.GetChildMenu() == null && !ActivePage.KeyboardHasFocus())
            {
                if (Game1.options.doesInputListContain(Game1.options.journalButton, key) && readyToClose())
                {
                    exitThisMenu();
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            _hoverText = "";

            GetActiveMenu().performHoverAction(x, y);

            if (ActivePage.GetChildMenu() == null)
            {
                foreach (ClickableTextureComponent tab in _tabs)
                {
                    if (tab.containsPoint(x, y))
                    {
                        _hoverText = tab.hoverText;
                        return;
                    }
                }
            }
        }

        public override bool readyToClose()
        {
            return GetActiveMenu().readyToClose();
        }

        public override bool shouldDrawCloseButton()
        {
            return ActivePage.GetChildMenu() == null;
        }

        public override void update(GameTime time)
        {
            GetActiveMenu().update(time);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, _tabs[ActiveTab].hoverText, xPositionOnScreen + width / 2, yPositionOnScreen - 64);
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f);

            foreach (ClickableTextureComponent tab in _tabs)
            {
                tab.draw(b);
            }

            for (IClickableMenu menu = ActivePage; menu != null; menu = menu.GetChildMenu())
            {
                menu.draw(b);
            }

            base.draw(b);

            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);

            if (HoverText.Length > 0 && ActivePage.GetChildMenu() == null)
            {
                drawHoverText(b, HoverText, Game1.dialogueFont);
            }
        }

        private IClickableMenu GetActiveMenu()
        {
            IClickableMenu menu = ActivePage;

            while (menu.GetChildMenu() != null)
            {
                menu = menu.GetChildMenu();
            }

            return menu;
        }
    }
}
