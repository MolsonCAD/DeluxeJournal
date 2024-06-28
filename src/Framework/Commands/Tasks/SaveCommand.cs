using StardewModdingAPI;

namespace DeluxeJournal.Framework.Commands.Tasks
{
    internal class SaveCommand : Command
    {
        public SaveCommand() : base("dj_tasks_save", "Save the current state of the tasks list.")
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            DeluxeJournalMod.TaskManager?.Save();
        }
    }
}
