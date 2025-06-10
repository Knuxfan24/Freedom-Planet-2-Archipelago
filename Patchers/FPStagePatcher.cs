﻿using System.IO;
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
            // Check that we actually have slot data to reference, as this function can get called before we connect.
            if (Plugin.slotData == null)
                return;

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

        /// <summary>
        /// Changes the moon sprite in Merga's battle depending on the other games in the multiworld.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPStage), "Start")]
        private static void MoonEasterEgg()
        {
            // Check if we're in Merga's boss map.
            if (SceneManager.GetActiveScene().name == "Bakunawa4Boss")
            {
                // Set up a list of graphic names.
                List<string> moonGraphics = [];

                // Loop through each player in the multiworld and add their moon graphic name if the file exists.
                foreach (Archipelago.MultiClient.Net.Helpers.PlayerInfo player in Plugin.session.Players.AllPlayers)
                {
                    if (player.Game == "Sonic Adventure 2 Battle" && File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\shattered_moon.png") && !moonGraphics.Contains("shattered_moon"))
                        moonGraphics.Add("shattered_moon");

                    if (player.Game == "Majora's Mask Recompiled" && File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\majora_moon.png") && !moonGraphics.Contains("majora_moon"))
                        moonGraphics.Add("majora_moon");
                }

                // Abort if we haven't gotten any moon graphics.
                if (moonGraphics.Count == 0)
                    return; 

                // Pick and create a sprite out of a graphic.
                Texture2D texture = new(230, 230) { filterMode = FilterMode.Trilinear };
                texture.LoadImage(File.ReadAllBytes($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{moonGraphics[Plugin.rng.Next(moonGraphics.Count)]}.png"));
                Sprite moonGraphic = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

                // Edit all four moon objects.
                MoonEdit(UnityEngine.GameObject.Find("Moon"));
                MoonEdit(UnityEngine.GameObject.Find("Moon Overlay"));
                MoonEdit(UnityEngine.GameObject.Find("Moon_Cutscene"));
                MoonEdit(UnityEngine.GameObject.Find("Moon_Start"));

                void MoonEdit(GameObject moon)
                {
                    if (moon != null)
                    {
                        moon.GetComponent<SpriteRenderer>().sprite = moonGraphic;
                        moon.transform.position = new(320, -48, moon.transform.position.z);
                    }
                }
            }
        }
    }
}
