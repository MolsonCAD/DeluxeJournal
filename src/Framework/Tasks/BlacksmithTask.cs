using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;
using DeluxeJournal.Util;

namespace DeluxeJournal.Framework.Tasks
{
    internal class BlacksmithTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            private Item? _item = null;

            [TaskParameter("tool")]
            public Item? Item
            {
                get
                {
                    return _item;
                }

                set
                {
                    if (value is Tool tool && (tool is Axe || tool is Hoe || tool is WateringCan || tool is Pickaxe))
                    {
                        int localUpgradeLevel = ToolHelper.GetToolUpgradeLevelForPlayer(tool.BaseName, Game1.player);
                        tool.UpgradeLevel = (localUpgradeLevel < Tool.iridium) ? localUpgradeLevel + 1 : 0;
                        _item = tool;
                    }
                    else
                    {
                        _item = null;
                    }
                }
            }

            public override Item? SmartIconItem()
            {
                return Item;
            }

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                Item = new LocalizedObjects(translation).GetItem(task.TargetDisplayName);
            }

            public override ITask? Create(string name)
            {
                if (Item is Tool tool)
                {
                    return new BlacksmithTask(name, tool);
                }

                return null;
            }
        }

        /// <summary>Serialization constructor.</summary>
        public BlacksmithTask() : base(TaskTypes.Blacksmith)
        {
        }

        public BlacksmithTask(string name, Tool tool) : base(TaskTypes.Blacksmith, name)
        {
            TargetDisplayName = tool.DisplayName;
            TargetName = tool.BaseName;
            Variant = tool.UpgradeLevel;
            MaxCount = 2;

            Validate();
        }

        public override void Validate()
        {
            if (CanUpdate())
            {
                Tool upgraded = Game1.player.toolBeingUpgraded.Value;
                Count = (upgraded != null && upgraded.BaseName == TargetName) ? 1 : 0;
            }
        }

        public override bool ShouldShowCustomStatus()
        {
            return !Complete;
        }

        public override string GetCustomStatusKey()
        {
            if (Count == 0)
            {
                return "ui.tasks.status.deliver";
            }
            else if (Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                return "ui.tasks.status.upgrading";
            }
            else
            {
                return "ui.tasks.status.ready";
            }
        }

        public override int GetPrice()
        {
            return Count > 0 ? 0 : ToolHelper.PriceForToolUpgradeLevel(Variant);
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.SalablePurchased += OnSalablePurchased;
            events.ModEvents.Player.InventoryChanged += OnInventoryChanged;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.SalablePurchased -= OnSalablePurchased;
            events.ModEvents.Player.InventoryChanged -= OnInventoryChanged;
        }

        private void OnSalablePurchased(object? sender, SalablePurchasedEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && Count == 0)
            {
                if (e.Salable is Tool tool && tool.BaseName == TargetName)
                {
                    Count = 1;
                }
            }
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && e.Player.toolBeingUpgraded.Value == null && Count == 1)
            {
                foreach (Item item in e.Added)
                {
                    if (item is Tool tool && tool.BaseName == TargetName)
                    {
                        Count = MaxCount;
                        MarkAsCompleted();
                        break;
                    }
                }
            }
        }
    }
}
