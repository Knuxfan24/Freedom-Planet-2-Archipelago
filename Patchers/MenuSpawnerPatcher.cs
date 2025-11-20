namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuSpawnerPatcher
    {
        /// <summary>
        /// Forces the Arena Menu to instantly pull up the challenge menu.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuSpawner), "Start")]
        static void ReplaceBattlesphereMenu(MenuSpawner __instance)
        {
            if (SceneManager.GetActiveScene().name == "ArenaMenu")
                __instance.menuList[0] = __instance.menuList[0].gameObject.GetComponent<MenuArena>().challengeMenu.gameObject;
        }
    }
}
