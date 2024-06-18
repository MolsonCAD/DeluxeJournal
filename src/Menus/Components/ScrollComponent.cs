using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>Encapsulates scrolling logic.</summary>
    public class ScrollComponent : IClickableComponentSupplier
    {
        public event Action<ScrollComponent>? OnScroll;

        private readonly ClickableTextureComponent _upArrowButton;
        private readonly ClickableTextureComponent _downArrowButton;
        private readonly ClickableTextureComponent _scrollBar;

        private Rectangle _scrollBarBounds;
        private Rectangle _contentBounds;
        private Rectangle? _cachedScissorRect;
        private int _scrollAmount;
        private bool _scrolling;

        public Rectangle ScrollBarBounds => _scrollBarBounds;
        
        public Rectangle ContentBounds => _contentBounds;

        public bool ClipToScrollDistance { get; set; }

        public int ScrollDistance { get; set; }

        public int ContentHeight { get; set; }

        public int ScrollAmount
        {
            get
            {
                return _scrollAmount;
            }

            set
            {
                _scrollAmount = value;
                SetScrollFromAmount();
            }
        }

        public ScrollComponent(Rectangle scrollBarBounds, Rectangle contentBounds, int scrollDistance, bool clipToScrollDistance = false)
        {
            _scrollBarBounds = scrollBarBounds;
            _contentBounds = contentBounds;
            _cachedScissorRect = null;
            _scrollAmount = 0;
            _scrolling = false;

            ScrollDistance = scrollDistance;
            ClipToScrollDistance = clipToScrollDistance;

            _scrollBar = new ClickableTextureComponent(
                new Rectangle(_scrollBarBounds.X, _scrollBarBounds.Y, _scrollBarBounds.Width, 40),
                Game1.mouseCursors,
                new Rectangle(435, 463, 6, 10),
                4f)
            {
                myID = 12220,
                upNeighborID = 12221,
                downNeighborID = 12222,
                upNeighborImmutable = true,
                downNeighborImmutable = true
            };
            
            _upArrowButton = new ClickableTextureComponent(
                new Rectangle(_scrollBarBounds.X - 12, _scrollBarBounds.Y - 48, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 459, 11, 12),
                4f)
            {
                myID = 12221,
                downNeighborID = 12220,
                downNeighborImmutable = true
            };
            
            _downArrowButton = new ClickableTextureComponent(
                new Rectangle(_scrollBarBounds.X - 12, _scrollBarBounds.Y + _scrollBarBounds.Height + 4, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 472, 11, 12),
                4f)
            {
                myID = 12222,
                upNeighborID = 12220,
                upNeighborImmutable = true
            };
        }

        public IEnumerable<ClickableComponent> GetClickableComponents()
        {
            yield return _upArrowButton;
            yield return _downArrowButton;
            yield return _scrollBar;
        }

        public int GetScrollOffset()
        {
            return ScrollAmount / ScrollDistance;
        }

        public int GetOverflowAmount()
        {
            return Math.Max(ContentHeight - _contentBounds.Height, 0);
        }

        public float GetPercentScrolled()
        {
            float overflow = GetOverflowAmount();
            return (overflow > 0) ? _scrollAmount / overflow : 1f;
        }

        public bool CanScroll()
        {
            return ContentHeight > _contentBounds.Height;
        }

        public void Refresh()
        {
            SetScrollFromAmount();
        }

        public void Scroll(int direction, bool playSound = true)
        {
            if (CanScroll())
            {
                if (playSound)
                {
                    Game1.playSound("shiny4");
                }

                ScrollAmount -= Math.Sign(direction) * ScrollDistance;
            }
        }

        public void SetScrollFromY(int y, bool playSound = true)
        {
            int oldScrollBarY = _scrollBar.bounds.Y;
            float percentage = (y - _scrollBarBounds.Y) / (float)(_scrollBarBounds.Height - _scrollBar.bounds.Height);
            float scrollAmount = Utility.Clamp(percentage, 0, 1f) * (ContentHeight - _contentBounds.Height);

            if (ClipToScrollDistance)
            {
                ScrollAmount = (int)(scrollAmount / ScrollDistance) * ScrollDistance;
            }
            else
            {
                ScrollAmount = (int)scrollAmount;
            }

            if (playSound && oldScrollBarY != _scrollBar.bounds.Y)
            {
                Game1.playSound("shiny4");
            }
        }

        private void SetScrollFromAmount()
        {
            if (!CanScroll())
            {
                _scrollAmount = 0;
                return;
            }

            int overflow = GetOverflowAmount();

            if (_scrollAmount < 8)
            {
                _scrollAmount = 0;
            }
            else if (_scrollAmount > overflow - 8)
            {
                _scrollAmount = overflow;
            }

            int offset = (int)((_scrollBarBounds.Height - _scrollBar.bounds.Height) * GetPercentScrolled());
            _scrollBar.bounds.Y = _scrollBarBounds.Y + offset;
            OnScroll?.Invoke(this);
        }

        public virtual void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (CanScroll())
            {
                if (_downArrowButton.containsPoint(x, y) && _scrollAmount < GetOverflowAmount())
                {
                    _downArrowButton.scale = _downArrowButton.baseScale;
                    Scroll(-1, false);
                }
                else if (_upArrowButton.containsPoint(x, y) && _scrollAmount > 0)
                {
                    _upArrowButton.scale = _upArrowButton.baseScale;
                    Scroll(1, false);
                }
                else
                {
                    if (_scrollBar.containsPoint(x, y) || _scrollBarBounds.Contains(x, y))
                    {
                        _scrolling = true;
                    }

                    return;
                }

                if (playSound)
                {
                    Game1.playSound("shiny4");
                }
            }
        }

        public virtual void LeftClickHeld(int x, int y, bool playSound = true)
        {
            if (_scrolling)
            {
                SetScrollFromY(y, playSound);
            }
        }

        public virtual void ReleaseLeftClick(int x, int y)
        {
            _scrolling = false;
        }

        public virtual void TryHover(int x, int y)
        {
            if (CanScroll())
            {
                _upArrowButton.tryHover(x, y);
                _downArrowButton.tryHover(x, y);
                _scrollBar.tryHover(x, y);
            }
        }

        public virtual void BeginScissorTest(SpriteBatch b)
        {
            _cachedScissorRect = b.GraphicsDevice.ScissorRectangle;

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState()
            {
                ScissorTestEnable = true
            });

            b.GraphicsDevice.ScissorRectangle = _contentBounds;
        }

        public virtual void EndScissorTest(SpriteBatch b)
        {
            b.End();

            if (_cachedScissorRect != null)
            {
                b.GraphicsDevice.ScissorRectangle = (Rectangle)_cachedScissorRect;
            }

            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        }

        public virtual void DrawScrollBar(SpriteBatch b)
        {
            if (CanScroll())
            {
                _upArrowButton.draw(b);
                _downArrowButton.draw(b);
                _scrollBar.draw(b);
            }
        }
    }
}
