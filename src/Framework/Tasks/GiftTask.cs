using StardewModdingAPI;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;
using DeluxeJournal.Util;

namespace DeluxeJournal.Framework.Tasks
{
    internal class GiftTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            [TaskParameter("npc")]
            public NPC? NPC { get; set; }

            [TaskParameter("item", Constraints = TaskParameter.Constraint.Giftable)]
            public Item? Item { get; set; }

            public override Item? SmartIconItem()
            {
                return Item;
            }

            public override NPC? SmartIconNPC()
            {
                return NPC;
            }

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                NPC = new LocalizedObjects(translation).GetNPC(task.TargetName);
                Item = (task.TargetIndex == -1) ? null : Utility.getItemFromStandardTextDescription("O " + task.TargetIndex + " 1", null);
            }

            public override ITask? Create(string name)
            {
                return NPC != null ? new GiftTask(name, NPC.Name, Item?.ParentSheetIndex ?? -1) : null;
            }
        }

        /// <summary>Serialization constructor.</summary>
        public GiftTask() : base(TaskTypes.Gift)
        {
        }

        public GiftTask(string name, string npcName, int itemIndex) : base(TaskTypes.Gift, name)
        {
            TargetName = npcName;
            TargetIndex = itemIndex;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.ItemGifted += OnItemGifted;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.ItemGifted -= OnItemGifted;
        }

        private void OnItemGifted(object? sender, GiftEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && TargetName == e.NPC.Name && (TargetIndex == -1 || TargetIndex == e.Item.ParentSheetIndex))
            {
                MarkAsCompleted();
            }
        }
    }
}
