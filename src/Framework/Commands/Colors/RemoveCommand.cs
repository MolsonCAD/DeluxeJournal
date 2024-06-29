using DeluxeJournal.Task;
using StardewModdingAPI;

namespace DeluxeJournal.Framework.Commands.Colors
{
    internal class RemoveCommand : ColorCommand
    {
        public RemoveCommand() : base("dj_colors_remove",
            BuildDocString("Remove a color schema by index.",
                "dj_colors_remove <index>",
                "index: Index of the color schema to remove."))
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            AssertMinArgs(args, 1);

            int index = ColorIndexFromArg(args[0]);
            ColorSchema removed = DeluxeJournalMod.ColorSchemas[index];
            DeluxeJournalMod.ColorSchemas.RemoveAt(index);
            monitor.Log($"Removed color schema ({index}): {removed}", LogLevel.Info);
        }
    }
}
