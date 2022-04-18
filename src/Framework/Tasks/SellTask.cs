using StardewModdingAPI;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;
using DeluxeJournal.Util;

using Constraint = DeluxeJournal.Tasks.TaskParameter.Constraint;

namespace DeluxeJournal.Framework.Tasks
{
    internal class SellTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            [TaskParameter("item")]
            public Item? Item { get; set; }

            [TaskParameter("count", Tag = "count", Constraints = Constraint.GT0)]
            public int Count { get; set; } = 1;

            public override Item? SmartIconItem()
            {
                return Item;
            }

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                Item = new LocalizedObjects(translation).GetItem(task.TargetDisplayName);
                Count = task.MaxCount;
            }

            public override ITask? Create(string name)
            {
                return Item != null ? new SellTask(name, Item, Count) : null;
            }
        }

        /// <summary>Serialization constructor.</summary>
        public SellTask() : base(TaskTypes.Sell)
        {
        }

        public SellTask(string name, Item item, int count) : base(TaskTypes.Sell, name)
        {
            TargetDisplayName = item.DisplayName;
            TargetIndex = item.ParentSheetIndex;
            MaxCount = count;

            if (item is SObject obj)
            {
                BasePrice = obj.sellToStorePrice();
            }
            else
            {
                BasePrice = item.salePrice();
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

        private void OnSalableSold(object? sender, SalableSoldEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && e.Salable is Item item && TargetIndex == item.ParentSheetIndex)
            {
                IncrementCount(item.Stack);
            }
        }
    }
}
