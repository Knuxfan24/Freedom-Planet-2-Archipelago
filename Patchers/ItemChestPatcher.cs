using Archipelago.MultiClient.Net.Models;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class ItemChestPatcher
    {
        /// <summary>
        /// Sets up a chest as we need it.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemChest), "Start")]
        static void SetupChest(ItemChest __instance)
        {
            // Overwrite this chest's contents to the random type and its music ID to none.
            __instance.contents = FPItemChestContent.RANDOM;
            __instance.musicID = FPMusicTrack.NONE;

            // Set up a variable to hold our scouted location's information.
            ScoutedItemInfo _scoutedLocationInfo = null;

            // Get the index of this chest's location.
            long locationIndex = GetLocationIndex(__instance);

            // If this location exists, then scout the location to handle things related to it.
            if (Helpers.CheckLocationExists(locationIndex))
            {
                // Scout this chest's location.
                Plugin.session.Locations.ScoutLocationsAsync(HandleScoutInfo, [locationIndex]);

                // Pause operation until the location is scouted.
                while (_scoutedLocationInfo == null)
                    System.Threading.Thread.Sleep(1);

                // Check that this chest hasn't been checked.
                if (!Plugin.session.Locations.AllLocationsChecked.Contains(locationIndex))
                {
                    // Remove the label message so the original game's "You got a Brave Stone!" message doesn't show.
                    __instance.labelMessage = string.Empty;

                    // Change this chest's type to the Powerup type.
                    __instance.contents = FPItemChestContent.POWERUP;

                    // Set this chest's item sprite to the correct one.
                    __instance.itemSprite = Helpers.GetItemSprite(_scoutedLocationInfo);
                }

                void HandleScoutInfo(Dictionary<long, ScoutedItemInfo> scoutedLocationInfo) => _scoutedLocationInfo = scoutedLocationInfo.First().Value;
            }

        }

        /// <summary>
        /// Stops this chest from being opened if strict tracers is on and this stage's tracer hasn't been acquired.
        /// TODO: This causes a few minor cosmetic problems with the doors in Globe Opera 2 (for example).
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemChest), "State_Idle")]
        static bool LockChestStateIdle(ItemChest __instance)
        {
            // Determine if we SHOULDN'T lock this chest.
            bool shouldntLock = UnlockChest();

            // Grab the button prompt used on the HUD.
            GameObject buttonPrompt = UnityEngine.GameObject.FindObjectOfType<FPHudMaster>().buttonPrompt;

            // Check if this chest shouldn't be locked.
            if (!shouldntLock)
            {
                // Set up a value to hold an object reference.
                FPBaseObject objRef = null;

                // Loop through every player in the stage.
                while (FPStage.ForEach(FPPlayer.classID, ref objRef))
                {
                    // Store this player.
                    FPPlayer fPPlayer = (FPPlayer)objRef;

                    // Check if this player isn't in their KO or recovery states and is touching the hitbox of this chest.
                    if (!fPPlayer.IsKOdOrRecovering() && FPCollision.CheckOOBB(__instance, (FPHitBox)Traverse.Create(__instance).Field("hbTouch").GetValue(), fPPlayer, fPPlayer.hbTouch))
                    {
                        // Change the player's button prompt message to Tracer Required!
                        fPPlayer.buttonPromptAction = "Tracer Required!";

                        // Standard player button prompt set up.
                        fPPlayer.buttonPromptLocation = __instance.transform.position + new Vector3(0f, 100f, 0f);
                        fPPlayer.buttonPromptTimer = 0f;

                        // Disable the banner behind the text, as our string is too long for it.
                        buttonPrompt.GetComponent<SpriteRenderer>().enabled = false;

                        // Disable the script for the button graphic and remove its sprite.
                        buttonPrompt.transform.GetChild(1).GetComponent<MenuButtonGraphic>().enabled = false;
                        buttonPrompt.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                    }
                }
            }
            else
            {
                // Reenable the button prompt banner and graphic script.
                buttonPrompt.GetComponent<SpriteRenderer>().enabled = true;
                buttonPrompt.transform.GetChild(1).GetComponent<MenuButtonGraphic>().enabled = true;
            }

            // Either run or don't run the original function.
            return shouldntLock;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemChest), "InteractWithObjects")]
        static bool LockChestInteractWithObjects() => UnlockChest();

        /// <summary>
        /// Determines if a chest should be unlocked.
        /// </summary>
        /// <returns></returns>
        static bool UnlockChest()
        {
            // Check if we have strict chest tracers enabled.
            if ((long)Plugin.slotData["chest_tracer_strict"] == 1)
            {
                // Check that the chest tracer items exist in some form in the first place.
                if ((long)Plugin.slotData["chest_tracer_items"] != 0 || (long)Plugin.slotData["chest_tracer_global"] != 0)
                {
                    // Return whether or not this stage's chest tracer is acquired.
                    switch (FPStage.currentStage.stageID)
                    {
                        case 1: return Plugin.save.ChestTracers[0];
                        case 2: return Plugin.save.ChestTracers[1];
                        case 3: return Plugin.save.ChestTracers[2];
                        case 4: return Plugin.save.ChestTracers[3] && SceneManager.GetActiveScene().name == "AirshipSigwada";
                        case 5: return Plugin.save.ChestTracers[4];
                        case 6: return Plugin.save.ChestTracers[5];
                        case 7: return Plugin.save.ChestTracers[6];
                        case 9: return Plugin.save.ChestTracers[7];
                        case 10: return Plugin.save.ChestTracers[8];
                        case 11: return Plugin.save.ChestTracers[9];
                        case 12: return Plugin.save.ChestTracers[10];
                        case 14: return Plugin.save.ChestTracers[11];
                        case 15: return Plugin.save.ChestTracers[12];
                        case 16: return Plugin.save.ChestTracers[13];
                        case 17: return Plugin.save.ChestTracers[14];
                        case 18: return Plugin.save.ChestTracers[15];
                        case 19: return Plugin.save.ChestTracers[16];
                        case 20: return Plugin.save.ChestTracers[17];
                        case 21: return Plugin.save.ChestTracers[18];
                        case 23: return Plugin.save.ChestTracers[19];
                        case 24: return Plugin.save.ChestTracers[20];
                        case 26: return Plugin.save.ChestTracers[21];
                        case 27: return Plugin.save.ChestTracers[22];
                        case 28: return Plugin.save.ChestTracers[23];
                    }
                }
            }

            // If we've gotten down to here, then assume this chest should be unlocked.
            return true;
        }

        /// <summary>
        /// Sends a location check out upon opening a chest.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemChest), "BoxHit")]
        static void SendLocationCheck(ItemChest __instance)
        {
            // Get the index of this chest's location.
            long locationIndex = GetLocationIndex(__instance);

            // If this location exists, then complete the check of it.
            if (Helpers.CheckLocationExists(locationIndex) && !Plugin.session.Locations.AllLocationsChecked.Contains(locationIndex))
            {
                // Complete the location check for this index.
                Plugin.session.Locations.CompleteLocationChecks(locationIndex);

                // Scout the location we just completed.
                ScoutedItemInfo _scoutedLocationInfo = null;
                Plugin.session.Locations.ScoutLocationsAsync(HandleScoutInfo, [locationIndex]);

                // Pause operation until the location is scouted.
                while (_scoutedLocationInfo == null)
                    System.Threading.Thread.Sleep(1);

                // Add a message to the queue if this item is for someone else.
                if (_scoutedLocationInfo.Player.Name != Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot))
                    Plugin.sentMessageQueue.Add($"Found {_scoutedLocationInfo.Player.Name}'s {_scoutedLocationInfo.ItemName}.");

                // Swap the chest's contents to music so that we don't accidentally clobber our shop.
                __instance.contents = FPItemChestContent.MUSIC;

                // Reset the chest tracers to remove this chest from it.
                FPPlayerPatcher.CreateChestTracers();

                void HandleScoutInfo(Dictionary<long, ScoutedItemInfo> scoutedLocationInfo) => _scoutedLocationInfo = scoutedLocationInfo.First().Value;
            }
        }

        /// <summary>
        /// Gets the index this chest's location.
        /// </summary>
        static long GetLocationIndex(ItemChest __instance)
        {
            // Set up a value to hold the chest position read from our hardcoded lists.
            KeyValuePair<string, Vector2> chestPosition = new("", Vector2.zero);

            // Look up this chest's position based on our hardcoded list of chest positions.
            switch (FPStage.currentStage.stageID)
            {
                case 1: chestPosition = ChestLists.DragonValley.FirstOrDefault(x => x.Value == __instance.position); break;
                case 2: chestPosition = ChestLists.ShenlinPark.FirstOrDefault(x => x.Value == __instance.position); break;
                case 3: chestPosition = ChestLists.AvianMuseum.FirstOrDefault(x => x.Value == __instance.position); break;
                case 4: chestPosition = ChestLists.AirshipSigwada.FirstOrDefault(x => x.Value == __instance.position); break;
                case 5: chestPosition = ChestLists.TigerFalls.FirstOrDefault(x => x.Value == __instance.position); break;
                case 6: chestPosition = ChestLists.RobotGraveyard.FirstOrDefault(x => x.Value == __instance.position); break;
                case 7: chestPosition = ChestLists.ShadeArmory.FirstOrDefault(x => x.Value == __instance.position); break;
                case 9: chestPosition = ChestLists.PhoenixHighway.FirstOrDefault(x => x.Value == __instance.position); break;
                case 10: chestPosition = ChestLists.ZaoLand.FirstOrDefault(x => x.Value == __instance.position); break;
                case 11: chestPosition = ChestLists.GlobeOpera1.FirstOrDefault(x => x.Value == __instance.position); break;
                case 12: chestPosition = ChestLists.GlobeOpera2.FirstOrDefault(x => x.Value == __instance.position); break;
                case 14: chestPosition = ChestLists.PalaceCourtyard.FirstOrDefault(x => x.Value == __instance.position); break;
                case 15: chestPosition = ChestLists.TidalGate.FirstOrDefault(x => x.Value == __instance.position); break;
                case 16: chestPosition = ChestLists.ZulonJungle.FirstOrDefault(x => x.Value == __instance.position); break;
                case 17: chestPosition = ChestLists.NalaoLake.FirstOrDefault(x => x.Value == __instance.position); break;
                case 18: chestPosition = ChestLists.SkyBridge.FirstOrDefault(x => x.Value == __instance.position); break;
                case 19: chestPosition = ChestLists.LightningTower.FirstOrDefault(x => x.Value == __instance.position); break;
                case 20: chestPosition = ChestLists.AncestralForge.FirstOrDefault(x => x.Value == __instance.position); break;
                case 21: chestPosition = ChestLists.MagmaStarscape.FirstOrDefault(x => x.Value == __instance.position); break;
                case 23: chestPosition = ChestLists.GravityBubble.FirstOrDefault(x => x.Value == __instance.position); break;
                case 24: chestPosition = ChestLists.BakunawaRush.FirstOrDefault(x => x.Value == __instance.position); break;
                case 26: chestPosition = ChestLists.ClockworkArboretum.FirstOrDefault(x => x.Value == __instance.position); break;
                case 27: chestPosition = ChestLists.InversionDynamo.FirstOrDefault(x => x.Value == __instance.position); break;
                case 28: chestPosition = ChestLists.LunarCannon.FirstOrDefault(x => x.Value == __instance.position); break;

                default:
                    Plugin.consoleLog.LogError($"No chest handling present for stage ID {FPStage.currentStage.stageID} ({FPStage.currentStage.stageName})!");
                    return -1;
            }

            // Check that we actually have a key and position. If not, then throw an error.
            if (chestPosition.Key == "" || chestPosition.Value == Vector2.zero)
            {
                Plugin.consoleLog.LogError($"No chest found for position {__instance.position} in stage ID {FPStage.currentStage.stageID} ({FPStage.currentStage.stageName})!");
                return -1;
            }

            // Return the index of this chest's location.
            return Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", chestPosition.Key);
        }
    }
}
