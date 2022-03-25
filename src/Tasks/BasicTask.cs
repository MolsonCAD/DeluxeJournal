using DeluxeJournal.Framework.Tasks;

namespace DeluxeJournal.Tasks
{
    /// <summary>Basic task with no auto-completion features.</summary>
    public class BasicTask : TaskBase
    {
        /// <summary>Serialization constructor.</summary>
        public BasicTask() : base(TaskTypes.Basic)
        {
        }

        public BasicTask(string name) : base(TaskTypes.Basic, name)
        {
        }
    }
}
