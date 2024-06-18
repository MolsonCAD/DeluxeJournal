using System.Text;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using DeluxeJournal.Task;

namespace DeluxeJournal
{
    internal static class ConsoleCommands
    {
        private static DeluxeJournalMod Mod => DeluxeJournalMod.Instance!;

        private static IMonitor Monitor => Mod.Monitor;

        public static void AddCommands(ICommandHelper helper)
        {
            helper.Add("dj_colors_gen",
                DocString("Generate a simple color schema from a single source color.",
                    "dj_colors_gen <index> <color>",
                    "index: Index of the color schema to replace. Appends a new schema if index equals the length of the list.",
                    "color: Color hex code."),
                GenColorSchema);

            helper.Add("dj_colors_new",
                DocString("Add a new color schema.",
                    "dj_colors_new <color:main> <color:hover> <color:header> <color:accent> <color:shadow> [color:padding] [color:corner]",
                    "color: Color hex code."),
                NewColorSchema);

            helper.Add("dj_colors_edit",
                DocString("Edit a color in an existing color schema.",
                    "dj_colors_edit <index> <key> <color>",
                    "index: Index of the color schema to edit.",
                    "key: Name of the color property key. E.g. 'Main'.",
                    "color: Color hex code."),
                EditColorSchema);

            helper.Add("dj_colors_remove",
                DocString("Remove a color schema by index.",
                    "dj_colors_remove <index>",
                    "index: Index of the color schema to remove."),
                RemoveColorSchema);

            helper.Add("dj_colors_load",
                DocString("Load color data.",
                    "dj_colors_load [filename]",
                    "filename: Optional file name. Defaults to 'colors-default.json'."),
                LoadColorSchema);

            helper.Add("dj_colors_save",
                DocString("Save color data.",
                    "dj_colors_save [filename]",
                    "filename: Optional file name. Defaults to 'colors-saved.json'."),
                SaveColorSchema);

            helper.Add("dj_tasks_save", "Save the current state of the tasks list.", (string _, string[] _) => DeluxeJournalMod.TaskManager?.Save());
        }

        private static void LoadColorSchema(string command, string[] args)
        {
            string path;

            if (args.Length > 0)
            {
                string arg = args[0];
                path = DeluxeJournalMod.DataPath + "/";

                if (!arg.StartsWith("colors-"))
                {
                    path += "colors-";
                }

                path += arg;

                if (!arg.EndsWith(".json"))
                {
                    path += ".json";
                }
            }
            else
            {
                path = DeluxeJournalMod.ColorDataPath;
            }

            Mod.LoadColorSchemas(path);
            Monitor.Log($"Loaded color data from '{path}'.", LogLevel.Info);
        }

        private static void SaveColorSchema(string command, string[] args)
        {
            string path = DeluxeJournalMod.DataPath;

            if (args.Length > 0)
            {
                string arg = args[0].ToLower();
                path += "/";

                if (!arg.StartsWith("colors-"))
                {
                    path += "colors-";
                }

                path += arg;

                if (!arg.EndsWith(".json"))
                {
                    path += ".json";
                }
            }
            else
            {
                path += "/colors-saved.json";
            }

            Mod.SaveColorSchemas(path);
            Monitor.Log($"Saved color data to '{path}'.", LogLevel.Info);
        }

        private static void GenColorSchema(string command, string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Insufficient number of arguments.");
            }

            int index = IndexFromArg(args[0], true);
            Color main = ColorFromArg(args[1]);

            ColorSchema.ColorToHSV(main, out float _, out float _, out float mainValue);
            float luminance = ColorSchema.Luminance(main);
            float satBoost = 1f - mainValue;
            float valueShift = Math.Min(0, ColorSchema.ValueShiftLumThreshold - luminance);

            Color hover = ColorSchema.HSVShiftColor(main, ColorSchema.HueShift, ColorSchema.HoverMaxSatShift * luminance + satBoost, valueShift, 250f);
            Color header = ColorSchema.HSVShiftColor(main, ColorSchema.HueShift, ColorSchema.HeaderMaxSatShift * luminance + satBoost, valueShift, 250f);
            Color accent = ColorSchema.HSVShiftColor(main, 14f, 0.45f, -0.52f, 250f);
            Color shadow = ColorSchema.HSVShiftColor(main, 5f, 0.18f, -0.1f, 250f);

