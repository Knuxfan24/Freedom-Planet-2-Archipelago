namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuItemGetPatcher
    {
        /// <summary>
        /// Sends out a location check from a shop.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuItemGet), "Start")]
        static void SendShopLocationCheck(MenuItemGet __instance, ref MenuText ___itemName, ref FPHudDigit ___powerupIcon, ref FPHudDigit ___powerupShadow)
        {
            // Send the location based on the shop type.
            if (__instance.powerup != FPPowerup.NONE)
                Plugin.session.Locations.CompleteLocationChecks(Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", $"Milla Shop Item {(int)__instance.powerup - 1}"));
            else
                Plugin.session.Locations.CompleteLocationChecks(Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", $"Vinyl Shop Item {__instance.musicID}"));

            // Set the item name to the one stored in the Menu Shop Patcher.
            ___itemName.GetComponent<TextMesh>().text = MenuShopPatcher.SelectedItemName;

            // Set the sprite and its shadow to the one stored in the Menu Shop Patcher.
            ___powerupIcon.GetComponent<SpriteRenderer>().sprite = MenuShopPatcher.SelectedItemSprite;
            ___powerupShadow.GetComponent<SpriteRenderer>().sprite = MenuShopPatcher.SelectedItemSprite;

            // Save our files.
            Helpers.Save();
        }

        /// <summary>
        /// Removes 100 Rings upon buying a Vinyl, if RingLink is enabled.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuItemGet), "Start")]
        static void DeductRingLink(ref FPPowerup ___powerup)
        {
            // Check that this is a Vinyl and that our slot data has the ring_link flag.
            if (___powerup == FPPowerup.NONE/* && (long)Plugin.slotData["ring_link"] == 1*/)
            {
                // Remove our vinyl shop price from the RingLink value.
                Plugin.RingLinkCrystalCount -= (int)(long)Plugin.slotData["vinyl_shop_price"];

                // Reset the RingLink timer.
                Plugin.RingLinkTimer = 0f;
            }
        }
    }
}
