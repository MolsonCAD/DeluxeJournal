using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Framework.Tasks;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Framework.Serialization
{
    internal class TaskDataConverter : JsonConverter<TaskData>
    {
        public override bool CanWrite => false;

        private static IList<ITask> DeserializeTasks(JArray taskArray)
        {
            IList<ITask> tasks = new List<ITask>();

            foreach (JObject taskObject in taskArray)
            {
                string id = taskObject.Value<string>("ID");

                if (taskObject.ToObject(TaskRegistry.GetTaskTypeOrDefault(id)) is ITask task)
                {
                    tasks.Add(task);
                }
                else
                {
                    throw new JsonReaderException("Unable to deserialize ITask");
                }
            }

            return tasks;
        }

        public override TaskData ReadJson(JsonReader reader, Type objectType, TaskData? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            IDictionary<string, IDictionary<long, IList<ITask>>> map = new Dictionary<string, IDictionary<long, IList<ITask>>>();
            IDictionary<long, IList<ITask>> playerTasks;

            if (JObject.Load(reader)["Tasks"] is not JObject json)
            {
                throw new JsonReaderException("Object with key \"Tasks\" not found");
            }

            foreach (var save in json)
            {
                // Legacy versions (<= 1.0.3) do not have a UMID mapping for each (local) player's task list
                if (save.Value is JArray)
                {
                    map.Add(save.Key, new Dictionary<long, IList<ITask>>()
                    {
                        { 0, DeserializeTasks((JArray)save.Value) }
                    });
                }
                else if (save.Value is JObject playerTasksObject)
                {
                    playerTasks = new Dictionary<long, IList<ITask>>();

                    foreach (var taskArray in playerTasksObject)
                    {
                        if (taskArray.Value is JArray && long.TryParse(taskArray.Key, out long umid))
                        {
                            playerTasks.Add(umid, DeserializeTasks((JArray)taskArray.Value));
                        }
                    }

                    map.Add(save.Key, playerTasks);
                }
            }

            return new TaskData() { Tasks = map };
        }

        public override void WriteJson(JsonWriter writer, TaskData? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
