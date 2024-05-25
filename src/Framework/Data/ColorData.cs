using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Data
{
    internal class ColorData
    {
        public IList<ColorSchema> Colors { get; set; }

        public ColorData(IList<ColorSchema> colors)
        {
            Colors = colors;
        }
    }
}
