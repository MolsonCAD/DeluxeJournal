using StardewModdingAPI;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Util;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class SellTask : ItemTaskBase
    {
        public class Factory : TaskFactory
        {
            [TaskParameter(TaskParameterNames.Item, TaskParameterTag.ItemList, Constraints = ItemIdsConstraint)]
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

        /// <summary>Serialization constructor.</summary>
        public SellTask() : base(TaskTypes.Sell)
        {
        }

        public SellTask(string name, IList<string> itemIds, int count) : base(TaskTypes.Sell, name, itemIds, count)
        {
            BasePrice = SellPrice(ItemRegistry.Create(itemIds.First()));
            Validate();
        }

        public override void Validate()
        {
            base.Validate();

            if (BaseItemIds.Count > 0 && IngredientItemId != null
                && FlavoredItemHelper.CreateFlavoredItem(BaseItemIds.First(), IngredientItemId) is Item flavoredItem)
            {
                BasePrice = flavoredItem.sellToStorePrice();
            }
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
            if (CanUpdate() && IsTaskOwner(e.Player) && e.Salable is Item item && CheckItemMatch(item))
            {
                IncrementCount(e.Amount);
            }
        }
    }
}
