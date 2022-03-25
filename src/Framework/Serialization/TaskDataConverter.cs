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

        public override TaskData ReadJson(JsonReader reader, Type objectType, TaskData? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            IDictionary<string, List<ITask>> map = new Dictionary<string, List<ITask>>();

            if (JObject.Load(reader)["Tasks"] is not JObject json)
            {
                throw new JsonReaderException("Object with key \"Tasks\" not found");
            }

            foreach (var save in json)
            {
                if (save.Value is JArray taskArray)
                {
                    List<ITask> tasks = new List<ITask>();

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

                    map.Add(save.Key, tasks);
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
