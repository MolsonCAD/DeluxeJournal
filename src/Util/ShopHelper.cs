using System.Reflection;
using Microsoft.Xna.Framework;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Util
{
    /// <summary>Facilitates attaching callbacks to ShopMenus.</summary>
    public static class ShopHelper
    {
        /// <summary>Attach an onPurchase callback to a ShopMenu.</summary>
        /// <param name="shop">The ShopMenu.</param>
        /// <param name="onPurchase">The callback to be attached. A return value of true exits the menu.</param>
        public static void AttachPurchaseCallback(ShopMenu shop, Func<ISalable, Farmer, int, bool> onPurchase)
        {
            Func<ISalable, Farmer, int, bool> origOnPurchase = shop.onPurchase;

            shop.onPurchase = delegate (ISalable salable, Farmer player, int amount)
            {
                bool exit = onPurchase(salable, player, amount);

                if (origOnPurchase != null)
                {
                    return origOnPurchase(salable, player, amount) || exit;
                }

                return exit;
            };
        }
    }
}
