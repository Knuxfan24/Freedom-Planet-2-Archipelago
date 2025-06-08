namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuTitleScreenPatcher
    {
        /// <summary>
        /// Swaps out the main menu call with the connection menu instead.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuTitleScreen), "Start")]
        static void ReplaceMainMenu(MenuTitleScreen __instance)
        {
            __instance.nextMenu = Plugin.apAssetBundle.LoadAsset<GameObject>("Connection Menu");
            __instance.nextMenu.AddComponent<MenuConnection>();
        }
    }
}
