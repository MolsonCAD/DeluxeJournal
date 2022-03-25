namespace DeluxeJournal.Util
{
    public class BlueprintInfo
    {
        public string Name { get; }
        public string DisplayName { get; }
        public string BuildingType { get; }
        public int Cost { get; }

        public BlueprintInfo(string name, string displayName, string buildingType, int cost)
        {
            Name = name;
            DisplayName = displayName;
            BuildingType = buildingType;
            Cost = cost;
        }

        public bool IsAnimal()
        {
            return BuildingType == "Animal";
        }
    }
}
