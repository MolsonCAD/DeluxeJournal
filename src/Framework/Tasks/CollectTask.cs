using Newtonsoft.Json;
using StardewModdingAPI;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;
using DeluxeJournal.Util;

using static DeluxeJournal.Tasks.TaskParameterAttribute;

namespace DeluxeJournal.Framework.Tasks
{
    internal class CollectTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            [TaskParameter(TaskParameterNames.Item, TaskParameterTag.ItemList, Constraints = Constraint.SObject | Constraint.NotEmpty)]
            public IList<string>? ItemIds { get; set; }

            [TaskParameter(TaskParameterNames.Count, TaskParameterTag.Count, Constraints = Constraint.GE1)]
            public int Count { get; set; } = 1;

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Item;

            public override bool EnableSmartIconCount => true;

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                if (task is CollectTask collectTask)
                {
                    ItemIds = collectTask.ItemIds;
                    Count = collectTask.MaxCount;
                }
            }

            public override ITask? Create(string name)
            {
                return ItemIds != null && ItemIds.Count > 0 ? new CollectTask(name, ItemIds, Count) : null;
            }
        }

        /// <summary>The qualified item IDs of the items to be collected.</summary>
        public IList<string> ItemIds { get; set; }

        /// <summary>The qualified base item IDs of the items to be collected. Stripped of any encoded flavor ID information.</summary>
        [JsonIgnore]
        private List<string> BaseItemIds { get; set; } = new List<string>();

        /// <summary>The preserve item ID parent, if applicable.</summary>
        [JsonIgnore]
        private string? PreserveItemId { get; set; }

        /// <summary>Serialization constructor.</summary>
        public CollectTask() : base(TaskTypes.Collect)
        {
            ItemIds = Array.Empty<string>();
        }

        public CollectTask(string name, IList<string> itemIds, int count) : base(TaskTypes.Collect, name)
        {
            ItemIds = itemIds;
            MaxCount = count;
            Validate();
        }

        public override void Validate()
        {
            PreserveItemId = FlavoredItemHelper.ConvertFlavoredList(ItemIds, out var baseItemIds, false);
            BaseItemIds.Clear();
            BaseItemIds.AddRange(baseItemIds);
        }

        public override bool ShouldShowProgress()
        {
            return true;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.ItemCollected += OnItemCollected;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.ItemCollected -= OnItemCollected;
        }

        private void OnItemCollected(object? sender, ItemReceivedEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && BaseItemIds.Contains(e.Item.QualifiedItemId)
                && (string.IsNullOrEmpty(PreserveItemId) || PreserveItemId == e.Item.preservedParentSheetIndex.Value))
            {
                IncrementCount(e.Count);
            }
        }
    }
}
