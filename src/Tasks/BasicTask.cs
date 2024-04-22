using DeluxeJournal.Framework.Tasks;

namespace DeluxeJournal.Tasks
{
    /// <summary>Basic task with no auto-completion features.</summary>
    public class BasicTask : TaskBase
    {
        public override int Count
        {
            get => 0;
            set => base.Count = value;
        }

        public override int MaxCount
        {
            get => 1;
            set => base.MaxCount = value;
        }

        public override int BasePrice
        {
            get => 0;
            set => base.BasePrice = value;
        }

        /// <summary>Serialization constructor.</summary>
        public BasicTask() : base(TaskTypes.Basic)
        {
        }

        public BasicTask(string name) : base(TaskTypes.Basic, name)
        {
        }
    }
}
