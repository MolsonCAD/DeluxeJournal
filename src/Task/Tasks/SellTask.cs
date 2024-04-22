using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Util;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class SellTask : TaskBase
    {
        public class Factory : TaskFactory
        {
            [TaskParameter(TaskParameterNames.Item, TaskParameterTag.ItemList, Constraints = Constraint.ItemId | Constraint.NotEmpty)]
            public IList<string>? ItemIds { get; set; }

            [TaskParameter(TaskParameterNames.Count, TaskParameterTag.Count, Constraints = Constraint.GE1)]
            public int Count { get; set; } = 1;

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Item;

            public override bool EnableSmartIconCount => true;

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                if (task is SellTask sellTask)
                {
                    ItemIds = sellTask.ItemIds;
                    Count = sellTask.MaxCount;
                }
            }

            public override ITask? Create(string name)
            {
                return ItemIds != null && ItemIds.Count > 0 ? new SellTask(name, ItemIds, Count) : null;
            }
        }

        /// <summary>The qualified item IDs of the items to be sold.</summary>
        public IList<string> ItemIds { get; set; }

        /// <summary>The qualified base item IDs of the items to be bought. Stripped of any encoded flavor ID information.</summary>
        [JsonIgnore]
        private List<string> BaseItemIds { get; set; } = new List<string>();

        /// <summary>The preserve item ID, if applicable.</summary>
        [JsonIgnore]
        private string? PreserveItemId { get; set; }

        /// <summary>Serialization constructor.</summary>
        public SellTask() : base(TaskTypes.Sell)
        {
            ItemIds = Array.Empty<string>();
        }

        public SellTask(string name, IList<string> itemIds, int count) : base(TaskTypes.Sell, name)
        {
            ItemIds = itemIds;
            MaxCount = count;

            if (ItemRegistry.Create(itemIds.First()) is Item item)
            {
                BasePrice = item is SObject obj ? obj.sellToStorePrice() : item.salePrice();
            }

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

        public override int GetPrice()
        {
            return -base.GetPrice();
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.SalableSold += OnSalableSold;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.SalableSold -= OnSalableSold;
        }

        private void OnSalableSold(object? sender, SalableEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && e.Salable is Item item && BaseItemIds.Contains(item.QualifiedItemId)
                && (string.IsNullOrEmpty(PreserveItemId) || item is SObject obj && PreserveItemId == obj.preservedParentSheetIndex.Value))
            {
                IncrementCount(e.Amount);
            }
        }
    }
}
