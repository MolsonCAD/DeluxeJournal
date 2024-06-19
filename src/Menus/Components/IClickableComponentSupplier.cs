using StardewValley.Menus;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>Represents a menu component with <see cref="ClickableComponent"/> instances that should be populated.</summary>
    public interface IClickableComponentSupplier
    {
        /// <summary>Get all <see cref="ClickableComponent"/> instances within this menu component.</summary>
        /// <returns>An <see cref="IEnumerable{T}"/> collection of child <see cref="ClickableComponent"/> instances.</returns>
        IEnumerable<ClickableComponent> GetClickableComponents();
    }
}
