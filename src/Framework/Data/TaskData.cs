using Newtonsoft.Json;
using DeluxeJournal.Framework.Serialization;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Framework.Data
{
    [JsonConverter(typeof(TaskDataConverter))]
    internal class TaskData
    {
        public IDictionary<string, IDictionary<long, IList<ITask>>> Tasks { get; set; } = new Dictionary<string, IDictionary<long, IList<ITask>>>();
    }
}
