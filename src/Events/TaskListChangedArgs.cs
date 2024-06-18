using DeluxeJournal.Framework.Task;
using DeluxeJournal.Task;

namespace DeluxeJournal.Events
{
    internal class TaskListChangedArgs(EventManagedTaskList tasks, IEnumerable<ITask> added, IEnumerable<ITask> removed) : EventArgs
    {
        /// <summary>The task list that was changed.</summary>
        public EventManagedTaskList Tasks { get; } = tasks;

        /// <summary>A list of <see cref="ITask"/>s added to the task list.</summary>
        public IEnumerable<ITask> Added { get; } = added;

        /// <summary>A list of <see cref="ITask"/>s removed from the task list.</summary>
        public IEnumerable<ITask> Removed { get; } = removed;
    }
}
