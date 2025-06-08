using System.Linq;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPStagePatcher
    {
        /// <summary>
        /// Resets the player character ID to undo a swap trap.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPStage), "Start")]
        static void ResetCharacter()
        {
            // Only do this if we're not using a random character.
            if (!Plugin.usingRandomCharacter)
                FPSaveManager.character = (FPCharacterID)MenuConnection.characters.ElementAt(MenuConnection.characterIndex).Value;
        }

        /// <summary>
        /// Adds extra objects to make a few problematic chests obtainable.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPStage), "Start")]
        private static void AddExtraObjects()
        {
            // Only do this if we're actually using chest locations.
            if ((long)Plugin.slotData["chests"] == 0)
                return;

            switch (SceneManager.GetActiveScene().name)
            {
                // If we're in Globe Opera, then clone one of the Lantern Platforms to make reaching Chest 4 possible.
                case "GlobeOpera1":
                    var goPlatform = GameObject.Instantiate(UnityEngine.GameObject.Find("GO_LanternPlatform (17)"), new(32048, -720, 0), Quaternion.identity);
                    goPlatform.name = "Chest Platform";
                    break;

                // If we're in Sky Bridge, then clone the two parts of one of the platforms to make reaching Chest 2 possible with the help of the Super Feather potion.
                case "SkyBridge":
                    var sbPlatformL = GameObject.Instantiate(UnityEngine.GameObject.Find("hb_bigplatform_0 (4)"), new(6144, -4128, 0), Quaternion.identity);
                    sbPlatformL.name = "Chest Platform L";
                    var sbPlatformR = GameObject.Instantiate(UnityEngine.GameObject.Find("hb_bigplatform_0 (5)"), new(6432, -4128, 0), Quaternion.identity);
                    sbPlatformR.name = "Chest Platform R";
                    break;

                // If we're in Nalao Lake, then clone one of the Rising Bubbles to make reaching Chest 3 possible with the help of the Super Feather potion.
                case "NalaoLake":
                    var nlBubble = GameObject.Instantiate(UnityEngine.GameObject.Find("NL_RisingBubble (12)"), new(20600, -984, 0), Quaternion.identity);
                    nlBubble.name = "Chest Bubble";
                    break;
            }
        }
    }
}
