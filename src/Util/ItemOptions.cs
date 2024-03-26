using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using static DeluxeJournal.Util.ItemOptions;

namespace DeluxeJournal.Util
{
    public class ItemOptions
    {
        public class ListWithDefault<T>
        {
            public T? defaultItem;

            public List<T> items = new List<T>();

            public ListWithDefault()
            {
            }

                public ListWithDefault(T? defaultItem)
            {
                this.defaultItem = defaultItem;
                if (defaultItem != null)
                {
                    items.Add(defaultItem);
                }
            }

            public void setDefault(T value)
            {
                defaultItem = value;
            }

            public T? getDefault()
            {
                return defaultItem ?? items.FirstOrDefault();
            }

            public bool valueInList(T item)
            {
                return items.Contains(item);
            }


            public static ListWithDefault<T> operator +(ListWithDefault<T> a, T b)
            {
                if (a.defaultItem == null)
                {
                    a.defaultItem = b;
                }
                a.items.Add(b);
                return a;
            }
        }

        public class PlainTextMap<T> : Dictionary<string, ListWithDefault<T>>
        {
            public T? getDefault(string key)
            {
                return base[key].getDefault();
            }

            public void add(string key, T value)
            {
                if (base.ContainsKey(key)) base[key] += value;
                else base[key] = new ListWithDefault<T>(value);
            }

            public void addAsDefault(string key, T value)
            {
                if (base.ContainsKey(key)) base[key].setDefault(value);
                else base[key] = new ListWithDefault<T>(value);
            }
        }

        public ListWithDefault<Tool> ItemListAsTool(ListWithDefault<Item> items)
        {
            ListWithDefault<Tool> result = new ListWithDefault<Tool>();
            foreach (Item item in items.items)
            {
                if (item is Tool tool)
                {
                    result += tool;
                }
            }
            return result;
        }
    }
}
