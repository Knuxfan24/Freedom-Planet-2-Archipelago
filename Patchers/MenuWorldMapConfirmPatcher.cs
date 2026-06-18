using BepInEx.Bootstrap;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuWorldMapConfirmPatcher
    {
        /// <summary>
        /// Forces the Battlesphere to go to the Arena Menu.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start")]
        static void RedirectArena(MenuWorldMapConfirm __instance) => __instance.arenaSceneClassicIncomplete = __instance.arenaSceneClassic;

        /// <summary>
        /// Enables the extra item boxes if we have those slots unlocked.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start")]
        static void ExpandItemDisplay(MenuWorldMapConfirm __instance)
        {
            // Activate the unused Item 3 and 4 boxes if we've unlocked them.
            __instance.pfItemBox[2].SetActive(FPSaveManager.itemSlotExpansionLevel >= 1);
            __instance.pfItemBox[3].SetActive(FPSaveManager.itemSlotExpansionLevel >= 2);

            // If the Potion Seller mod (which this code comes from) isn't installed, then replicate its adjustment of the stage info's position.
            if (!Chainloader.PluginInfos.ContainsKey("com.eps.plugin.fp2.potion-seller"))
            {
                Offset("StageIcon", 22);
                Offset("StageName", 22);
                Offset("HideFromDialog/StageInfo", 22);
                Offset("HideFromDialog/HubInfo", 12);

                void Offset(string name, int offset)
                {
                    Transform transform = __instance.transform.Find(name).transform;
                    Vector3 position = transform.localPosition;
                    position.x = position.x + offset;
                    transform.localPosition = position;
                }
            }
        }
    }
}
