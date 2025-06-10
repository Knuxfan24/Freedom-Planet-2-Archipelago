global using Archipelago.MultiClient.Net;
global using BepInEx;
global using Freedom_Planet_2_Archipelago.CustomData;
global using HarmonyLib;
global using System;
global using System.Collections.Generic;
global using UnityEngine;

using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using BepInEx.Configuration;
using BepInEx.Logging;
using Freedom_Planet_2_Archipelago.Patchers;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago
{
    [BepInPlugin("K24_FP2_Archipelago", "Archipelago", "0.0.1")]
    [BepInDependency("000.kuborro.libraries.fp2.fp2lib")]
    public class Plugin : BaseUnityPlugin
    {
        // The asset bundle exported from the Unity project.
        public static AssetBundle apAssetBundle;

        // Logger.
        public static ManualLogSource consoleLog;

        // The config options.
        public static ConfigEntry<string> configServerAddress;
        public static ConfigEntry<string> configSlotName;
        public static ConfigEntry<string> configPassword;
        public static ConfigEntry<int> configCharacter;

        // The AP session's data.
        public static ArchipelagoSession session;
        public static Dictionary<string, object> slotData;
        public static DeathLinkService DeathLink;

        // The AP's save.
        public static ArchipelagoSave save;

        // The item queue and its timer.
        public static Dictionary<ArchipelagoItem, int> itemQueue = [];
        public static float itemQueueTimer = -1;

        // The message banner object.
        public static GameObject messageBanner;

        // The list of "Sent [x] to [y]" messages.
        public static List<string> sentMessageQueue = [];

        // Random character selector
        public static System.Random rng = new(); // Also used by the Swap Trap.
        public static bool usingRandomCharacter = false;

        // Trap based values.
        public static List<GameObject> playerPrefabs = []; // Only used by the Swap Trap, so we'll place it under this set.
        public static float MirrorTrapTimer = -1;

        private void Awake()
        {
            // Set up the logger.
            consoleLog = Logger;

            // Check for the asset bundle.
            if (!File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\archipelago.assets"))
            {
                consoleLog.LogError("Failed to find the archipelago.assets file! Please ensure it is correctly located in your Freedom Planet 2's mod_overrides\\Archipelago folder.");
                return;
            }

            // Create the Archipelago Saves directory if it doesn't exist.
            if (!Directory.Exists($@"{Paths.GameRootPath}\Archipelago Saves"))
                Directory.CreateDirectory($@"{Paths.GameRootPath}\Archipelago Saves");

            // Get the player prefabs from the game itself.
            foreach (GameObject obj in UnityEngine.Resources.FindObjectsOfTypeAll<GameObject>())
                if (obj.name is "Player Lilac" or "Player Carol" or "Player Milla" or "Player Neera") playerPrefabs.Add(obj);

            // Get the config options.
            configServerAddress = Config.Bind("Connection",
                                              "Server Address",
                                              "archipelago.gg:",
                                              "The server address that was last connected to.");

            configSlotName = Config.Bind("Connection",
                                         "Slot Name",
                                         "Freedom Planet 2",
                                         "The name of the last slot that was connected to.");

            configPassword = Config.Bind("Connection",
                                         "Password",
                                         "",
                                         "The password that was used for the last session connected to.");

            configCharacter = Config.Bind("Connection",
                                          "Character Index",
                                          0,
                                          "The index of the character that was selected for the last connection.");

            // Load our asset bundle.
            apAssetBundle = AssetBundle.LoadFromFile($@"{Paths.GameRootPath}\mod_overrides\Archipelago\archipelago.assets");
            
            // If we're in Debug Mode, then print all the asset names from the asset bundle.
            #if DEBUG
            foreach (string assetName in apAssetBundle.GetAllAssetNames())
                consoleLog.LogInfo(assetName);
            #endif
            
            // Create the message banner object.
            messageBanner = GameObject.Instantiate(apAssetBundle.LoadAsset<GameObject>("Message Label"));
            messageBanner.AddComponent<MessageBanner>();
            DontDestroyOnLoad(messageBanner);

            // Patch all the functions that need patching.
            Harmony.CreateAndPatchAll(typeof(AcrabellePieTrapPatcher));
            Harmony.CreateAndPatchAll(typeof(EnemySanity));
            Harmony.CreateAndPatchAll(typeof(FPCameraPatcher));
            Harmony.CreateAndPatchAll(typeof(FPHudMasterPatcher));
            Harmony.CreateAndPatchAll(typeof(FPPlayerPatcher));
            Harmony.CreateAndPatchAll(typeof(FPResultsMenuPatcher));
            Harmony.CreateAndPatchAll(typeof(FPSaveManagerPatcher));
            Harmony.CreateAndPatchAll(typeof(FPStagePatcher));
            Harmony.CreateAndPatchAll(typeof(ItemChestPatcher));
            Harmony.CreateAndPatchAll(typeof(ItemStarCardPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuArenaChallengeSelectPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuClassicPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuClassicShopHubPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuCreditsPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuGlobalPausePatcher));
            Harmony.CreateAndPatchAll(typeof(MenuItemGetPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuItemSelectPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuJukeboxPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuShopPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuSpawnerPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuTitleScreenPatcher));
            Harmony.CreateAndPatchAll(typeof(MenuWorldMapConfirmPatcher));
            Harmony.CreateAndPatchAll(typeof(PlayerBossMergaPatcher));
        }

        private void Update()
        {
            // Kick the player out to the Arena Menu if they're in the Time Capsule or Tournament Scenes.
            if (SceneManager.GetActiveScene().name == "Cutscene_BattlesphereCapsule" || SceneManager.GetActiveScene().name == "ArenaChallengeMenu")
                SceneManager.LoadScene("ArenaMenu");

            // Check that the item queue timer isn't currently frozen.
            if (itemQueueTimer != -1)
            {
                // Increment the queue timer by the game's delta timer.
                itemQueueTimer += Time.deltaTime;

                // Check if the timer has reached 0.25.
                if (itemQueueTimer >= 0.25f)
                {
                    // Check that the banner is in its idle state.
                    if (messageBanner.GetComponent<MessageBanner>().state == new FPObjectState(messageBanner.GetComponent<MessageBanner>().State_Idle))
                    {
                        // Check if we have any sent messages, as these should take priority.
                        if (sentMessageQueue.Count != 0)
                        {
                            // Set the banner to the expand state and pass our message to it.
                            messageBanner.GetComponent<MessageBanner>().state = messageBanner.GetComponent<MessageBanner>().State_Expand;
                            messageBanner.GetComponent<MessageBanner>().text = sentMessageQueue[0];

                            // Remove this message from the queue.
                            sentMessageQueue.RemoveAt(0);

                            // Play the item get sound.
                            FPAudio.PlaySfx(FPAudio.SFX_ITEMGET);
                        }

                        // If we don't have any sent messages, then check for any items we are waiting to receive.
                        else if (itemQueue.Count != 0)
                        {
                            // Get the first item in our queue.
                            KeyValuePair<ArchipelagoItem, int> item = itemQueue.ElementAt(0);

                            // Actually receive the item.
                            Helpers.HandleItem(item);

                            // Remove this item from the queue.
                            itemQueue.Remove(item.Key);

                            // Set up a message to display, depending on various factors.
                            string message = $"Recieved {item.Key.ItemName} from {item.Key.Source}.";
                            if (item.Key.Source == session.Players.GetPlayerName(session.ConnectionInfo.Slot)) message = $"Found your {item.Key.ItemName}.";
                            if (item.Value > 1) message = $"Recieved {item.Value} {item.Key.ItemName}s from {item.Key.Source}.";
                            if (item.Key.Source == session.Players.GetPlayerName(session.ConnectionInfo.Slot) && item.Value > 1) message = $"Found {item.Value} of your {item.Key.ItemName}s.";

                            // Set the banner to the expand state and pass our message to it.
                            messageBanner.GetComponent<MessageBanner>().state = messageBanner.GetComponent<MessageBanner>().State_Expand;
                            messageBanner.GetComponent<MessageBanner>().text = message;

                            // Play the item get sound.
                            FPAudio.PlaySfx(FPAudio.SFX_ITEMGET);
                        }
                    }

                    // Remove 0.25 from the queue timer.
                    itemQueueTimer -= 0.25f;
                }
            }
        
            // Decrement the Mirror Trap's timer if it's higher than 0 and a player exists.
            if (MirrorTrapTimer > 0 && FPPlayerPatcher.player != null)
                MirrorTrapTimer -= Time.deltaTime;
        }
    }
}
