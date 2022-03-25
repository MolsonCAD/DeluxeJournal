using StardewModdingAPI;

namespace DeluxeJournal.Tasks
{
    /// <summary>Generic factory for ITasks without state.</summary>
    public class BasicFactory<T> : TaskFactory where T : ITask, new()
    {
        public override void Initialize(ITask task, ITranslationHelper translation)
        {
        }

        public override ITask? Create(string name)
        {
            return new T() { Name = name };
        }
    }
}
