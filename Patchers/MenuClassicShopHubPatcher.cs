namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuClassicShopHubPatcher
    {
        /// <summary>
        /// Locks the shops that don't have any locations based on the YAML options.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassicShopHub), "Start")]
        static void LockUnusedShops(MenuClassicShopHub __instance)
        {
            // Lock Milla's shop and the gem exchange if its disabled.
            if ((long)Plugin.slotData["milla_shop"] == 0)
            {
                Lock(0);
                Lock(2);
            }

            // Lock the vinyl shop if its disbled.
            if ((long)Plugin.slotData["vinyl_shop"] == 0)
            {
                Lock(1);
            }

            void Lock(int shopIndex)
            {
                // Set the chosen shop to be locked.
                __instance.menuOptions[shopIndex].locked = true;

                // Set the chosen shop's lock sprite, as the menu's prefab doesn't actually have it set.
                __instance.menuOptions[shopIndex].GetComponent<MenuOption>().lockedSprite = Plugin.apAssetBundle.LoadAsset<Sprite>("lock");
            }
        }
    }
}
