namespace DeluxeJournal.Framework.Data
{
    internal class NotesData
    {
        /// <summary>Notes text per save file.</summary>
        public IDictionary<string, string> Text { get; set; } = new Dictionary<string, string>();
    }
}
