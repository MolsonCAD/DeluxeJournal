namespace DeluxeJournal.Task.Tasks
{
    /// <summary>Non-completable dummy task that serves as a header in the task list.</summary>
    internal class HeaderTask : TaskBase
    {
        public override bool Active
        {
            get => true;
            set { }
        }

        public override bool Complete
        {
            get => false;
            set { }
        }

        /// <summary>Serialization constructor.</summary>
        public HeaderTask() : base(TaskTypes.Header)
        {
        }

        public HeaderTask(string name) : base(TaskTypes.Header, name)
        {
        }
    }
}
