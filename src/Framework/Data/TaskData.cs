using Newtonsoft.Json;
using DeluxeJournal.Framework.Serialization;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Framework.Data
{
    [JsonConverter(typeof(TaskDataConverter))]
    internal class TaskData
    {
        public IDictionary<string, List<ITask>> Tasks { get; set; } = new Dictionary<string, List<ITask>>();
    }
}
