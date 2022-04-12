using StardewModdingAPI;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;
using DeluxeJournal.Util;

using Constraint = DeluxeJournal.Tasks.TaskParameter.Constraint;

namespace DeluxeJournal.Framework.Tasks
{
    internal class BuyTask : TaskBase
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
                return Item != null ? new BuyTask(name, Item, Count) : null;
            }
        }

        /// <summary>Serialization constructor.</summary>
        public BuyTask() : base(TaskTypes.Buy)
        {
        }

        public BuyTask(string name, Item item, int count) : base(TaskTypes.Buy, name)
        {
            TargetDisplayName = item.DisplayName;
            TargetIndex = item.ParentSheetIndex;
            MaxCount = count;
            BasePrice = item.salePrice();
        }

        public override bool ShouldShowProgress()
        {
            return true;
        }

        public override int GetPrice()
        {
            return base.GetPrice() * (MaxCount - Count);
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.SalablePurchased += OnSalablePurchased;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.SalablePurchased -= OnSalablePurchased;
        }

        private void OnSalablePurchased(object? sender, SalablePurchasedEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && e.Salable is Item item && TargetIndex == item.ParentSheetIndex)
            {
                IncrementCount(e.Amount);
            }
        }
    }
}