            ColorSchema schema = new(main, hover, header, accent, shadow);
            Monitor.Log($"Generated color schema: {schema}", LogLevel.Info);

            if (index == DeluxeJournalMod.ColorSchemas.Count)
            {
                DeluxeJournalMod.ColorSchemas.Add(schema);
            }
            else
            {
                DeluxeJournalMod.ColorSchemas[index] = schema;
            }
        }

        private static void NewColorSchema(string command, string[] args)
        {
            ExtractColorsFromArgs(args, out Color main, out Color hover, out Color header, out Color accent, out Color shadow, out Color? padding, out Color? corner);
            DeluxeJournalMod.ColorSchemas.Add(new(main, hover, header, accent, shadow, padding, corner));
        }

        private static void EditColorSchema(string command, string[] args)
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("Insufficient number of arguments.");
            }

            ColorSchema schema = DeluxeJournalMod.ColorSchemas[IndexFromArg(args[0])];
            string key = args[1];

            foreach (var property in typeof(ColorSchema).GetProperties())
            {
                if (property.Name.Equals(key, StringComparison.OrdinalIgnoreCase) && property.PropertyType == typeof(Color))
                {
                    property.SetValue(schema, ColorFromArg(args[2]));
                    Monitor.Log($"Modified color schema: {schema}", LogLevel.Info);
                    return;
                }
            }

            throw new ArgumentException($"Invalid key name: '{key}'.");
        }

        private static void RemoveColorSchema(string command, string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("Insufficient number of arguments.");
            }

            int index = IndexFromArg(args[0]);

            Monitor.Log($"Removed color schema: {DeluxeJournalMod.ColorSchemas[index]}", LogLevel.Info);
            DeluxeJournalMod.ColorSchemas.RemoveAt(index);
        }

        private static void ExtractColorsFromArgs(string[] args, out Color main, out Color hover, out Color header, out Color accent, out Color shadow, out Color? padding, out Color? corner, int start = 0)
        {
            if (args.Length < start + 5)
            {
                throw new ArgumentException("Insufficient number of arguments.");
            }

            main = ColorFromArg(args[start]);
            hover = ColorFromArg(args[start + 1]);
            header = ColorFromArg(args[start + 2]);
            accent = ColorFromArg(args[start + 3]);
            shadow = ColorFromArg(args[start + 4]);
            padding = args.Length > start + 5 ? ColorFromArg(args[start + 5]) : null;
            corner = args.Length > start + 6 ? ColorFromArg(args[start + 6]) : null;
        }

        private static Color ColorFromArg(string arg)
        {
            if (ColorSchema.HexToColor(arg) is not Color color)
            {
                throw new ArgumentException("Invalid hex color code.");
            }

            return color;
        }

        private static int IndexFromArg(string arg, bool allowAppend = false)
        {
            int count = DeluxeJournalMod.ColorSchemas.Count;
            int index;

            if (arg == "0")
            {
                throw new ArgumentException("Index 0 is reserved for the auto-generated default color schema and should not be modified! To bypass this, use index '0!'.");
            }
            else if (!int.TryParse(arg.TrimEnd('!'), out index))
            {
                throw new ArgumentException("Could not parse color schema index.");
            }

            if (index < 0 || index > count || (!allowAppend && index == count))
            {
                throw new IndexOutOfRangeException($"Invalid color schema index. Number of loaded color schemas: {count}");
            }

            return index;
        }

        /// <summary>Builds a command documentation string.</summary>
        /// <param name="brief">Brief description.</param>
        /// <param name="usage">Command usage format.</param>
        /// <param name="args">Command argument help strings.</param>
        private static string DocString(string brief, string usage, params string[] args)
        {
            StringBuilder doc = new(brief);

            if (!string.IsNullOrEmpty(usage))
            {
                doc.Append("\n\n Usage: ");
                doc.Append(usage);

                foreach (string arg in args)
                {
                    doc.Append("\n\t");
                    doc.Append(arg);
                }
            }

            return doc.ToString();
        }
    }
}
