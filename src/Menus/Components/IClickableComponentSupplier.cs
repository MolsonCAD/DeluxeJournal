using StardewValley.Menus;

namespace DeluxeJournal.Menus.Components
{
    public interface IClickableComponentSupplier
    {
        IEnumerable<ClickableComponent> GetClickableComponents();
    }
}
