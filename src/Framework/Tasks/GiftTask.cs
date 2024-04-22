using StardewModdingAPI;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;

using static DeluxeJournal.Tasks.TaskParameterAttribute;

namespace DeluxeJournal.Framework.Tasks
{
    internal class GiftTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            [TaskParameter(TaskParameterNames.Item, TaskParameterTag.ItemList, Constraints = Constraint.SObject)]
            public IList<string>? ItemIds { get; set; }

            [TaskParameter(TaskParameterNames.NPC, TaskParameterTag.NpcName)]
            public string? NpcName { get; set; }

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Item | SmartIconFlags.Npc;

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                if (task is GiftTask giftTask)
                {
                    NpcName = giftTask.NpcName;
                    ItemIds = giftTask.ItemIds;
                }
            }

            public override ITask? Create(string name)
            {
                return NpcName != null && (ItemIds == null || ItemIds.Count > 0)
                    ? new GiftTask(name, NpcName, ItemIds)
                    : null;
            }
        }

        /// <summary>The qualified item IDs of the item to be gifted.</summary>
        public IList<string>? ItemIds { get; set; }

        /// <summary>The internal name of the target NPC.</summary>
        public string NpcName { get; set; }

        /// <summary>Serialization constructor.</summary>
        public GiftTask() : base(TaskTypes.Gift)
        {
            NpcName = string.Empty;
        }

        public GiftTask(string name, string npcName, IList<string>? itemIds) : base(TaskTypes.Gift, name)
        {
            NpcName = npcName;
            ItemIds = itemIds;
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
            if (CanUpdate() && IsTaskOwner(e.Player) && NpcName == e.NPC.Name && (ItemIds == null || ItemIds.Contains(e.Item.QualifiedItemId)))
            {
                MarkAsCompleted();
            }
        }
    }
}
