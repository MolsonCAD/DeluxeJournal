namespace DeluxeJournal.Task
{
    /// <summary>Generic factory for an <see cref="ITask"/> without state.</summary>
    public class BasicFactory<T> : TaskFactory where T : ITask, new()
    {
        protected override void InitializeInternal(ITask task)
        {
        }

        protected override ITask? CreateInternal(string name)
        {
            return new T() { Name = name };
        }
    }
}
