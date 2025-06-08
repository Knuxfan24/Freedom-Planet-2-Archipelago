using System.Linq;
using System.Text.RegularExpressions;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPBaseEnemyPatcher
    {
        /// <summary>
        /// "Enemy" types that shouldn't be in enemy sanity.
        /// </summary>
        private static readonly string[] Blacklist = ["Plant Block", "Large Bell"];

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPBaseEnemy), "SetDeath")]
        static void GenericEnemySanityHandler(FPBaseEnemy __instance)
        {
            // Disable this until I properly implement it.
            return;

            // Check if the player has enemy sanity on.
            if ((long)Plugin.slotData["enemies"] == 0)
                return;

            // Get this enemy's type.
            string enemyType = Regex.Replace(__instance.GetType().Name, "(\\B[A-Z])", " $1");

            // Check if the blacklist contains this enemy type and stop if it does.
            if (Blacklist.Contains(enemyType))
                return;

            // Get the location for this enemy type.
            long locationIndex = Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", enemyType);

            // Complete this location check if it exists, or print a notice if it doesn't.
            if (Helpers.CheckLocationExists(locationIndex))
                Plugin.session.Locations.CompleteLocationChecks(locationIndex);
            else
                Plugin.consoleLog.LogInfo($"No location found for defeating a {enemyType}?");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ProtoPincer), "State_Death")]
        static void ProtoPincerDeath() => BossDeath("Proto Pincer");
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TriggerJoy), "State_Death")]
        static void TriggerJoyDeath() => BossDeath("Proto Pincer");

        static void BossDeath(string bossName)
        {
            // Disable this until I properly implement it. Assuming I even decide to include bosses in it.
            return;

            // Check if the player has enemy sanity on.
            if ((long)Plugin.slotData["enemies"] == 0)
                return;

            // Get the location for this boss type.
            long locationIndex = Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", bossName);

            // Complete this location check if it exists.
            if (Helpers.CheckLocationExists(locationIndex))
                Plugin.session.Locations.CompleteLocationChecks(locationIndex);
        }
    }
}
