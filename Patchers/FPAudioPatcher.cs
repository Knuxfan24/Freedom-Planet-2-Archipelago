using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPAudioPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPAudio), "Start")]
        static void FastWeaponsCore(FPAudio __instance)
        {
            // If the player has "Fast Weapon's Core" enabled and we're in Bakunawa5, then remove the stage music so it doesn't play for a few frames.
            if (SceneManager.GetActiveScene().name == "Bakunawa5" && (long)Plugin.slotData["fast_weapons_core"] == 1)
                __instance.bgmStage = null;
        }
    }
}
