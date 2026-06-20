using BepInEx.Bootstrap;

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

        /// <summary>
        /// Lights up the obtained Chaos Emeralds in the pause menu. The actual icons are added by the Sonic mod itself.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause), "LateUpdate")]
        static void UpdateChaosEmeralds()
        {
            // Don't bother with this if the Sonic mod isn't installed.
            if (!Chainloader.PluginInfos.ContainsKey("K24_FP2_Sonic"))
                return;

            // TODO: The display still appears even if the Sonic Mod Compatibility option is disabled.

            // Find and set the alpha to 1 on each emerald depending on the collected ones.
            if (Plugin.save.SonicChaosEmeralds[0]) UnityEngine.GameObject.Find("Red Chaos Emerald").GetComponent<SpriteRenderer>().color = new(1, 1, 1, 1);
            if (Plugin.save.SonicChaosEmeralds[1]) UnityEngine.GameObject.Find("Blue Chaos Emerald").GetComponent<SpriteRenderer>().color = new(1, 1, 1, 1);
            if (Plugin.save.SonicChaosEmeralds[2]) UnityEngine.GameObject.Find("Yellow Chaos Emerald").GetComponent<SpriteRenderer>().color = new(1, 1, 1, 1);
            if (Plugin.save.SonicChaosEmeralds[3]) UnityEngine.GameObject.Find("Green Chaos Emerald").GetComponent<SpriteRenderer>().color = new(1, 1, 1, 1);
            if (Plugin.save.SonicChaosEmeralds[4]) UnityEngine.GameObject.Find("White Chaos Emerald").GetComponent<SpriteRenderer>().color = new(1, 1, 1, 1);
            if (Plugin.save.SonicChaosEmeralds[5]) UnityEngine.GameObject.Find("Cyan Chaos Emerald").GetComponent<SpriteRenderer>().color = new(1, 1, 1, 1);
            if (Plugin.save.SonicChaosEmeralds[6]) UnityEngine.GameObject.Find("Purple Chaos Emerald").GetComponent<SpriteRenderer>().color = new(1, 1, 1, 1);
        }
    }
}
