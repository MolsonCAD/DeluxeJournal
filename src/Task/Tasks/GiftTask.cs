using StardewModdingAPI;
using DeluxeJournal.Events;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class GiftTask : ItemTaskBase
    {
        public class Factory : TaskFactory
        {
            [TaskParameter(TaskParameterNames.Item, TaskParameterTag.ItemList, Constraints = OptionalObjectIdsConstraint)]
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
                    ? new GiftTask(name, NpcName, ItemIds ?? new List<string>())
                    : null;
            }
        }

        /// <summary>The internal name of the target NPC.</summary>
        public string NpcName { get; set; }

        /// <summary>Serialization constructor.</summary>
        public GiftTask() : base(TaskTypes.Gift)
        {
            NpcName = string.Empty;
        }

        public GiftTask(string name, string npcName, IList<string> itemIds) : base(TaskTypes.Gift, name, itemIds, 1)
        {
            NpcName = npcName;
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
            if (CanUpdate() && IsTaskOwner(e.Player) && NpcName == e.NPC.Name && (BaseItemIds.Count == 0 || CheckItemMatch(e.Item)))
            {
                MarkAsCompleted();
            }
        }
    }
}
