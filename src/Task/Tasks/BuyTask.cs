using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Util;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class BuyTask : TaskBase
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
                if (task is BuyTask buyTask)
                {
                    ItemIds = buyTask.ItemIds;
                    Count = buyTask.MaxCount;
                }
            }

            public override ITask? Create(string name)
            {
                return ItemIds != null && ItemIds.Count > 0 ? new BuyTask(name, ItemIds, Count) : null;
            }
        }

        /// <summary>The qualified item IDs of the items to be bought.</summary>
        public IList<string> ItemIds { get; set; }

        /// <summary>The qualified base item IDs of the items to be bought. Stripped of any encoded flavor ID information.</summary>
        [JsonIgnore]
        private List<string> BaseItemIds { get; set; } = new List<string>();

        /// <summary>The preserve item ID, if applicable.</summary>
        [JsonIgnore]
        private string? IngredientItemId { get; set; }

        /// <summary>Serialization constructor.</summary>
        public BuyTask() : base(TaskTypes.Buy)
        {
            ItemIds = Array.Empty<string>();
        }

        public BuyTask(string name, IList<string> itemIds, int count) : base(TaskTypes.Buy, name)
        {
            ItemIds = itemIds;
            MaxCount = count;
            BasePrice = BuyPrice(ItemRegistry.Create(itemIds.First()));
            Validate();
        }

        public override void Validate()
        {
            IngredientItemId = FlavoredItemHelper.ConvertFlavoredList(ItemIds, out var baseItemIds, false);
            BaseItemIds.Clear();
            BaseItemIds.AddRange(baseItemIds);

            if (BaseItemIds.Count > 0 && IngredientItemId != null
                && FlavoredItemHelper.CreateFlavoredItem(BaseItemIds.First(), IngredientItemId) is Item flavoredItem)
            {
                BasePrice = flavoredItem.salePrice();
            }
        }

        public override bool ShouldShowProgress()
        {
            return true;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.SalablePurchased += OnSalablePurchased;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.SalablePurchased -= OnSalablePurchased;
        }

        private void OnSalablePurchased(object? sender, SalableEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && e.Salable is Item item && BaseItemIds.Contains(item.QualifiedItemId)
                && (string.IsNullOrEmpty(IngredientItemId) || (item is SObject obj && IngredientItemId == obj.preservedParentSheetIndex.Value)))
            {
                IncrementCount(e.Amount);
            }
        }
    }
}
