using System.Text;
using StardewModdingAPI;

namespace DeluxeJournal.Framework.Commands.Tasks
{
    internal class PrintCommand : Command
    {
        public PrintCommand() : base("dj_notes_print", "Prints the saved notes text to the console.")
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                monitor.Log("A save file must be loaded before accessing the notes.", LogLevel.Error);
                return;
            }

            monitor.Log(new StringBuilder(DeluxeJournalMod.Instance!.GetNotes()).Replace("\n", "").Replace('\r', '\n').ToString(), LogLevel.Info);
        }
    }
}
