using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class PlayerSpawnPointPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSpawnPoint), "Start")]
        static void FastWeaponsCore(PlayerSpawnPoint __instance)
        {
            // If the player has "Fast Weapon's Core" enabled and we're in Bakunawa5, then move the spawn point to the boss arena.
            if (SceneManager.GetActiveScene().name == "Bakunawa5" && (long)Plugin.slotData["fast_weapons_core"] == 1)
                __instance.transform.position = new(53032, -2576);
        }
    }
}
