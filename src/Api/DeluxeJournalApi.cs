using Microsoft.Xna.Framework;
using DeluxeJournal.Menus;
using DeluxeJournal.Framework;

namespace DeluxeJournal.Api
{
    public class DeluxeJournalApi : IDeluxeJournalApi
    {
        private readonly PageManager _pageManager;

        internal DeluxeJournalApi(DeluxeJournalMod mod)
        {
            if (mod.PageManager == null)
            {
                throw new InvalidOperationException("Deluxe Journal API instantiated before mod entry.");
            }

            _pageManager = mod.PageManager;
        }

        public void RegisterPage(string id, Func<Rectangle, IPage> supplier, int order = 0)
        {
            _pageManager.RegisterPage(id, supplier, order);
        }
    }
}
