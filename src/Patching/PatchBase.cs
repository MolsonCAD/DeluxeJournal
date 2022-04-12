using HarmonyLib;
using StardewModdingAPI;

namespace DeluxeJournal.Patching
{
    /// <summary>Base class for singleton IPatches.</summary>
    /// <typeparam name="T">Inheriting type to create an instance of.</typeparam>
    internal abstract class PatchBase<T> : IPatch where T : IPatch
    {
        private static T? _instance;

        /// <summary>Patch instance. This should be set by the derived class.</summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException(typeof(T).FullName + " instance accessed before assignment!");
                }

                return _instance;
            }

            protected set
            {
                if (_instance != null)
                {
                    throw new InvalidOperationException(typeof(T).FullName + " has already been instantiated!");
                }

                _instance = value;
            }
        }

        public string Name => GetType().FullName ?? GetType().Name;

        protected IMonitor Monitor { get; }

        public PatchBase(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public abstract void Apply(Harmony harmony);

        protected void LogError(Exception ex, string methodName)
        {
            Monitor.Log(string.Format("Failed in patch {0}.{1}: {2}", Name, methodName, ex), LogLevel.Error);
        }
    }
}
