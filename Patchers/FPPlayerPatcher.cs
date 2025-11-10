using FP2Lib.Player;
using Newtonsoft.Json.Linq;
using Rewired;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPPlayerPatcher
    {
        /// <summary>
        /// Holds a reference to the player's object.
        /// </summary>
        public static FPPlayer player;

        /// <summary>
        /// Whether or not we have a DeathLink queued.
        /// </summary>
        public static bool hasBufferedDeathLink;

        /// <summary>
        /// Whether or not we can send a DeathLink out.
        /// </summary>
        public static bool canSendDeathLink = true;

        /// <summary>
        /// Whether or not we have a Powerup queued.
        /// </summary>
        public static bool hasBufferedPowerup;

        /// <summary>
        /// How many lives (if any) we have queued.
        /// </summary>
        public static int hasBufferedExtraLives;

        /// <summary>
        /// Whether or not we have an Invincibility queued.
        /// </summary>
        public static bool hasBufferedInvincibility;

        /// <summary>
        /// Whether or not we have a shield queued.
        /// </summary>
        public static FPItemBoxTypes hasBufferedShield = FPItemBoxTypes.BOX_CRATE;

        /// <summary>
        /// The list of active Chest Tracers.
        /// </summary>
        static readonly List<GameObject> chestTracers = [];

        /// <summary>
        /// An animator containing the character's shocked animation.
        /// </summary>
        public static AnimatorOverrideController overrideAnimator = null;
        
        /// <summary>
        /// The character's original animator.
        /// </summary>
        public static RuntimeAnimatorController storedAnimator = null;
        
        /// <summary>
        /// The character's original vaItemGet AudioClip array.
        /// </summary>
        public static AudioClip[] storedItemVoices = null;

        /// <summary>
        /// Initial set up of the player's object.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Start")]
        private static void Setup(FPPlayer __instance)
        {
            // Store this player for future reference.
            player = __instance;

            // Reset the flag to allow us to send out DeathLinks.
            canSendDeathLink = true;

            // Create the chest tracers.
            CreateChestTracers();

            // Tell the Data Storage where we are if we're using remote players.
            if (Plugin.configRemotePlayers.Value == true)
            {
                JObject playerJObject = Plugin.session.DataStorage[$"FP2_PlayerSlot{Plugin.session.ConnectionInfo.Slot}"].To<JObject>();
                playerJObject["PositionX"] = player.position.x;
                playerJObject["PositionY"] = player.position.y;
                Plugin.session.DataStorage[$"FP2_PlayerSlot{Plugin.session.ConnectionInfo.Slot}"] = playerJObject;
            }
        }

        /// <summary>
        /// Creates the tracers pointing to each chest in the current stage.
        /// </summary>
        public static void CreateChestTracers()
        {
            // Log the creation of the chest tracers, as a debug log.
            Plugin.consoleLog?.LogDebug("[ChestTracers] Enter CreateChestTracers");

            // Flag to check if tracers should be hidden upon creation.
            bool shouldKeepHidden = false;

            // Validate slotData and chest_tracers key.
            if (Plugin.slotData == null)
            {
                Plugin.consoleLog?.LogWarning("[ChestTracers] Plugin.slotData is null. Aborting tracer creation.");
                return;
            }
            if (!Plugin.slotData.ContainsKey("chest_tracers"))
            {
                Plugin.consoleLog?.LogWarning("[ChestTracers] slotData missing key 'chest_tracers'. Aborting tracer creation.");
                return;
            }

            // Only do this if we actually have chest tracers enabled.
            try
            {
                if ((long)Plugin.slotData["chest_tracers"] == 0)
                {
                    // Log the fact that the tracers are disabled, as a debug log.
                    Plugin.consoleLog?.LogDebug("[ChestTracers] Chest tracers disabled by slotData.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Plugin.consoleLog?.LogError($"[ChestTracers] Failed to read 'chest_tracers' value: {ex.Message}");
                return;
            }

            // Validate current stage
            if (FPStage.currentStage == null)
            {
                Plugin.consoleLog?.LogError("[ChestTracers] FPStage.currentStage is null. Aborting tracer creation.");
                return;
            }

            // Validate save and tracer flags array
            if (Plugin.save == null)
            {
                Plugin.consoleLog?.LogError("[ChestTracers] Plugin.save is null. Aborting tracer creation.");
                return;
            }
            if (Plugin.save.ChestTracers == null)
            {
                Plugin.consoleLog?.LogError("[ChestTracers] Plugin.save.ChestTracers is null. Aborting tracer creation.");
                return;
            }

            // Destroy each active tracer then clear the list of them.
            // Also checks if the hidden flag should be set.
            int beforeCount = chestTracers?.Count ?? 0;
            Plugin.consoleLog?.LogDebug($"[ChestTracers] Cleaning up previous tracers. Count={beforeCount}");
            foreach (GameObject tracer in chestTracers)
            {
                if (tracer == null)
                {
                    Plugin.consoleLog?.LogDebug("[ChestTracers] Found null tracer reference during cleanup.");
                    continue;
                }

                if (!tracer.transform.GetChild(0).gameObject.activeSelf)
                    shouldKeepHidden = true;

                GameObject.Destroy(tracer);
            }
            chestTracers.Clear();

            // Set up a list to hold the chest locations.
            List<Vector3> locations = [];
            
            try
            {
                bool GetFlag(int idx) => Plugin.save.ChestTracers != null && idx >= 0 && idx < Plugin.save.ChestTracers.Length && Plugin.save.ChestTracers[idx];

                switch (FPStage.currentStage.stageID)
                {
                    case 1:  if (GetFlag(0))  GetChests(ChestLists.DragonValley); break;
                    case 2:  if (GetFlag(1))  GetChests(ChestLists.ShenlinPark); break;
                    case 3:  if (GetFlag(2))  GetChests(ChestLists.AvianMuseum); break;
                    case 4:  if (GetFlag(3)  && SceneManager.GetActiveScene().name == "AirshipSigwada") GetChests(ChestLists.AirshipSigwada); break;
                    case 5:  if (GetFlag(4))  GetChests(ChestLists.TigerFalls); break;
                    case 6:  if (GetFlag(5))  GetChests(ChestLists.RobotGraveyard); break;
                    case 7:  if (GetFlag(6))  GetChests(ChestLists.ShadeArmory); break;
                    case 9:  if (GetFlag(7))  GetChests(ChestLists.PhoenixHighway); break;
                    case 10: if (GetFlag(8))  GetChests(ChestLists.ZaoLand); break;
                    case 11: if (GetFlag(9))  GetChests(ChestLists.GlobeOpera1); break;
                    case 12: if (GetFlag(10)) GetChests(ChestLists.GlobeOpera2); break;
                    case 14: if (GetFlag(11)) GetChests(ChestLists.PalaceCourtyard); break;
                    case 15: if (GetFlag(12)) GetChests(ChestLists.TidalGate); break;
                    case 16: if (GetFlag(13)) GetChests(ChestLists.ZulonJungle); break;
                    case 17: if (GetFlag(14)) GetChests(ChestLists.NalaoLake); break;
                    case 18: if (GetFlag(15)) GetChests(ChestLists.SkyBridge); break;
                    case 19: if (GetFlag(16)) GetChests(ChestLists.LightningTower); break;
                    case 20: if (GetFlag(17)) GetChests(ChestLists.AncestralForge); break;
                    case 21: if (GetFlag(18)) GetChests(ChestLists.MagmaStarscape); break;
                    case 23: if (GetFlag(19)) GetChests(ChestLists.GravityBubble); break;
                    case 24: if (GetFlag(20)) GetChests(ChestLists.BakunawaRush); break;
                    case 26: if (GetFlag(21)) GetChests(ChestLists.ClockworkArboretum); break;
                    case 27: if (GetFlag(22)) GetChests(ChestLists.InversionDynamo); break;
                    case 28: if (GetFlag(23)) GetChests(ChestLists.LunarCannon); break;
                    default:
                        Plugin.consoleLog?.LogDebug($"[ChestTracers] No tracer list for stageID={FPStage.currentStage.stageID}. Locations remain empty.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Plugin.consoleLog?.LogError($"[ChestTracers] Exception while building location list: {ex.Message}");
            }

            Plugin.consoleLog?.LogDebug($"[ChestTracers] Locations collected: {locations.Count}");

            // Validate asset bundle before instantiation. This situation shouldn't ever come up, but just in case.
            if (Plugin.apAssetBundle == null)
            {
                Plugin.consoleLog?.LogError("[ChestTracers] apAssetBundle is null. Cannot instantiate tracer prefabs.");
                return;
            }

            // Loop through each read location.
            int created = 0;
            foreach (Vector2 location in locations)
            {
                // Create the tracer's game object.
                GameObject prefab = Plugin.apAssetBundle.LoadAsset<GameObject>("Chest Tracer");

                GameObject tracerPrefab = null;
                try
                {
                    tracerPrefab = GameObject.Instantiate(prefab);
                }
                catch (Exception ex)
                {
                    Plugin.consoleLog?.LogError($"[ChestTracers] Instantiate failed: {ex.Message}");
                    continue;
                }

                // Create and attach a tracer script to the game object, setting its targer position to this location.
                var tracerScript = tracerPrefab.AddComponent<ChestTracer>();
                tracerScript.targetPosition = location;

                // Hide the tracer if we need to.
                if (shouldKeepHidden)
                    tracerPrefab.transform.GetChild(0).gameObject.SetActive(false);

                // Add this tracer to the list of tracers.
                chestTracers.Add(tracerPrefab);
                created++;
            }

            Plugin.consoleLog?.LogDebug($"[ChestTracers] Tracers created: {created}, hidden={shouldKeepHidden}");

            void GetChests(Dictionary<string, Vector2> table)
            {
                if (table == null)
                {
                    Plugin.consoleLog?.LogWarning("[ChestTracers] Chest table is null for current stage.");
                    return;
                }

                // Validate session/locations
                if (Plugin.session == null || Plugin.session.Locations == null)
                {
                    Plugin.consoleLog?.LogWarning("[ChestTracers] Session or Locations is null. Skipping location checks.");
                    return;
                }

                var allChecked = Plugin.session.Locations.AllLocationsChecked;
                if (allChecked == null)
                {
                    Plugin.consoleLog?.LogWarning("[ChestTracers] AllLocationsChecked is null. Skipping location checks.");
                    return;
                }

                int added = 0;
                // Loop through each chest in the location table.
                foreach (KeyValuePair<string, Vector2> entry in table)
                {
                    if (string.IsNullOrEmpty(entry.Key))
                    {
                        Plugin.consoleLog?.LogWarning("[ChestTracers] Encountered chest entry with empty key.");
                        continue;
                    }

                    long locationIndex = -1;
                    try
                    {
                        locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", entry.Key);
                    }
                    catch (Exception ex)
                    {
                        Plugin.consoleLog?.LogWarning($"[ChestTracers] GetLocationIdFromName failed for '{entry.Key}': {ex.Message}");
                        continue;
                    }

                    bool exists = false;
                    try
                    {
                        exists = Helpers.CheckLocationExists(locationIndex);
                    }
                    catch (Exception ex)
                    {
                        Plugin.consoleLog?.LogWarning($"[ChestTracers] CheckLocationExists failed for index={locationIndex}: {ex.Message}");
                        continue;
                    }

                    if (exists && !allChecked.Contains(locationIndex))
                    {
                        locations.Add(entry.Value);
                        added++;
                    }
                }

                Plugin.consoleLog?.LogDebug($"[ChestTracers] GetChests added {added} pending chest locations for stageID={FPStage.currentStage.stageID}.");
            }
        }

        /// <summary>
        /// Handles toggling the Chest Tracer arrows on and off.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void UpdateChestTracers()
        {
            // Check for the F9 key or Select button (which is frustratingly mapped to pause by default).
            if (Input.GetKeyDown(KeyCode.F9) || Input.GetKeyDown("joystick 1 button 8"))
                foreach (GameObject tracer in chestTracers)
                    tracer.transform.GetChild(0).gameObject.SetActive(!tracer.transform.GetChild(0).gameObject.activeSelf);
        }

        /// <summary>
        /// Handles killing the player if a DeathLink comes in.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void ReceiveDeathLink()
        {
            // Check that the stage has finished loading and that we have a DeathLink waiting.
            // Also check that we don't have a PlayerShip or PlayerBFF2000 object, as we handle the DeathLink seperately in those.
            if (FPStage.objectsRegistered && hasBufferedDeathLink && PlayerShipPatcher.player == null && PlayerBFF2000Patcher.player == null)
            {
                // Turn off our can send flag so we don't send a DeathLink of our own.
                canSendDeathLink = false;

                // If the DeathLink slot value is just enable, then force run the player's crush action to blow them up.
                if ((long)Plugin.slotData["death_link"] == 1)
                    player.Action_Crush();

                // If the DeathLink slot value is enable_survive, then kill the player normally.
                if ((long)Plugin.slotData["death_link"] == 2)
                {
                    // Remove the player's invincibility, guard and health.
                    player.invincibilityTime = 0;
                    player.guardTime = 0;
                    player.health = 0;

                    // Damage the player.
                    player.Action_Hurt();
                }

                // Turn our buffered flag back off.
                hasBufferedDeathLink = false;
            }
        }

        /// <summary>
        /// Gives the player an amount of extra lives.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void ReceiveExtraLives()
        {
            // Check that the stage has finished loading and that we have a Powerup waiting.
            if (FPStage.objectsRegistered && hasBufferedExtraLives > 0)
            {
                // Loop through the amount of lives we have queued.
                for (int lifeIndex = 0; lifeIndex < hasBufferedExtraLives; lifeIndex++)
                {
                    // If we have less than 9, then give one.
                    if (player.lives < 9)
                        player.lives++;

                    // Create a +1 icon for this life.
                    // TODO: The spacing is weird, fiddle with that Y value calulcation more.
                    CrystalBonus crystalBonus = (CrystalBonus)FPStage.CreateStageObject(CrystalBonus.classID, 292f, -((lifeIndex + 1) * 64));
                    crystalBonus.animator.Play("HUD_Add");
                    crystalBonus.duration = 40f;
                }

                // Create the two stars coming off the player.
                InvincibilityStar invincibilityStar = (InvincibilityStar)FPStage.CreateStageObject(InvincibilityStar.classID, -100f, -100f);
                invincibilityStar.parentObject = player;
                invincibilityStar.distance = 320f;
                invincibilityStar.descend = true;
                InvincibilityStar invincibilityStar2 = (InvincibilityStar)FPStage.CreateStageObject(InvincibilityStar.classID, -100f, -100f);
                invincibilityStar2.parentObject = player;
                invincibilityStar2.rotation = 180f;
                invincibilityStar2.distance = 320f;
                invincibilityStar2.descend = true;

                // Play the Extra Life jingle.
                FPAudio.PlayJingle(3);

                // Reset our queued lives count.
                hasBufferedExtraLives = 0;
            }
        }

        /// <summary>
        /// Gives the player invincibility.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void ReceiveInvincibility()
        {
            // Check that the stage has finished loading and that we have an Invincibility waiting.
            if (FPStage.objectsRegistered && hasBufferedInvincibility)
            {
                // Set the Invincibility and Flash time values.
                player.invincibilityTime = Mathf.Max(player.invincibilityTime, 1200f);
                player.flashTime = Mathf.Max(player.flashTime, 1200f);

                // Create the two Invincibility stars.
                InvincibilityStar invincibilityStar = (InvincibilityStar)FPStage.CreateStageObject(InvincibilityStar.classID, -100f, -100f);
                invincibilityStar.parentObject = player;
                InvincibilityStar invincibilityStar2 = (InvincibilityStar)FPStage.CreateStageObject(InvincibilityStar.classID, -100f, -100f);
                invincibilityStar2.parentObject = player;
                invincibilityStar2.rotation = 180f;

                // Play the unused Invincibility jingle.
                FPAudio.PlayJingle(FPAudio.JINGLE_INVINCIBILITY);

                // Disable the Invincibility flag.
                hasBufferedInvincibility = false;
            }
        }

        /// <summary>
        /// Gives the player a shield.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void ReceiveShield()
        {
            // Check that the stage has finished loading and that we have a Shield waiting.
            if (FPStage.objectsRegistered && hasBufferedShield != FPItemBoxTypes.BOX_CRATE)
            {
                // Calculate how much health the shield should get.
                int shieldHealth = 2 + player.potions[5];
                if (player.IsPowerupActive(FPPowerup.STRONG_SHIELDS))
                    shieldHealth += 3;

                // Set the shield's health.
                player.shieldHealth = Mathf.Min(player.shieldHealth + shieldHealth, (int)player.healthMax * 2);

                // Spawn the orb for the shield.
                ShieldOrb shieldOrb = (ShieldOrb)FPStage.CreateStageObject(ShieldOrb.classID, player.position.x, player.position.y + 60f);
                shieldOrb.spawnLocation = player;
                shieldOrb.parentObject = player;

                // Set the shield type, play the correct animation for the orb and play the correct sound.
                switch (hasBufferedShield)
                {
                    case FPItemBoxTypes.BOX_WOODSHIELD:
                        shieldOrb.animator.Play("Wood", 0, 0f);
                        player.shieldID = 0;
                        FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("ShieldWood"));
                        break;
                    case FPItemBoxTypes.BOX_EARTHSHIELD:
                        shieldOrb.animator.Play("Earth", 0, 0f);
                        player.shieldID = 1;
                        FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("ShieldEarth"));
                        break;
                    case FPItemBoxTypes.BOX_WATERSHIELD:
                        shieldOrb.animator.Play("Water", 0, 0f);
                        player.shieldID = 2;
                        FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("ShieldWater"));
                        break;
                    case FPItemBoxTypes.BOX_FIRESHIELD:
                        shieldOrb.animator.Play("Fire", 0, 0f);
                        player.shieldID = 3;
                        FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("ShieldFire"));
                        break;
                    case FPItemBoxTypes.BOX_METALSHIELD:
                        shieldOrb.animator.Play("Metal", 0, 0f);
                        player.shieldID = 4;
                        FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("ShieldMetal"));
                        break;
                }

                // Create the shield flash effect.
                ShieldHit shieldHit = (ShieldHit)FPStage.CreateStageObject(ShieldHit.classID, player.position.x, player.position.y);
                shieldHit.SetParentObject(player);
                shieldHit.remainingDuration = 15f;

                // Reset the shield flag.
                hasBufferedShield = FPItemBoxTypes.BOX_CRATE;
            }
        }

        /// <summary>
        /// Gives the player their character's powerup.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void ReceivePowerup()
        {
            // Check that the stage has finished loading and that we have a Powerup waiting.
            if (FPStage.objectsRegistered && hasBufferedPowerup)
            {
                // Determine what to do based on the character ID.
                switch (player.characterID)
                {
                    // For the base game characters, just copy the behaviour from the actual game's ItemFuel class.
                    case FPCharacterID.LILAC:
                        player.powerupTimer = Mathf.Max(player.powerupTimer, 600f);
                        player.flashTime = Mathf.Max(player.flashTime, 600f);
                        FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("Enter"));
                        break;

                    case FPCharacterID.CAROL:
                        if (player.state == new FPObjectState(player.State_GrindRail))
                        {
                            player.barTimer = 0f;
                            player.Action_Jump();
                        }
                        player.Action_Carol_AddBike();
                        break;
                    case FPCharacterID.BIKECAROL:
                        player.invincibilityTime = Mathf.Max(player.invincibilityTime, 240f);
                        player.flashTime = Mathf.Max(player.flashTime, 240f);
                        FPAudio.PlaySfx(16);
                        break;

                    case FPCharacterID.MILLA:
                        player.Action_MillaMultiCube();
                        FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("Enter"));
                        break;

                    case FPCharacterID.NEERA:
                        player.powerupTimer = Mathf.Max(player.powerupTimer, 600f);
                        player.flashTime = Mathf.Max(player.flashTime, 600f);
                        player.Action_SpeedShoes(1.5f);
                        FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("Enter"));
                        break;

                    // For modded characters, run their ItemFuelPickup action defined through FP2Lib (assuming the PlayerHandler has a character loaded).
                    default: PlayerHandler.currentCharacter?.ItemFuelPickup(); break;
                }

                // Disable the Powerup flag.
                hasBufferedPowerup = false;
            }
        }

        /// <summary>
        /// Resets the flag for being able to send DeathLinks upon reviving.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_KO_Recover")]
        static void KORecover() => canSendDeathLink = true;

        /// <summary>
        /// Calls the SendDeathLink function depending on the player state.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_KO")]
        static void KOed()
        {
            if (player.oxygenLevel <= 0)
                SendDeathLink($"{Helpers.GetPlayer()} forgot to breathe. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);
            else if (player.heatLevel >= 1)
                SendDeathLink($"{Helpers.GetPlayer()} was baked. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);
            else
                SendDeathLink($"{Helpers.GetPlayer()} got slapped. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Action_Crush")]
        static void Crush()
        {
            if (SceneManager.GetActiveScene().name != "Bakunawa5")
                SendDeathLink($"{Helpers.GetPlayer()} became a pancake. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);
            else
                SendDeathLink($"{Helpers.GetPlayer()} got skewered. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_CrushKO")]
        static void Fall() => SendDeathLink($"{Helpers.GetPlayer()} fell in a hole. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", true);
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_FallKO")]
        static void RingOut()
        {
            if (SceneManager.GetActiveScene().name == "Battlesphere_RingOut")
                SendDeathLink($"{Helpers.GetPlayer()} fell in a hole. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_Defeat")]
        static void RaceLost() => SendDeathLink($"{Helpers.GetPlayer()} was too slow. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);

        /// <summary>
        /// Sends a DeathLink.
        /// <paramref name="reason">The reason shown to other clients.</paramref>
        /// <paramref name="checkHealth">Whether or not this DeathLink should only activate at 0 health.</paramref>/>
        /// </summary>
        public static void SendDeathLink(string reason, bool checkHealth)
        {
            // If DeathLink is disabled, then don't run any of this code.
            if ((long)Plugin.slotData["death_link"] == 0)
                return;

            // Check if we can actually send a DeathLink.
            if (canSendDeathLink)
            {
                // Check if this DeathLink relies on the player's heatlh status and that they have health. If so, then don't send out a Deathlink.
                if (checkHealth && player.health >= 0f)
                    return;

                // If this death was caused by Milla getting crushed, then swap the message out to reference her post-Robot Graveyard line to Askal.
                if (reason == $"Milla became a pancake. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]")
                    reason = $"{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)} made a Milla sandwich.";

                // Add a message to our sent queue so it'll take priority for the message label.
                Plugin.sentMessageQueue.Add("Sending death to your friends!");

                // Send a DeathLink.
                Plugin.DeathLink.SendDeathLink(new Archipelago.MultiClient.Net.BounceFeatures.DeathLink.DeathLink(Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot), reason));

                // Print the DeathLink send to the console too, as a debug log.
                Plugin.consoleLog.LogDebug($"Sending DeathLink with reason:\r\n\t{reason}");

                // Set the flag to avoid sending extras.
                canSendDeathLink = false;
            }
        }

        /// <summary>
        /// Removes a few lines in State_ItemGet.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_ItemGet")]
        static IEnumerable<CodeInstruction> RemoveItemGetSound(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Remove the FPAudio.PlaySFX call that plays the item get sound, as its also used for the label which spawns immediately after a chest open anyway.
            codes[53].opcode = OpCodes.Nop;
            codes[54].opcode = OpCodes.Nop;

            // Remove the direction forcing.
            codes[55].opcode = OpCodes.Nop;
            codes[56].opcode = OpCodes.Nop;
            codes[57].opcode = OpCodes.Nop;

            // Remove the if statement that causes the voice line to play.
            for (int codeIndex = 110; codeIndex <= 126; codeIndex++)
                codes[codeIndex].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Edits a few of the player's values when getting a trap item from a chest.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "State_ItemGet")]
        static void GetTrapItemEdit()
        {
            // If we don't have an override animator set (because we're a custom character or this chest isn't a trap), then restore some of the transpiles and abort.
            if (overrideAnimator == null)
            {
                if (player.genericTimer > 30f && player.currentAnimation != "ItemGet")
                    player.direction = FPDirection.FACING_RIGHT;

                if (player.genericTimer > 60f && player.genericTimer < 70f)
                {
                    player.genericTimer += 10f;
                    player.Action_PlayVoiceArray("ItemGet");
                }

                return;
            }

            // Check the timer and animation like in the original State_ItemGet.
            if (player.genericTimer > 30f && player.currentAnimation != "ItemGet")
            {
                // If we don't have a stored animator, then store our current one.
                if (storedAnimator == null)
                    storedAnimator = player.animator.runtimeAnimatorController;

                // Check if we haven't already stored the voice array.
                if (storedItemVoices == null)
                {
                    // Store the voice array.
                    storedItemVoices = player.vaItemGet;

                    // Swap the voice array for the one from the asset bundle.
                    // We skip Carol's bike state, as we don't replace her animation, thanks to her lacking a suitable shocked one (might use her look up animation?)
                    switch (player.characterID)
                    {
                        case FPCharacterID.LILAC: player.vaItemGet = Plugin.LilacTrapSounds; break;
                        case FPCharacterID.CAROL: player.vaItemGet = Plugin.CarolTrapSounds; break;
                        case FPCharacterID.MILLA: player.vaItemGet = Plugin.MillaTrapSounds; break;
                        case FPCharacterID.NEERA: player.vaItemGet = Plugin.NeeraTrapSounds; break;
                    }

                    // Play a sound from our new voice array (rather than delaying it).
                    player.Action_PlayVoiceArray("ItemGet");
                }

                // Replace the animator with our override one.
                player.animator.runtimeAnimatorController = overrideAnimator;
            }
        }

        /// <summary>
        /// Reverts the edit so that we actually have animations again.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void RevertTrapItemEdit()
        {
            // Check if we have an override and stored animator set, and that we're not in the ItemGet animation anymore.
            if (overrideAnimator != null && storedAnimator != null && player.currentAnimation != "ItemGet")
            {
                // Undo the edits. 
                player.animator.runtimeAnimatorController = storedAnimator;
                player.vaItemGet = storedItemVoices;

                // Reset the values to null.
                overrideAnimator = null;
                storedAnimator = null;
                storedItemVoices = null;
            }
        }

        /// <summary>
        /// Handles swapping the player out if a Swap Trap comes in.
        /// </summary>
        public static void SwapTrap()
        {
            // If the player is Carol with her bike, then swap her to regular Carol to simplify the index check.
            if (player.characterID == FPCharacterID.BIKECAROL)
                player.characterID = FPCharacterID.CAROL;

            // Set the index to our character ID.
            int index = (int)player.characterID;

            // Reroll the index if it landed on our current ID. We currently only select the base game characters as custom ones can do some funky shit that breaks everything.
            while (index == (int)player.characterID)
            {
                index = Plugin.rng.Next(0, 4);
                //index = Plugin.rng.Next(MenuConnection.characters.Count - 1);
                index = MenuConnection.characters.ElementAt(index).Value;
            }

            // Set up a variable to hold this character's prefab.
            GameObject prefab = null;

            // Select the prefab based on the index.
            switch (index)
            {
                case 0: prefab = Plugin.playerPrefabs[0]; break; // lilac
                case 1: prefab = Plugin.playerPrefabs[3]; break; // carol
                case 3: prefab = Plugin.playerPrefabs[2]; break; // milla
                case 4: prefab = Plugin.playerPrefabs[1]; break; // neera
                //default:
                //    foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
                //    {
                //        if (chara.id == index)
                //        {
                //            prefab = chara.prefab;
                //        }
                //    }
                //    break; // custom
            }

            // Bail out if we didn't find a prefab.
            if (prefab == null)
                return;

            // Set the player character's ID.
            player.characterID = (FPCharacterID)index;
            PlayerHandler.currentCharacter = PlayerHandler.GetPlayableCharaByFPCharacterId((FPCharacterID)index);

            // Load the FPPlayer script from the prefab, if only so we aren't spamming this line all over the place.
            FPPlayer playerPrefabScript = prefab.GetComponent<FPPlayer>();

            // Replace our animator with the prefab's.
            player.animator.runtimeAnimatorController = prefab.GetComponent<Animator>().runtimeAnimatorController;

            // Replace our energy recover rates with our prefab's.
            player.energyRecoverRate = playerPrefabScript.energyRecoverRate;
            player.energyRecoverRateCurrent = playerPrefabScript.energyRecoverRate;

            // If we're switching from a character that has a tail and aren't switching to Neera, then replace the tail's animator as well.
            if (player.childSprite != null && playerPrefabScript.childSprite != null)
                player.childAnimator.runtimeAnimatorController = playerPrefabScript.childSprite.GetComponent<Animator>().runtimeAnimatorController;

            // Replace our debris with the prefab's.
            player.debrisColor = playerPrefabScript.debrisColor;
            player.debrisSprites = playerPrefabScript.debrisSprites;

            // Replace all of our sounds with the prefab's.
            player.sfxJump = playerPrefabScript.sfxJump;
            player.sfxDoubleJump = playerPrefabScript.sfxDoubleJump;
            player.sfxSkid = playerPrefabScript.sfxSkid;
            player.sfxRegen = playerPrefabScript.sfxRegen;
            player.sfxLilacBlink = playerPrefabScript.sfxLilacBlink;
            player.sfxUppercut = playerPrefabScript.sfxUppercut;
            player.sfxBoostCharge = playerPrefabScript.sfxBoostCharge;
            player.sfxBoostLaunch = playerPrefabScript.sfxBoostLaunch;
            player.sfxBigBoostLaunch = playerPrefabScript.sfxBigBoostLaunch;
            player.sfxBoostRebound = playerPrefabScript.sfxBoostRebound;
            player.sfxBoostExplosion = playerPrefabScript.sfxBoostExplosion;
            player.sfxDivekick1 = playerPrefabScript.sfxDivekick1;
            player.sfxDivekick2 = playerPrefabScript.sfxDivekick2;
            player.sfxCyclone = playerPrefabScript.sfxCyclone;
            player.sfxCarolAttack1 = playerPrefabScript.sfxCarolAttack1;
            player.sfxCarolAttack2 = playerPrefabScript.sfxCarolAttack2;
            player.sfxCarolAttack3 = playerPrefabScript.sfxCarolAttack3;
            player.sfxPounce = playerPrefabScript.sfxPounce;
            player.sfxWallCling = playerPrefabScript.sfxWallCling;
            player.sfxMillaShieldSummon = playerPrefabScript.sfxMillaShieldSummon;
            player.sfxMillaShieldFire = playerPrefabScript.sfxMillaShieldFire;
            player.sfxMillaCubeSpawn = playerPrefabScript.sfxMillaCubeSpawn;
            player.sfxRolling = playerPrefabScript.sfxRolling;
            player.vaAttack = playerPrefabScript.vaAttack;
            player.vaHardAttack = playerPrefabScript.vaHardAttack;
            player.vaSpecialA = playerPrefabScript.vaSpecialA;
            player.vaSpecialB = playerPrefabScript.vaSpecialB;
            player.vaHit = playerPrefabScript.vaHit;
            player.vaKO = playerPrefabScript.vaKO;
            player.vaIdle = playerPrefabScript.vaIdle;
            player.vaRevive = playerPrefabScript.vaRevive;
            player.vaStart = playerPrefabScript.vaStart;
            player.vaItemGet = playerPrefabScript.vaItemGet;
            player.vaClear = playerPrefabScript.vaClear;
            player.vaJackpotClear = playerPrefabScript.vaJackpotClear;
            player.vaLowDamageClear = playerPrefabScript.vaLowDamageClear;
            player.vaExtra = playerPrefabScript.vaExtra;
            player.sfxIdle = playerPrefabScript.sfxIdle;
            player.sfxMove = playerPrefabScript.sfxMove;
            player.bgmResults = playerPrefabScript.bgmResults;

            // Replace our physics stats with the prefab's.
            player.topSpeed = playerPrefabScript.topSpeed;
            player.acceleration = playerPrefabScript.acceleration;
            player.deceleration = playerPrefabScript.deceleration;
            player.airAceleration = playerPrefabScript.airAceleration;
            player.skidDeceleration = playerPrefabScript.skidDeceleration;
            player.gravityStrength = playerPrefabScript.gravityStrength;
            player.jumpStrength = playerPrefabScript.jumpStrength;
            player.jumpRelease = playerPrefabScript.jumpRelease;
            player.climbingSpeed = playerPrefabScript.climbingSpeed;
            player.fightStanceTime = playerPrefabScript.fightStanceTime;
            player.idlePoses = playerPrefabScript.idlePoses;

            // Replace our swap values with the prefab's.
            player.swapAcceleration = playerPrefabScript.swapAcceleration;
            player.swapAirAceleration = playerPrefabScript.swapAirAceleration;
            player.swapAnimator = playerPrefabScript.swapAnimator;
            player.swapChildSprite = playerPrefabScript.swapChildSprite;
            player.swapCharacterID = playerPrefabScript.swapCharacterID;
            player.swapClimbingSpeed = playerPrefabScript.swapClimbingSpeed;
            player.swapDeceleration = playerPrefabScript.swapDeceleration;
            player.swapEnergyRecoverRate = playerPrefabScript.swapEnergyRecoverRate;
            player.swapGravityStrength = playerPrefabScript.swapGravityStrength;
            player.swapJumpRelease = playerPrefabScript.swapJumpRelease;
            player.swapJumpStrength = playerPrefabScript.swapJumpStrength;
            player.swapSfxIdle = playerPrefabScript.swapSfxIdle;
            player.swapSfxJump = playerPrefabScript.swapSfxJump;
            player.swapSfxMove = playerPrefabScript.swapSfxMove;
            player.swapSfxSkid = playerPrefabScript.swapSfxSkid;
            player.swapSkidDeceleration = playerPrefabScript.swapSkidDeceleration;
            player.swapSkidThreshold = playerPrefabScript.swapSkidThreshold;
            player.swapTopSpeed = playerPrefabScript.swapTopSpeed;
            player.hasSwapCharacter = playerPrefabScript.hasSwapCharacter;

            // Check if we don't have a tail but our prefab does.
            if (player.childSprite == null && playerPrefabScript.childSprite != null)
            {
                // Instantiate the child sprite prefab.
                player.childSprite = UnityEngine.Object.Instantiate(playerPrefabScript.childSprite);

                // Set the child sprite's child sprite to its sprite renderer.
                player.childSprite.childSprite = player.childSprite.GetComponent<SpriteRenderer>();

                // Parent this child sprite to the player.
                player.childSprite.parentObject = player;

                //Set the child sprite's position and transform parent approriately.
                player.childSprite.transform.position = new Vector3(player.transform.position.x + player.childSprite.xOffset, player.transform.position.y + player.childSprite.yOffset, player.transform.position.z + player.childSprite.zOffset);
                player.childSprite.transform.parent = player.transform;

                // Set the child sprite's layer to the same as the player.
                player.childSprite.gameObject.layer = player.gameObject.layer;

                // Get the sprite renderer and animator for the child sprite.
                player.childRender = player.childSprite.GetComponent<SpriteRenderer>();
                player.childAnimator = player.childSprite.GetComponent<Animator>();

                // Activate the child sprite.
                player.childSprite.gameObject.SetActive(true);
            }

            // If we're swapping to Carol, then set up the swap child sprite too.
            if (player.swapChildSprite != null)
            {
                // Instantiate the swap child sprite from the player prefab.
                player.swapChildSprite = UnityEngine.Object.Instantiate(playerPrefabScript.swapChildSprite);

                // Set the swap child sprite's child sprite to its sprite renderer.
                player.swapChildSprite.childSprite = player.swapChildSprite.GetComponent<SpriteRenderer>();

                // Parent this swap child sprite to the player.
                player.swapChildSprite.parentObject = player;

                //Set the swap child sprite's position and transform parent approriately.
                player.swapChildSprite.transform.position = new Vector3(player.transform.position.x + player.swapChildSprite.xOffset, player.transform.position.y + player.swapChildSprite.yOffset, player.transform.position.z + player.swapChildSprite.zOffset);
                player.swapChildSprite.transform.parent = player.transform;

                // Set the swap child sprite's layer to the same as the player.
                player.swapChildSprite.gameObject.layer = player.gameObject.layer;

                // Disable the swap child sprite's renderer if they're Carol's bike state.
                if (player.hasSwapCharacter && player.swapCharacterID == FPCharacterID.BIKECAROL)
                    player.swapChildSprite.GetComponent<SpriteRenderer>().enabled = false;

                // Append (swap) to the child sprite's object name.
                player.swapChildSprite.gameObject.name = player.swapChildSprite.gameObject.name + " (swap)";

                // Deactivate the swap child sprite.
                player.swapChildSprite.gameObject.SetActive(false);
            }

            // Stop all the player's audio so that Carol's bike sound doesn't keep playing.
            foreach (AudioSource audioChannel in player.audioChannel)
                audioChannel.Stop();

            // Shake the camera a bit.
            FPCamera.stageCamera.screenShake = Mathf.Max(FPCamera.stageCamera.screenShake, 10f);

            // Loop through and create 4 sparks.
            for (int sparkIndex = 0; sparkIndex < 4; sparkIndex++)
            {
                Spark spark = (Spark)FPStage.CreateStageObject(Spark.classID, player.position.x, player.position.y);
                spark.velocity.x = Mathf.Cos((float)Math.PI / 180f * ((float)sparkIndex * 90f + 45f)) * 20f;
                spark.velocity.y = Mathf.Sin((float)Math.PI / 180f * ((float)sparkIndex * 90f + 45f)) * 20f;
                spark.SetAngle();
            }

            // Create the Boost Breaker explosion.
            BoostExplosion boostExplosion = (BoostExplosion)FPStage.CreateStageObject(BoostExplosion.classID, player.position.x, player.position.y);
            boostExplosion.attackKnockback.x = player.attackKnockback.x * 0.5f;
            boostExplosion.attackKnockback.y = player.attackKnockback.y * 0.5f;
            boostExplosion.attackEnemyInvTime = player.attackEnemyInvTime;
            boostExplosion.parentObject = player;
            boostExplosion.faction = player.faction;

            // Play the Boost Breaker sound from Lilac's prefab.
            player.Action_PlaySoundUninterruptable(Plugin.playerPrefabs[0].GetComponent<FPPlayer>().sfxBoostExplosion);

            // Check if the player is now Carol.
            if (player.characterID == FPCharacterID.CAROL)
            {
                // Create a new Audio Channel array for the player.
                player.audioChannel = new AudioSource[6];

                // Loop through and set up the six audio sources.
                for (int audioIndex = 0; audioIndex < 6; audioIndex++)
                {
                    // Create a new game object with the name "PlayerAudioSource".
                    GameObject gameObject = new("PlayerAudioSource");

                    // Parent this audio source to the player.
                    gameObject.transform.parent = player.gameObject.transform;

                    // Add an audio source to this slot in the array.
                    player.audioChannel[audioIndex] = gameObject.AddComponent<AudioSource>();

                    // Set this slot in the array's volume to our saved sound volume.
                    player.audioChannel[audioIndex].volume = FPSaveManager.volumeSfx;

                    // Set this slot's play on awake value to false.
                    player.audioChannel[audioIndex].playOnAwake = false;
                }

                // Set up the volume on audio channel 0 to our saved voice volume.
                player.audioChannel[0].volume = FPSaveManager.volumeVoices;

                // Set the 4th and 5th slots to Carol's bike sounds.
                player.audioChannel[4].clip = player.sfxIdle;
                player.audioChannel[5].clip = player.sfxMove;
            }

            // If we've swapped to Neera and have a child sprite, then destroy it.
            if (playerPrefabScript.childSprite == null && player.childSprite != null)
                GameObject.Destroy(player.childSprite.gameObject);

            // Set the player into their guard state and animation.
            player.SetPlayerAnimation("GuardAir", null, null, true);
            player.Action_Guard();

            // TODO: Figure out a way to stop an active attack hitbox from lingering here.
        }

        /// <summary>
        /// Handles flipping left and right controls when a Mirror Trap is active.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "ProcessInputControl")]
        static bool MirrorTrapControls_NoRewired()
        {
            // If the Mirror Trap Timer isn't going, then simply run the original function instead.
            if (Plugin.MirrorTrapTimer <= 0f)
                return true;

            // Get the axis input from the controller.
            float xAxis = InputControl.GetAxis(Controls.axes.horizontal);
            float yAxis = InputControl.GetAxis(Controls.axes.vertical);

            // Reset the directional presses.
            player.input.upPress = false;
            player.input.downPress = false;
            player.input.leftPress = false;
            player.input.rightPress = false;

            // Check if the player is pressing left or not and set the flags for pressing right accordingly.
            if (xAxis < 0f - InputControl.joystickThreshold)
            {
                if (!player.input.right)
                    player.input.rightPress = true;

                player.input.right = true;
            }
            else
                player.input.right = false;

            // Check if the player is pressing right or not and set the flags for pressing left accordingly.
            if (xAxis > InputControl.joystickThreshold)
            {
                if (!player.input.left)
                    player.input.leftPress = true;

                player.input.left = true;
            }
            else
                player.input.left = false;

            // Check if the player is pressing up or not and set the flags accordingly.
            if (yAxis > InputControl.joystickThreshold)
            {
                if (!player.input.up)
                    player.input.upPress = true;

                player.input.up = true;
            }
            else
                player.input.up = false;

            // Check if the player is pressing down or not and set the flags accordingly.
            if (yAxis < 0f - InputControl.joystickThreshold)
            {
                if (!player.input.down)
                    player.input.downPress = true;

                player.input.down = true;
            }
            else
                player.input.down = false;

            // Check if the player is pressing the face buttons and set the flags accordingly.
            player.input.jumpPress = InputControl.GetButtonDown(Controls.buttons.jump);
            player.input.jumpHold = InputControl.GetButton(Controls.buttons.jump);
            player.input.attackPress = InputControl.GetButtonDown(Controls.buttons.attack);
            player.input.attackHold = InputControl.GetButton(Controls.buttons.attack);
            player.input.specialPress = InputControl.GetButtonDown(Controls.buttons.special);
            player.input.specialHold = InputControl.GetButton(Controls.buttons.special);
            player.input.guardPress = InputControl.GetButtonDown(Controls.buttons.guard);
            player.input.guardHold = InputControl.GetButton(Controls.buttons.guard);
            player.input.confirm = player.input.jumpPress | InputControl.GetButtonDown(Controls.buttons.pause);
            player.input.cancel = player.input.attackPress | Input.GetKey(KeyCode.Escape);

            // Stop the original function from running.
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "ProcessRewired")]
        static bool MirrorTrapControls_Rewired(ref Player ___rewiredPlayerInput)
        {
            // If the Mirror Trap Timer isn't going, then simply run the original function instead.
            if (Plugin.MirrorTrapTimer <= 0f)
                return true;

            // Reset the directional presses.
            player.input.upPress = false;
            player.input.downPress = false;
            player.input.leftPress = false;
            player.input.rightPress = false;

            // Check if the player is pressing left or not and set the flags for pressing right accordingly.
            if (___rewiredPlayerInput.GetButton("Left"))
            {
                if (!player.input.right)
                    player.input.rightPress = true;

                player.input.right = true;
            }
            else
                player.input.right = false;

            // Check if the player is pressing right or not and set the flags for pressing left accordingly.
            if (___rewiredPlayerInput.GetButton("Right"))
            {
                if (!player.input.left)
                    player.input.leftPress = true;

                player.input.left = true;
            }
            else
                player.input.left = false;

            // Check if the player is pressing up or not and set the flags accordingly.
            if (___rewiredPlayerInput.GetButton("Up"))
            {
                if (!player.input.up)
                    player.input.upPress = true;

                player.input.up = true;
            }
            else
                player.input.up = false;

            // Check if the player is pressing down or not and set the flags accordingly.
            if (___rewiredPlayerInput.GetButton("Down"))
            {
                if (!player.input.down)
                    player.input.downPress = true;

                player.input.down = true;
            }
            else
                player.input.down = false;

            // Check if the player is pressing the face buttons and set the flags accordingly.
            player.input.jumpPress = ___rewiredPlayerInput.GetButtonDown("Jump");
            player.input.jumpHold = ___rewiredPlayerInput.GetButton("Jump");
            player.input.attackPress = ___rewiredPlayerInput.GetButtonDown("Attack");
            player.input.attackHold = ___rewiredPlayerInput.GetButton("Attack");
            player.input.specialPress = ___rewiredPlayerInput.GetButtonDown("Special");
            player.input.specialHold = ___rewiredPlayerInput.GetButton("Special");
            player.input.guardPress = ___rewiredPlayerInput.GetButtonDown("Guard");
            player.input.guardHold = ___rewiredPlayerInput.GetButton("Guard");
            player.input.confirm = player.input.jumpPress | InputControl.GetButtonDown(Controls.buttons.pause);
            player.input.cancel = player.input.attackPress | Input.GetKey(KeyCode.Escape);

            // Stop the original function from running.
            return false;
        }

        /// <summary>
        /// Updates the player position and facing direction on the data storage.
        /// This and RemotePlayerAnimation are the functions that cause the horrendous lag I believe.
        /// TODO: Try and understand how to potentially use the IEnumerator and Yield stuff to make updating the DataStorage here asynchronous.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update")]
        static void RemotePlayerPosition()
        {
            // Check if our position has actually changed so we don't update the data storage for no reason.
            if (player.prevPosition != player.position && Plugin.configRemotePlayers.Value == true)
            {
                // Get our entry from the data storage.
                JObject playerJObject = Plugin.session.DataStorage[$"FP2_PlayerSlot{Plugin.session.ConnectionInfo.Slot}"].To<JObject>();

                // Update our position and facing direction.
                playerJObject["PositionX"] = player.position.x;
                playerJObject["PositionY"] = player.position.y;
                playerJObject["Facing"] = (int)player.direction;

                // Push the updated entry to the data storage.
                Plugin.session.DataStorage[$"FP2_PlayerSlot{Plugin.session.ConnectionInfo.Slot}"] = playerJObject;
            }
        }

        /// <summary>
        /// Updates the player animation on the data storage.
        /// This and RemotePlayerPosition are the functions that cause the horrendous lag I believe.
        /// TODO: Try and understand how to potentially use the IEnumerator and Yield stuff to make updating the DataStorage here asynchronous.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPPlayer), "SetPlayerAnimation")]
        static void RemotePlayerAnimation(ref bool skipNameCheck, ref string aniName)
        {
            // Only update the value if the conditions to actually change the animation are valid.
            if ((!skipNameCheck && !(player.currentAnimation != aniName)) || Plugin.configRemotePlayers.Value == false)
                return;

            // Get our entry from the data storage.
            JObject playerJObject = Plugin.session.DataStorage[$"FP2_PlayerSlot{Plugin.session.ConnectionInfo.Slot}"].To<JObject>();

            // Update our animation.
            playerJObject["Animation"] = aniName;

            // Push the updated entry to the data storage.
            Plugin.session.DataStorage[$"FP2_PlayerSlot{Plugin.session.ConnectionInfo.Slot}"] = playerJObject;
        }
    }
}
