using Newtonsoft.Json;
using StardewModdingAPI;
using DeluxeJournal.Framework.Serialization;
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework.Data
{
    [JsonConverter(typeof(TaskDataConverter))]
    internal class TaskData
    {
        internal const string DefaultVersion = "1.0.0";

        /// <summary>Mod version for selecting the correct deserialization strategy.</summary>
        public string Version { get; set; } = DefaultVersion;

        /// <summary>Task data.</summary>
        public IDictionary<string, IDictionary<long, IList<ITask>>> Tasks { get; set; } = new Dictionary<string, IDictionary<long, IList<ITask>>>();

        /// <summary>Default constructor.</summary>
        public TaskData()
        {
        }

        /// <summary>Initialization constructor.</summary>
        /// <param name="version">Version obtained from the mod's manifest.</param>
        public TaskData(ISemanticVersion version) : this(version.ToString())
        {
        }

        /// <summary>Initialization constructor.</summary>
        /// <param name="version">Version obtained from the mod's manifest.</param>
        public TaskData(string version)
        {
            Version = version;
        }
    }
}
