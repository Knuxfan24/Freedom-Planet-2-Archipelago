namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuGlobalPausePatcher
    {
        /// <summary>
        /// Redirects the game quit to the Title Screen rather than the Main Menu and disconnects us from the server.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause), "State_Transition")]
        static void ReturnToTitle()
        {
            FPSaveManager.menuToLoad = 4;
            Plugin.session.Socket.Disconnect();
        }

        /// <summary>
        /// Replaces the Core Counter in the menu with a Time Capsule counter.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause), "Start")]
        static void ReplaceCores(MenuGlobalPause __instance)
        {
            __instance.overviewCounters[3].GetComponent<TextMesh>().text = Plugin.save.TimeCapsuleCount.ToString();
            __instance.overviewCounters[3].transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Plugin.apAssetBundle.LoadAsset<Sprite>("time_capsule");
        }
    }
}
