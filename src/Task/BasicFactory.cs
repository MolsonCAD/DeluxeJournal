using StardewModdingAPI;

namespace DeluxeJournal.Task
{
    /// <summary>Generic factory for an <see cref="ITask"/> without state.</summary>
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
