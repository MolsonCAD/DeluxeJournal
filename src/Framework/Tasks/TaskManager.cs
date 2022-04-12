using StardewValley;
using StardewModdingAPI;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Framework.Tasks
{
    internal class TaskManager
    {
        private readonly ITaskEvents _events;
        private readonly IDataHelper _dataHelper;
        private readonly TaskData _data;
        private readonly IDictionary<long, TaskList> _tasks;

        public IList<ITask> Tasks
        {
            get
            {
                if (!_tasks.ContainsKey(Game1.player.UniqueMultiplayerID))
                {
                    _tasks[Game1.player.UniqueMultiplayerID] = new TaskList(_events);
                }

                return _tasks[Game1.player.UniqueMultiplayerID];
            }
        }

        public TaskManager(ITaskEvents events, IDataHelper dataHelper)
        {
            _events = events;
            _dataHelper = dataHelper;
            _data = _dataHelper.ReadGlobalData<TaskData>(DeluxeJournalMod.TASKS_DATA_KEY) ?? new TaskData();
            _tasks = new Dictionary<long, TaskList>();
        }

        public void OnDayEnding()
        {
            ITask task;

            for (int i = Tasks.Count - 1; i >= 0; i--)
            {
                task = Tasks[i];

                if (task.RenewPeriod != ITask.Period.Never)
                {
                    if (task.Complete)
                    {
                        task.Complete = task.Active = false;
                    }

                    if (!task.Active && task.DaysRemaining() <= 1)
                    {
                        task.Active = true;
                    }
                }

                if (task.Complete)
                {
                    Tasks.RemoveAt(i);
                }
            }
        }

        /// <summary>Sort local player tasks.</summary>
        public void SortTasks()
        {
            ((TaskList)Tasks).Sort();
        }

        public void Load()
        {
            string saveFolderName = Constants.SaveFolderName;
            long umid;

            // Each TaskList must be cleared in order to unsubscribe from task events
            foreach (TaskList tasks in _tasks.Values)
            {
                tasks.Clear();
            }

            _tasks.Clear();

            if (_data.Tasks.ContainsKey(saveFolderName))
            {
                foreach (long key in _data.Tasks[saveFolderName].Keys)
                {
                    // UMID is set to 0 when data is converted from legacy versions (<= 1.0.3)
                    umid = (key == 0) ? Game1.player.UniqueMultiplayerID : key;

                    if (!_tasks.ContainsKey(umid))
                    {
                        _tasks[umid] = new TaskList(_events);
                    }

                    foreach (ITask task in _data.Tasks[saveFolderName][key])
                    {
                        task.OwnerUMID = umid;
                        _tasks[umid].Add(task);
                    }

                    _tasks[umid].Sort();
                }
            }
        }

        public void Save()
        {
            _data.Tasks[Constants.SaveFolderName] = _tasks
                .Where(entry => entry.Value.Count > 0)
                .ToDictionary(entry => entry.Key, entry => (IList<ITask>)entry.Value.ToList());

            _dataHelper.WriteGlobalData(DeluxeJournalMod.TASKS_DATA_KEY, _data);
        }
    }
}
