using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley;

namespace DeluxeJournal.Framework.Serialization
{
    internal class WorldDateConverter : JsonConverter<WorldDate>
    {
        public override WorldDate ReadJson(JsonReader reader, Type objectType, WorldDate? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            string season = json["Season"]?.ToObject<string>() ?? "spring";
            int day = json["Day"]?.ToObject<int>() ?? 1;
            int year = json["Year"]?.ToObject<int>() ?? 1;

            return new WorldDate(year, season, day);
        }

        public override void WriteJson(JsonWriter writer, WorldDate? value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Season");
            writer.WriteValue(value?.Season.ToString() ?? "spring");
            writer.WritePropertyName("Day");
            writer.WriteValue(value?.DayOfMonth ?? 1);
            writer.WritePropertyName("Year");
            writer.WriteValue(value?.Year ?? 1);
            writer.WriteEndObject();
        }
    }
}
