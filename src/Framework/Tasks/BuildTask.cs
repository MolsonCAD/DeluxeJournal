using StardewModdingAPI;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;

using Constraint = DeluxeJournal.Tasks.TaskParameter.Constraint;

namespace DeluxeJournal.Framework.Tasks
{
    internal class BuildTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            [TaskParameter("building", Tag = "building")]
            public string BuildingType { get; set; } = string.Empty;

            [TaskParameter("count", Tag = "count", Constraints = Constraint.GT0)]
            public int Count { get; set; } = 1;

            [TaskParameter("cost", Tag = "cost", Hidden = true)]
            public int Cost { get; set; } = 0;

            public override string? SmartIconName()
            {
                return BuildingType;
            }

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                BuildingType = task.TargetDisplayName;
                Count = task.MaxCount;
                Cost = task.BasePrice;
            }

            public override ITask? Create(string name)
            {
                return new BuildTask(name, BuildingType, Count, Cost);
            }
        }

        /// <summary>Serialization constructor.</summary>
        public BuildTask() : base(TaskTypes.Build)
        {
        }

        public BuildTask(string name, string buildingType, int count, int cost) : base(TaskTypes.Build, name)
        {
            TargetDisplayName = buildingType;
            MaxCount = count;
            BasePrice = cost;
        }

        public override bool ShouldShowProgress()
        {
            return MaxCount > 1;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.BuildingConstructed += OnBuildingConstructed;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.BuildingConstructed -= OnBuildingConstructed;
        }

        private void OnBuildingConstructed(object? sender, BuildingConstructedEventArgs e)
        {
            if (CanUpdate() && (e.NameAfterConstruction == TargetDisplayName || (TargetDisplayName == "Cabin" && e.Building.isCabin)))
            {
                IncrementCount();
            }
        }
    }
}
