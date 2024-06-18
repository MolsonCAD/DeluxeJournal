using System.Collections;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Task
{
    /// <summary>A List wrapper for conveniently managing <see cref="ITask"/> instances.</summary>
    internal class EventManagedTaskList : IList<ITask>, IReadOnlyList<ITask>
    {
        private readonly ITaskEvents _events;
        private readonly List<ITask> _tasks;

        public ITask this[int index]
        {
            get => _tasks[index];

            set
            {
                ITask old = _tasks[index];

                if (old != value)
                {
                    old.EventUnsubscribe(_events);
                    value.EventSubscribe(_events);
                    _tasks[index] = value;

                    if (TaskListChangedEvent.HasEventListeners)
                    {
                        TaskListChangedEvent.Raise(this, new(this, new ITask[] { value }, new ITask[] { old }));
                    }
                }
            }
        }

        public int Count => _tasks.Count;

        public bool IsReadOnly => ((IList<ITask>)_tasks).IsReadOnly;

        public IManagedEvent<TaskListChangedArgs> TaskListChangedEvent { get; }

        public EventManagedTaskList(ITaskEvents events, IManagedEvent<TaskListChangedArgs> taskListChangedEvent)
        {
            _events = events;
            _tasks = new List<ITask>();
            TaskListChangedEvent = taskListChangedEvent;
        }

        public void Add(ITask task)
        {
            task.EventSubscribe(_events);
            _tasks.Add(task);

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, new ITask[] { task }, Enumerable.Empty<ITask>()));
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].EventUnsubscribe(_events);
            }

            IEnumerable<ITask> copy = _tasks.ToList();
            _tasks.Clear();

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, Enumerable.Empty<ITask>(), copy));
            }
        }

        public bool Contains(ITask task)
        {
            return _tasks.Contains(task);
        }

        public void CopyTo(ITask[] array, int arrayIndex)
        {
            _tasks.CopyTo(array, arrayIndex);
        }

        public int IndexOf(ITask task)
        {
            return _tasks.IndexOf(task);
        }

        public void Insert(int index, ITask task)
        {
            task.EventSubscribe(_events);
            _tasks.Insert(index, task);

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, new ITask[] { task }, Enumerable.Empty<ITask>()));
            }
        }

        public bool Remove(ITask task)
        {
            bool removed = _tasks.Remove(task);
            task.EventUnsubscribe(_events);

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, Enumerable.Empty<ITask>(), new ITask[] { task }));
            }

            return removed;
        }

        public void RemoveAt(int index)
        {
            ITask task = _tasks[index];
            task.EventUnsubscribe(_events);
            _tasks.RemoveAt(index);

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, Enumerable.Empty<ITask>(), new ITask[] { task }));
            }
        }

        /// <summary>
        /// Sort the <see cref="ITask"/> instances by their current state while preserving the ordering
        /// among tasks in the same state.
        /// </summary>
        public void Sort()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].SetSortingIndex(i);
            }

            _tasks.Sort();

            if (TaskListChangedEvent.HasEventListeners)
            {
                TaskListChangedEvent.Raise(this, new(this, Enumerable.Empty<ITask>(), Enumerable.Empty<ITask>()));
            }
        }

        public IEnumerator<ITask> GetEnumerator()
        {
            return _tasks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_tasks).GetEnumerator();
        }
    }
}
