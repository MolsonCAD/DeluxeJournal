using DeluxeJournal.Task;

namespace DeluxeJournal.Events
{
    public class TaskStatusChangedArgs(ITask task, bool oldActive, bool oldComplete, int oldCount, bool newActive, bool newComplete, int newCount) : EventArgs
    {
        /// <summary>The <see cref="ITask"/> whose status has changed.</summary>
        public ITask Task { get; } = task;

        /// <summary>The old <see cref="ITask.Active"/> state.</summary>
        public bool OldActive { get; } = oldActive;

        /// <summary>The old <see cref="ITask.Complete"/> state.</summary>
        public bool OldComplete { get; } = oldComplete;

        /// <summary>The old <see cref="ITask.Count"/> value.</summary>
        public int OldCount { get; } = oldCount;

        /// <summary>The new <see cref="ITask.Active"/> state.</summary>
        public bool NewActive { get; } = newActive;

        /// <summary>The new <see cref="ITask.Complete"/> state.</summary>
        public bool NewComplete { get; } = newComplete;

        /// <summary>The new <see cref="ITask.Count"/> value.</summary>
        public int NewCount { get; } = newCount;
    }
}
