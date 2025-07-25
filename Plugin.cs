﻿// TODO: Check if the BFF2000 needs a special DeathLink too.
// TODO: Release "Found [x]'s [y]" messages.
global using Archipelago.MultiClient.Net;
global using BepInEx;
global using Freedom_Planet_2_Archipelago.CustomData;
global using HarmonyLib;
global using System;
global using System.Collections.Generic;
global using UnityEngine;

using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Packets;
using BepInEx.Configuration;
using BepInEx.Logging;
using Freedom_Planet_2_Archipelago.Patchers;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago
{
    [BepInPlugin("K24_FP2_Archipelago", "Archipelago", "0.1.0")]
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
        public static ConfigEntry<long> configDeathLinkOverride;
        public static ConfigEntry<long> configRingLinkOverride;
        public static ConfigEntry<long> configTrapLinkOverride;

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

        // Random character selector.
        public static System.Random rng = new(); // Also used by the Swap Trap.
        public static bool usingRandomCharacter = false;

        // Trap based values.
        public static List<GameObject> playerPrefabs = []; // Only used by the Swap Trap, so we'll place it under this set.
        public static float MirrorTrapTimer = -1;
        public static float PowerPointTrapTimer = -1;
        public static float ZoomTrapTimer = -1;
        public static float PixellationTrapTimer = -1;
        public static GameObject AaaTrap;
        public static List<DialogQueue> AaaTrapLines = [];
        public static List<ArchipelagoItem> BufferedTraps = [];
        public static List<ArchipelagoItem> TrapLinks = [];
        public static bool RailTrap = false;
        public static float BufferTrapTimer = -1;

        // RingLink based values.
        public static int RingLinkCrystalCount = 0;
        public static float RingLinkTimer = 0;

        // Dictionary of custom sounds for item receives.
        public static Dictionary<string, AudioClip> ItemSounds = [];

        // Arrays of audio clips for each character finding a trap item in a chest.
        public static AudioClip[] LilacTrapSounds = [];
        public static AudioClip[] CarolTrapSounds = [];
        public static AudioClip[] MillaTrapSounds = [];
        public static AudioClip[] NeeraTrapSounds = [];

        public static List<DialogQueue> WeaponsCoreUnlockLines = [];

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

            // Check if the sounds directory exists.
            if (Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sounds\"))
            {
                // Loop through each WAV file in the sounds directory.
                foreach (string wavFile in Directory.GetFiles($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sounds\", "*.wav"))
                {
                    using (WWW audioLoader = new(Helpers.FilePathToFileUrl(wavFile)))
                    {
                        // Freeze the game until the audio loader is done.
                        while (!audioLoader.isDone)
                            System.Threading.Thread.Sleep(1);

                        // Create an audio clip from the loaded file.
                        AudioClip audio = audioLoader.GetAudioClip(false, true, AudioType.WAV);

                        // Freeze the application until the audio clip is loaded fully.
                        while (!(audio.loadState == AudioDataLoadState.Loaded))
                            System.Threading.Thread.Sleep(1);

                        // Add the loaded audio to our dictionary of audio clips.
                        ItemSounds.Add(Path.GetFileNameWithoutExtension(wavFile).ToLower(), audio);
                    }
                }
            }

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

            configDeathLinkOverride = Config.Bind("Overrides",
                                                  "DeathLink",
                                                  -1L,
                                                  "Overrides the DeathLink setting in the player YAML.\r\n" +
                                                  "-1: No Override\r\n" +
                                                  "0: Disabled\r\n" +
                                                  "1: Enabled\r\n" +
                                                  "2: Enable Survive");

            configRingLinkOverride = Config.Bind("Overrides",
                                                 "RingLink",
                                                 -1L,
                                                 "Overrides the RingLink setting in the player YAML.\r\n" +
                                                 "-1: No Override\r\n" +
                                                 "0: Disabled\r\n" +
                                                 "1: Enabled");

            configTrapLinkOverride = Config.Bind("Overrides",
                                                 "TrapLink",
                                                 -1L,
                                                 "Overrides the TrapLink setting in the player YAML.\r\n" +
                                                 "-1: No Override\r\n" +
                                                 "0: Disabled\r\n" +
                                                 "1: Enabled");

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

            // Create the Aaa Trap object.
            AaaTrap = GameObject.Instantiate(apAssetBundle.LoadAsset<GameObject>("AaaTrap"));
            DontDestroyOnLoad(AaaTrap);

            // Loop through the dialog in the Aaa trap.
            for (int aaaIndex = 0; aaaIndex < AaaTrap.GetComponent<PlayerDialog>().queue.Length; aaaIndex++)
            {
                // If the character is set to Aaa, then add it to the trap list.
                if (AaaTrap.GetComponent<PlayerDialog>().queue[aaaIndex].name == "Aaa")
                    AaaTrapLines.Add(AaaTrap.GetComponent<PlayerDialog>().queue[aaaIndex]);

                // If the character is set to Serpentine, then add it to the Weapon's Core Unlock Lines list.
                if (AaaTrap.GetComponent<PlayerDialog>().queue[aaaIndex].name == "Serpentine")
                    WeaponsCoreUnlockLines.Add(AaaTrap.GetComponent<PlayerDialog>().queue[aaaIndex]);

                // Clear out this entry in the Aaa trap.
                AaaTrap.GetComponent<PlayerDialog>().queue[aaaIndex] = new();
            }

            // Loop through each asset.
            foreach (string asset in apAssetBundle.GetAllAssetNames())
            {
                // Check if this asset is an OGG file.
                if (Path.GetExtension(asset) == ".ogg")
                {
                    // Add this asset to the approriate array if its one of our voice files.
                    if (Path.GetFileNameWithoutExtension(asset).StartsWith("fp2_lilac")) LilacTrapSounds = LilacTrapSounds.AddToArray(apAssetBundle.LoadAsset<AudioClip>(Path.GetFileNameWithoutExtension(asset)));
                    if (Path.GetFileNameWithoutExtension(asset).StartsWith("fp2_carol")) CarolTrapSounds = CarolTrapSounds.AddToArray(apAssetBundle.LoadAsset<AudioClip>(Path.GetFileNameWithoutExtension(asset)));
                    if (Path.GetFileNameWithoutExtension(asset).StartsWith("fp2_milla")) MillaTrapSounds = MillaTrapSounds.AddToArray(apAssetBundle.LoadAsset<AudioClip>(Path.GetFileNameWithoutExtension(asset)));
                    if (Path.GetFileNameWithoutExtension(asset).StartsWith("fp2_neera")) NeeraTrapSounds = NeeraTrapSounds.AddToArray(apAssetBundle.LoadAsset<AudioClip>(Path.GetFileNameWithoutExtension(asset)));
                }
            }

            // Patch all the functions that need patching.
            Harmony.CreateAndPatchAll(typeof(AcrabellePieTrapPatcher));
            Harmony.CreateAndPatchAll(typeof(ArenaRetryMenuPatcher));
            Harmony.CreateAndPatchAll(typeof(EnemySanity));
            Harmony.CreateAndPatchAll(typeof(FPAudioPatcher));
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
            Harmony.CreateAndPatchAll(typeof(MenuContinuePatcher));
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
            Harmony.CreateAndPatchAll(typeof(PlayerShipPatcher));
            Harmony.CreateAndPatchAll(typeof(PlayerSpawnPointPatcher));
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
                            if (item.Value > 1) message = $"Recieved {item.Key.ItemName} ({item.Value}x) from {item.Key.Source}.";
                            if (item.Key.Source == session.Players.GetPlayerName(session.ConnectionInfo.Slot) && item.Value > 1) message = $"Found your {item.Key.ItemName} ({item.Value}x).";

                            // Set the banner to the expand state and pass our message to it.
                            messageBanner.GetComponent<MessageBanner>().state = messageBanner.GetComponent<MessageBanner>().State_Expand;
                            messageBanner.GetComponent<MessageBanner>().text = message;

                            // Play the item get sound.
                            FPAudio.PlaySfx(FPAudio.SFX_ITEMGET);

                            // If we have a sound for this item, then play it too.
                            if (ItemSounds.ContainsKey(item.Key.ItemName.ToLower()))
                                FPAudio.PlaySfx(ItemSounds[item.Key.ItemName.ToLower()]);
                        }
                    }

                    // Remove 0.25 from the queue timer.
                    itemQueueTimer -= 0.25f;
                }
            }

            // Increment the RingLink timer.
            RingLinkTimer += Time.deltaTime;

            // Check if the timer has reached 0.25.
            if (RingLinkTimer >= 0.25f)
            {
                // Remove 0.25 from the RingLink timer.
                RingLinkTimer -= 0.25f;

                // Check if the Crystal count for the RingLink isn't 0.
                if (RingLinkCrystalCount != 0)
                {
                    // Create a packet for this RingLink and send it out.
                    BouncePacket packet = new()
                    {
                        Tags = ["RingLink"],
                        Data = new()
                        {
                            { "time", (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds },
                            { "source", session.ConnectionInfo.Slot },
                            { "amount", RingLinkCrystalCount }
                        }
                    };
                    session.Socket.SendPacket(packet);

                    // Reset the crystal count.
                    RingLinkCrystalCount = 0;
                }
            }
        
            // Decrement the Mirror Trap's timer if it's higher than 0 and a player exists.
            if (MirrorTrapTimer > 0 && FPPlayerPatcher.player != null)
                MirrorTrapTimer -= Time.deltaTime;

            // Check if the PowerPoint Trap Timer is above 0.
            if (PowerPointTrapTimer > 0)
            {
                // Set the game's framerate to 15 FPS.
                Application.targetFrameRate = 15;

                // Decrement the PowerPoint Trap Timer.
                PowerPointTrapTimer -= Time.deltaTime;
            }

            // If not, then check if the timer is between -1 and 0.
            else if (PowerPointTrapTimer <= 0 && PowerPointTrapTimer > -1)
            {
                // Set the PowerPoint Trap Timer to -1 so the framerate change doesn't fire every frame.
                PowerPointTrapTimer = -1;

                // Reset the game's framerate to the default value.
                FPSaveManager.SetTargetFPS();
            }

            // Check if the Zoom Trap Timer is above 0.
            if (ZoomTrapTimer > 0 && FPPlayerPatcher.player != null)
            {
                // Zoom the camera in to 0.5.
                FPCamera.stageCamera.RequestZoom(0.5f, FPCamera.ZoomPriority_VeryHigh);

                // Decrement the Zoom Trap Timer.
                ZoomTrapTimer -= Time.deltaTime;
            }

            // If not, then check if the timer is between -1 and 0.
            else if (ZoomTrapTimer <= 0 && ZoomTrapTimer > -1)
            {
                // Set the Zoom Trap Timer to -1 so the zoom level change doesn't fire every frame.
                ZoomTrapTimer = -1;

                // Zoom the camera back out to 1.
                FPCamera.stageCamera.RequestZoom(1f, FPCamera.ZoomPriority_VeryHigh);
            }

            // Check if the Pixellation Trap Timer is above 0.
            if (PixellationTrapTimer > 0)
            {
                // Check if the message label is currently idle.
                if (messageBanner.GetComponent<MessageBanner>().state == messageBanner.GetComponent<MessageBanner>().State_Idle)
                {
                    // Force the game to render at 25% scale.
                    FPCamera.stageCamera.ResizeRenderTextures(0.25f);

                    // Decrement the Pixellation Trap Timer.
                    PixellationTrapTimer -= Time.deltaTime;
                }

                // If the label isn't idle, then restore the normal internal scale so it can actually be read.
                else
                    FPCamera.stageCamera.ResizeRenderTextures(FPSaveManager.screenInternalScale);
            }

            // If not, then check if the timer is between -1 and 0.
            else if (PixellationTrapTimer <= 0 && PixellationTrapTimer > -1)
            {
                // Set the PowerPoint Trap Timer to -1 so the framerate change doesn't fire every frame.
                PixellationTrapTimer = -1;

                // Reset the game's framerate to the default value.
                FPCamera.stageCamera.ResizeRenderTextures(FPSaveManager.screenInternalScale);
            }

            // If we have a buffered trap and the timer isn't running, then randomly select a time between 5 and 30.
            if (BufferedTraps.Count > 0 && BufferTrapTimer == -1)
                BufferTrapTimer = rng.Next(5, 31);

            // Decrement the Buffered Trap Timer if the player exists.
            if (BufferTrapTimer > 0 && FPPlayerPatcher.player != null)
                BufferTrapTimer -= Time.deltaTime;

            // Check if the timer is between -1 and 0.
            if (BufferTrapTimer <= 0 && BufferTrapTimer > -1)
            {
                // Activate the trap we're waiting for.
                Helpers.HandleItem(new(BufferedTraps[0], 1));

                // Show a message for the activated trap.
                if (BufferedTraps[0].Source == session.Players.GetPlayerName(session.ConnectionInfo.Slot))
                    sentMessageQueue.Add($"Activating your {BufferedTraps[0].ItemName}.");
                else
                    sentMessageQueue.Add($"Activating {BufferedTraps[0].ItemName} from {BufferedTraps[0].Source}.");

                // If we have a sound for this item, then play it too.
                if (ItemSounds.ContainsKey(BufferedTraps[0].ItemName.ToLower()))
                    FPAudio.PlaySfx(ItemSounds[BufferedTraps[0].ItemName.ToLower()]);

                // Remove this trap from the list and reset the timer.
                BufferedTraps.RemoveAt(0);
                BufferTrapTimer = -1;
            }

            if (TrapLinks.Count > 0)
            {
                Helpers.HandleItem(new(TrapLinks[0], 1));
                TrapLinks.RemoveAt(0);
            }

            if (RailTrap)
            {
                // Get all the objects that have a collider.
                Collider2D[] colliderObjects = UnityEngine.Object.FindObjectsOfType<Collider2D>();

                // Loop through each object with a collider.
                foreach (Collider2D colliderObject in colliderObjects)
                {
                    // Check that this collider's object doesn't already have a rail.
                    if (colliderObject.gameObject.GetComponent<GrindRail>() == null)
                    {
                        // Create and attach a rail to the object.
                        GrindRail rail = colliderObject.gameObject.AddComponent<GrindRail>();

                        // Set the rail sounds from the asset bundle.
                        rail.sfxRailStart = apAssetBundle.LoadAsset<AudioClip>("GrindRail_Start");
                        rail.sfxRailLoop = apAssetBundle.LoadAsset<AudioClip>("GrindRail_Loop");
                    }
                }
            }
        }
    }
}
