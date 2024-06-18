using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Framework;

namespace DeluxeJournal.Menus
{
    public abstract class IOverlay : IClickableMenu, IDisposable
    {
        /// <summary>Minimum resize width.</summary>
        public const int MinWidth = 100;

        /// <summary>Minumum resize height.</summary>
        public const int MinHeight = 100;

        /// <summary>Size of the resize boxes shown in edit-mode.</summary>
        public const int ResizeBoxSize = 16;

        /// <summary>Background texture box source.</summary>
        public static readonly Rectangle BackgroundSource = new(48, 58, 3, 3);

        /// <summary>Outline texture box source.</summary>
        public static readonly Rectangle OutlineSource = new(48, 61, 3, 3);

        /// <summary>Background texture box color.</summary>
        public static Color BackgroundColor { get; set; } = Color.Black * 0.25f;

        /// <summary>Background color opacity value between <c>0f</c> and <c>1f</c>.</summary>
        public static float BackgroundOpacity => BackgroundColor.A / 255f;

        /// <summary>Registered page ID value assigned by the <see cref="PageRegistry"/> (this value is set immediately AFTER construction).</summary>
        public string PageId { get; set; } = string.Empty;

        /// <summary>Overlay bounds snapped to the screen edge if within the <see cref="SnapDistance"/>.</summary>
        public Rectangle EdgeSnappedBounds { get; private set; }

        /// <summary><see cref="Rectangle"/> wrapper for the <see cref="IClickableMenu"/> bounds.</summary>
        public virtual Rectangle Bounds
        {
            get => new(xPositionOnScreen, yPositionOnScreen, width, height);

            set
            {
                Move(value.X, value.Y);
                Resize(value.Width, value.Height);
            }
        }

        /// <summary>On-screen coordinates of this overlay.</summary>
        public new Point Position
        {
            get => new(xPositionOnScreen, yPositionOnScreen);
            set => Move(value.X, value.Y);
        }

        /// <summary>Size of this overlay.</summary>
        public Point Size
        {
            get => new(width, height);
            set => Resize(value.X, value.Y);
        }

        /// <summary>Whether this overlay visible on-screen.</summary>
        public virtual bool IsVisible { get; set; }

        /// <summary>Whether the visibility of overlay can be toggled via hotkey.</summary>
        public bool IsVisibilityLocked { get; set; }

        /// <summary>Whether the overlay is currently in edit-mode.</summary>
        public bool IsEditing { get; set; }

        /// <summary>Whether the custom color is optional or is always used.</summary>
        public virtual bool IsColorOptional { get; set; }

        /// <summary>Whether the custom color should be used.</summary>
        public virtual bool IsColorSelected { get; set; }

        /// <summary>Custom color selected by the player.</summary>
        public Color CustomColor { get; set; }

        /// <summary>Distance from the screen edge to start snapping.</summary>
        public int SnapDistance { get; set; } = 4;

        /// <summary>Margin between the screen edge when snapped.</summary>
        public int SnapMargin { get; set; } = -4;

        public IOverlay(Rectangle bounds)
            : this(bounds.X, bounds.Y, bounds.Width, bounds.Height)
        {
        }

        public IOverlay(int x, int y, int width, int height)
            : base(x, y, width, height, false)
        {
        }

        /// <summary>Move the overlay in the specified on-screen coordinates.</summary>
        public virtual void Move(int x, int y)
        {
            xPositionOnScreen = x;
            yPositionOnScreen = y;
            CalculateEdgeSnappedBounds();
        }

        /// <summary>Resize the overlay to the specified dimensions.</summary>
        public virtual void Resize(int width, int height)
        {
            this.width = Math.Clamp(width, MinWidth, Game1.uiViewport.Width - ResizeBoxSize);
            this.height = Math.Clamp(height, MinHeight, Game1.uiViewport.Height - ResizeBoxSize);
            CalculateEdgeSnappedBounds();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            int x = (int)((newBounds.Width - this.width) * (xPositionOnScreen / (float)(oldBounds.Width - this.width)));
            int y = (int)((newBounds.Height - this.height) * (yPositionOnScreen / (float)(oldBounds.Height - this.height)));
            int width = this.width;
            int height = this.height;

            x -= Math.Max(0, x + this.width - newBounds.Width);
            y -= Math.Max(0, y + this.height - newBounds.Height);

            if (x < 0)
            {
                width += x - ResizeBoxSize;
                x = 0;
            }

            if (y < 0)
            {
                height += y - ResizeBoxSize;
                y = 0;
            }

            Move(x, y);

            if (width != this.width || height != this.height)
            {
                Resize(width, height);
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (IsVisible && Game1.activeClickableMenu is not DeluxeJournalMenu)
            {
                DrawContents(b);
            }
        }

        /// <summary>Draw with edit-mode graphics.</summary>
        public virtual void DrawInEditMode(SpriteBatch b)
        {
            DrawContents(b);
        }

        /// <summary>Draw the overlay contents.</summary>
        public abstract void DrawContents(SpriteBatch b);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <param name="disposing">Whether this method was invoked from the <see cref="IDisposable.Dispose"/> implementation or the finalizer.</param>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>Get the overlay bounds snapped to the screen edge.</summary>
        protected virtual void CalculateEdgeSnappedBounds()
        {
            Rectangle bounds = Bounds;

            if (bounds.X <= SnapDistance)
            {
                bounds.X = SnapMargin;
                bounds.Width -= SnapMargin;
            }
            else if (bounds.Right >= Game1.uiViewport.Width - SnapDistance)
            {
                bounds.X = Game1.uiViewport.Width - bounds.Width;
                bounds.Width -= SnapMargin;
            }

            if (bounds.Y <= SnapDistance)
            {
                bounds.Y = SnapMargin;
                bounds.Height -= SnapMargin;
            }
            else if (bounds.Bottom >= Game1.uiViewport.Height - SnapDistance)
            {
                bounds.Y = Game1.uiViewport.Height - bounds.Height;
                bounds.Height -= SnapMargin;
            }

            EdgeSnappedBounds = bounds;
        }
    }
}
