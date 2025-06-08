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
    }
}
