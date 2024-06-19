using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Data
{
    internal class ColorData(IList<ColorSchema> colors)
    {
        public IList<ColorSchema> Colors { get; set; } = colors;
    }
}
