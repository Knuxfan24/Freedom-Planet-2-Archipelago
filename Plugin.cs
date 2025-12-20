// TODO: Release "Found [x]'s [y]" messages.
global using Archipelago.MultiClient.Net;
global using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
global using Archipelago.MultiClient.Net.Models;
global using Archipelago.MultiClient.Net.Packets;
global using BepInEx;
global using Freedom_Planet_2_Archipelago.CustomData;
global using Freedom_Planet_2_Archipelago.Patchers;
global using HarmonyLib;
global using Newtonsoft.Json;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Reflection.Emit;
global using System.Threading;
global using UnityEngine;
global using UnityEngine.SceneManagement;

using BepInEx.Configuration;
using BepInEx.Logging;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Freedom_Planet_2_Archipelago
{
    [BepInPlugin("K24_FP2_Archipelago", "Archipelago", "0.2.0")]
    [BepInDependency("000.kuborro.libraries.fp2.fp2lib")]
    public class Plugin : BaseUnityPlugin
    {
        // The asset bundle exported from the Unity project.
        public static AssetBundle apAssetBundle;

        // The icons used for the chat box.
        public static Sprite apChatIcon;
        public static Dictionary<string, Sprite> apChatIcons = [];

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
        public static ConfigEntry<bool> configRemotePlayers;
        public static ConfigEntry<int> configChat;

        // The AP session's data.
        public static ArchipelagoSession session;
        public static Dictionary<string, object> slotData;
        public static Dictionary<long, ScoutedItemInfo> items;
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
        public static System.Random rng = new();
        public static bool usingRandomCharacter = false;
        public static List<GameObject> playerPrefabs = [];

        // Trap based values.
        public static float MirrorTrapTimer = -1;
        public static float PowerPointTrapTimer = -1;
        public static float ZoomTrapTimer = -1;
        public static float PixellationTrapTimer = -1;
        public static GameObject TextDisplay;
        public static List<DialogQueue> AaaTrapLines = [];
        public static List<ArchipelagoItem> BufferedTraps = [];
        public static List<ArchipelagoItem> TrapLinks = [];
        public static bool RailTrap = false;
        public static float BufferTrapTimer = -1;

        // RingLink based values.
        public static int RingLinkCrystalCount = 0;

        // Dictionary of custom sounds for item receives.
        public static Dictionary<string, AudioClip> ItemSounds = [];

        // Arrays of audio clips for each character finding a trap item in a chest.
        public static AudioClip[] LilacTrapSounds = [];
        public static AudioClip[] CarolTrapSounds = [];
        public static AudioClip[] MillaTrapSounds = [];
        public static AudioClip[] NeeraTrapSounds = [];

        // Serpentine's lines that are played when the Weapon's Core unlock criteria is met.
        public static List<DialogQueue> WeaponsCoreUnlockLines = [];
        
        // Background bounce-packet sender to keep SendPacket off the main thread.
        private static readonly Queue<BouncePacket> BounceQueue = new();
        private static readonly AutoResetEvent BounceSignal = new(false);
        private static Thread bounceThread;
        
        private static readonly Queue<LocationData> LocationQueue = new();
        private static readonly AutoResetEvent LocationSignal = new(false);
        private static Thread locationThread;

        // Stuff to handle remote players on a different thread.
        public static bool updatedRemotePlayer;
        public static JObject ourRemotePlayer;
        private static Thread remotePlayerThread;

        // How many spam traps are left.
        public static int SpamTrapCount;

        // Allow starting coroutines from static contexts.
        public static Plugin Instance;

        private void Awake()
        {
            // Set up the logger.
            consoleLog = Logger;

            // Set static instance for coroutine helpers.
            Instance = this;

            // Check for the asset bundle.
            if (!File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\archipelago.assets"))
            {
                consoleLog.LogError($@"Failed to find the archipelago.assets file! Please ensure it is correctly located in '{Paths.GameRootPath}\mod_overrides\Archipelago'.");
                return;
            }

            // Create the Archipelago directories if they doesn't exist.
            if (!Directory.Exists($@"{Paths.GameRootPath}\Archipelago Saves")) Directory.CreateDirectory($@"{Paths.GameRootPath}\Archipelago Saves");
            if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Players")) Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Players");
            if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sounds")) Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sounds");
            if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites")) Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites");
            if (!Directory.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Template Sprite Definitions")) Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Template Sprite Definitions");
                 
            // Loop through each WAV file in the sounds directory.
            foreach (string wavFile in Directory.GetFiles($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sounds\", "*.wav"))
            {
                #pragma warning disable IDE0063 // Use simple 'using' statement. Removing the braces feels MORE complicated.
                using (WWW audioLoader = new(Helpers.FilePathToFileUrl(wavFile)))
                {
                    // Freeze the game until the audio loader is done.
                    while (!audioLoader.isDone)
                        Thread.Sleep(1);

                    // Create an audio clip from the loaded file.
                    AudioClip audio = audioLoader.GetAudioClip(false, true, AudioType.WAV);

                    // Freeze the application until the audio clip is loaded fully.
                    while (!(audio.loadState == AudioDataLoadState.Loaded))
                        Thread.Sleep(1);

                    // Add the loaded audio to our dictionary of audio clips.
                    ItemSounds.Add(Path.GetFileNameWithoutExtension(wavFile).ToLower(), audio);
                }
                #pragma warning restore IDE0063
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

            configRemotePlayers = Config.Bind("Misc",
                                              "Remote Players",
                                              false,
                                              "Allows other Freedom Planet 2 players in the multiworld session to visually appear if in the same level.\r\n" +
                                              "false: Disabled\r\n" +
                                              "true: Enabled");

            configChat = Config.Bind("Misc",
                                     "Chat Piping",
                                     1,
                                     "Pipes the Archipelago chat through to the game using cutscene text boxes.\r\n" +
                                     "0: Disabled\r\n" +
                                     "1: Chat Messages Only\r\n" +
                                     "2: Full");

            // Load our asset bundle.
            apAssetBundle = AssetBundle.LoadFromFile($@"{Paths.GameRootPath}\mod_overrides\Archipelago\archipelago.assets");
            
            // Print all the asset names from the asset bundle, as a debug log.
            foreach (string assetName in apAssetBundle.GetAllAssetNames())
                consoleLog.LogDebug(assetName);

            // Load the chat icon.
            apChatIcon = Plugin.apAssetBundle.LoadAsset<Sprite>("chat_ap");

            // Loop through and get the player icons for the chat.
            foreach (string file in Directory.GetFiles($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Players", "*.png"))
            {
                // Set up a new texture using point filtering.
                Texture2D texture = new(76, 76) { filterMode = FilterMode.Point };

                // Read the sprite for this texture.
                texture.LoadImage(File.ReadAllBytes(file));

                // Add an entry to the chat icons list for this player.
                apChatIcons.Add(Path.GetFileNameWithoutExtension(file), Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(-0.1842f, 1.079f), 1));
            }

            // Create the message banner object.
            messageBanner = GameObject.Instantiate(apAssetBundle.LoadAsset<GameObject>("Message Label"));
            messageBanner.AddComponent<MessageBanner>();
            DontDestroyOnLoad(messageBanner);

            // Create the Text Display object from the Aaa Trap prefab.
            TextDisplay = GameObject.Instantiate(apAssetBundle.LoadAsset<GameObject>("AaaTrap"));
            DontDestroyOnLoad(TextDisplay);

            // Loop through the dialog in the text display.
            for (int textIndex = 0; textIndex < TextDisplay.GetComponent<PlayerDialog>().queue.Length; textIndex++)
            {
                // If the character is set to Aaa, then add it to the trap list.
                if (TextDisplay.GetComponent<PlayerDialog>().queue[textIndex].name == "Aaa")
                    AaaTrapLines.Add(TextDisplay.GetComponent<PlayerDialog>().queue[textIndex]);

                // If the character is set to Serpentine, then add it to the Weapon's Core Unlock Lines list.
                if (TextDisplay.GetComponent<PlayerDialog>().queue[textIndex].name == "Serpentine")
                    WeaponsCoreUnlockLines.Add(TextDisplay.GetComponent<PlayerDialog>().queue[textIndex]);
            }

            // Create a new (probably oversized) array for the text display.
            TextDisplay.GetComponent<PlayerDialog>().queue = new DialogQueue[4096];

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
            Harmony.CreateAndPatchAll(typeof(PlayerBFF2000Patcher));
            Harmony.CreateAndPatchAll(typeof(PlayerBossMergaPatcher));
            Harmony.CreateAndPatchAll(typeof(PlayerShipPatcher));
            Harmony.CreateAndPatchAll(typeof(PlayerSpawnPointPatcher));
        }

        // Helper to start coroutines from static code.
        public static void RunCoroutine(IEnumerator routine)
        {
            if (Instance != null && routine != null)
                Instance.StartCoroutine(routine);
        }

        private void Start()
        {
            StartCoroutine(SceneGuardLoop());
            StartCoroutine(ItemQueueLoop());
            StartCoroutine(RingLinkLoop());
            StartCoroutine(MirrorTrapLoop());
            StartCoroutine(PowerPointTrapWatcher());
            StartCoroutine(ZoomTrapWatcher());
            StartCoroutine(PixellationTrapWatcher());
            StartCoroutine(BufferedTrapLoop());
            StartCoroutine(TrapLinksLoop());
            StartCoroutine(RailTrapLoop());

            // Start background sender for bounce packets to avoid blocking the main thread.
            if (bounceThread == null)
            {
                bounceThread = new Thread(BounceSenderLoop) { IsBackground = true, Name = "AP Bounce Sender" };
                bounceThread.Start();
            }

            if (locationThread == null)
            {
                locationThread = new Thread(LocationSenderLoop) { IsBackground = true, Name = "AP Location Sender" };
                locationThread.Start();
            }

            if (remotePlayerThread == null)
            {
                remotePlayerThread = new Thread(RemotePlayerHandler) { IsBackground = true, Name = "AP Remote Player Handler" };
                remotePlayerThread.Start();
            }
        }
        
        public static void EnqueueBounce(BouncePacket packet)
        {
            lock (BounceQueue)
            {
                BounceQueue.Enqueue(packet);
                BounceSignal.Set();
            }
        }

        public static void EnqueueLocation(long locationIndex)
        {
            lock (LocationQueue)
            {
                LocationQueue.Enqueue(new LocationData
                {
                    LocationIndex = locationIndex
                });
                LocationSignal.Set();
            }
        }

        /// <summary>
        /// Handles updating our own remote player data.
        /// TODO: This thread seems to make the game freeze on close, requiring the BepInEx console to be closed to make it properly terminate?
        /// </summary>
        private static void RemotePlayerHandler()
        {
            while (Application.isPlaying)
            {
                try
                {
                    // If we aren't connected, then don't do anything here.
                    if (Plugin.session == null)
                        continue;
                    if (Plugin.session.ConnectionInfo.Slot == -1)
                        continue;

                    // If we don't have remote players enabled, then return out of this thread.
                    if (Plugin.configRemotePlayers.Value == false)
                        return;
    
                    // Check if our remote player value is updated.
                    if (updatedRemotePlayer)
                    {
                        // Update the data storage with our new information.
                        Plugin.session.DataStorage[$"FP2_PlayerSlot{Plugin.session.ConnectionInfo.Slot}"] = Plugin.ourRemotePlayer;

                        // Clear the updated flag.
                        updatedRemotePlayer = false;
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Handles updates to a remote player's data.
        /// </summary>
        #pragma warning disable IDE0060 // Remove unused parameter, as the additionalArguments value this complains about IS needed.
        public static void RemotePlayerChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
        #pragma warning restore IDE0060
        {
            // Find the remote player for this data storage entry.
            GameObject remotePlayerObject = GameObject.Find($"FP2_PlayerSlot{originalValue["Player"]}");

            // Check that we actually found an object.
            if (remotePlayerObject != null)
            {
                // Find the RemotePlayer script for this object.
                RemotePlayer script = remotePlayerObject.GetComponent<RemotePlayer>();

                // Check that we actually found a script.
                if (script != null)
                {
                    if (newValue["Scene"].Value<string>() == SceneManager.GetActiveScene().name)
                    {
                        // Update the remote player's position, animation and direction values.
                        script.position.x = newValue["PositionX"].Value<float>();
                        script.position.y = newValue["PositionY"].Value<float>();
                        script.currentAni = newValue["Animation"].Value<string>();
                        script.direction = (FPDirection)newValue["Facing"].Value<int>();
                    }
                    else
                    {
                        script.position.x = -240f;
                        script.position.y = 240f;
                    }

                    // Check if we've found a character for this player, or if they've changed due to a Swap Trap.
                    if (!script.hasVisualCharacter || originalValue["Character"].Value<string>() != newValue["Character"].Value<string>())
                    {
                        // Try to find the right prefab index for this player's character.
                        int prefabIndex = -1;
                        switch (newValue["Character"].Value<string>())
                        {
                            case "Lilac": prefabIndex = 0; break;
                            case "Carol": prefabIndex = 3; break;
                            case "Milla": prefabIndex = 2; break;
                            case "Neera": prefabIndex = 1; break;
                        }

                        // Check if we found a prefab index.
                        if (prefabIndex != -1)
                        {
                            // Set the hasCharacter flag to true.
                            script.hasVisualCharacter = true;

                            // Get the sprite and animator from the player prefab.
                            remotePlayerObject.GetComponent<SpriteRenderer>().sprite = Plugin.playerPrefabs[prefabIndex].GetComponent<SpriteRenderer>().sprite;

                            // Either get or create an animator for this player and set the controller to the one from the player prefab.
                            Animator animator = remotePlayerObject.GetComponent<Animator>() ?? remotePlayerObject.AddComponent<Animator>();
                            animator.runtimeAnimatorController = Plugin.playerPrefabs[prefabIndex].GetComponent<Animator>().runtimeAnimatorController;

                            // Shift the arrow and name up so they still appear above the player, even if they're Neera.
                            remotePlayerObject.transform.FindChild("arrow").transform.localPosition = new(0, 56, 0);
                            remotePlayerObject.transform.FindChild("Name").transform.localPosition = new(0, 72, -4);
                        }
                    }
                }
            }
        }

        private static void BounceSenderLoop()
        {
            while (Application.isPlaying)
            {
                BounceSignal.WaitOne();
                while (Application.isPlaying)
                {
                    BouncePacket packet = null;
                    lock (BounceQueue)
                    {
                        if (BounceQueue.Count > 0)
                            packet = BounceQueue.Dequeue();
                        else
                            break;
                    }

                    try
                    {
                        if (session != null && session.Socket != null && packet != null)
                            session.Socket.SendPacket(packet);
                    }
                    catch (Exception ex)
                    {
                        consoleLog?.LogWarning($"Bounce send failed: {ex.Message}");
                    }
                }
            }
        }

        private static void LocationSenderLoop()
        {
            while (Application.isPlaying)
            {
                LocationSignal.WaitOne();
                while (Application.isPlaying)
                {
                    LocationData location = null;
                    lock (LocationQueue)
                    {
                        if (LocationQueue.Count > 0)
                            location = LocationQueue.Dequeue();
                        else
                            break;
                    }

                    try
                    {
                        if (session != null && session.Socket != null && location != null)
                        {
                            session.Locations.CompleteLocationChecks(location.LocationIndex);
                            session.Locations.ScoutLocationsAsync(_ => {}, location.LocationIndex);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        consoleLog?.LogWarning($"Location send failed: {ex.Message}");
                    }
                }
            }
        }

        
        private IEnumerator SceneGuardLoop()
        {
            var wait = new WaitForSeconds(0.1f);
            while(Application.isPlaying)
            {
                string scene = SceneManager.GetActiveScene().name;
                if (scene == "Cutscene_BattlesphereCapsule" || scene == "ArenaChallengeMenu")
                    SceneManager.LoadScene("ArenaMenu");
                yield return wait;
            }
        }
        
        private IEnumerator ItemQueueLoop()
        {
            var tick = new WaitForSeconds(0.25f);

            while(Application.isPlaying)
            {
                if (itemQueueTimer != -1)
                {
                    // Only act when the banner is idle.
                    if (messageBanner != null &&
                        messageBanner.GetComponent<MessageBanner>().state == new FPObjectState(messageBanner.GetComponent<MessageBanner>().State_Idle))
                    {
                        // Sent messages take priority.
                        if (sentMessageQueue.Count != 0)
                        {
                            messageBanner.GetComponent<MessageBanner>().state = messageBanner.GetComponent<MessageBanner>().State_Expand;
                            messageBanner.GetComponent<MessageBanner>().text = sentMessageQueue[0];
                            sentMessageQueue.RemoveAt(0);
                            FPAudio.PlaySfx(FPAudio.SFX_ITEMGET);
                        }
                        else if (itemQueue.Count != 0)
                        {
                            KeyValuePair<ArchipelagoItem, int> item = itemQueue.ElementAt(0);
                            Helpers.HandleItem(item);
                            itemQueue.Remove(item.Key);

                            string selfName = session != null ? session.Players.GetPlayerName(session.ConnectionInfo.Slot) : "";
                            string message = $"Recieved {item.Key.ItemName} from {item.Key.Source}.";
                            if (item.Key.Source == selfName) message = $"Found your {item.Key.ItemName}.";
                            if (item.Value > 1) message = $"Recieved {item.Key.ItemName} ({item.Value}x) from {item.Key.Source}.";
                            if (item.Key.Source == selfName && item.Value > 1) message = $"Found your {item.Key.ItemName} ({item.Value}x).";

                            messageBanner.GetComponent<MessageBanner>().state = messageBanner.GetComponent<MessageBanner>().State_Expand;
                            messageBanner.GetComponent<MessageBanner>().text = message;

                            FPAudio.PlaySfx(FPAudio.SFX_ITEMGET);

                            if (ItemSounds.ContainsKey(item.Key.ItemName.ToLower()))
                                FPAudio.PlaySfx(ItemSounds[item.Key.ItemName.ToLower()]);
                        }
                    }

                    // Maintain similar semantics to original timer.
                    itemQueueTimer = Math.Max(0f, itemQueueTimer - 0.25f);
                }

                yield return tick;
            }
        }
        private IEnumerator RingLinkLoop()
        {
            var tick = new WaitForSeconds(0.25f);
            while(Application.isPlaying)
            {
                if (RingLinkCrystalCount != 0 && session != null)
                {
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

                    // Enqueue to background sender to avoid main-thread blocking.
                    EnqueueBounce(packet);
                    RingLinkCrystalCount = 0;
                }
                yield return tick;
            }
        }

        private IEnumerator MirrorTrapLoop()
        {
            while(Application.isPlaying)
            {
                if (MirrorTrapTimer > 0 && FPPlayerPatcher.player != null)
                    MirrorTrapTimer -= Time.deltaTime;
                yield return null;
            }
        }

        private bool _powerPointActive;
        private IEnumerator PowerPointTrapWatcher()
        {
            while(Application.isPlaying)
            {
                if (PowerPointTrapTimer > 0 && !_powerPointActive)
                {
                    float duration = PowerPointTrapTimer;
                    PowerPointTrapTimer = -1; // consume timer
                    yield return StartCoroutine(PowerPointTrapRoutine(duration));
                }
                else if (PowerPointTrapTimer <= 0 && PowerPointTrapTimer > -1 && !_powerPointActive)
                {
                    // Edge-case: ensure we reset FPS even if a short/ended timer was set.
                    PowerPointTrapTimer = -1;
                    FPSaveManager.SetTargetFPS();
                }
                yield return null;
            }
        }

        private IEnumerator PowerPointTrapRoutine(float duration)
        {
            _powerPointActive = true;
            Application.targetFrameRate = 15;
            float t = duration;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                yield return null;
            }
            FPSaveManager.SetTargetFPS();
            _powerPointActive = false;
        }

        private bool _zoomActive;
        private IEnumerator ZoomTrapWatcher()
        {
            while(Application.isPlaying)
            {
                if (ZoomTrapTimer > 0 && FPPlayerPatcher.player != null && !_zoomActive)
                {
                    float duration = ZoomTrapTimer;
                    ZoomTrapTimer = -1; // consume timer; routine handles timing
                    yield return StartCoroutine(ZoomTrapRoutine(duration));
                }
                else if (ZoomTrapTimer <= 0 && ZoomTrapTimer > -1 && !_zoomActive)
                {
                    // Ensure restoration if a zero/expired value arrives.
                    ZoomTrapTimer = -1;
                    FPCamera.stageCamera?.RequestZoom(1f, FPCamera.ZoomPriority_VeryHigh);
                }
                yield return null;
            }
        }

        private IEnumerator ZoomTrapRoutine(float duration)
        {
            _zoomActive = true;

            float t = duration;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                FPCamera.stageCamera?.RequestZoom(0.5f, FPCamera.ZoomPriority_VeryHigh);
                yield return null;
            }
            
            FPCamera.stageCamera?.RequestZoom(1f, FPCamera.ZoomPriority_VeryHigh);
            _zoomActive = false;
        }

        private bool _pixellationActive;
        private IEnumerator PixellationTrapWatcher()
        {
            while(Application.isPlaying)
            {
                if (PixellationTrapTimer > 0 && !_pixellationActive)
                {
                    float duration = PixellationTrapTimer;
                    PixellationTrapTimer = -1; // consume timer
                    yield return StartCoroutine(PixellationTrapRoutine(duration));
                }
                else if (PixellationTrapTimer <= 0 && PixellationTrapTimer > -1 && !_pixellationActive)
                {
                    PixellationTrapTimer = -1;
                    FPCamera.stageCamera?.ResizeRenderTextures(FPSaveManager.screenInternalScale);
                }
                yield return null;
            }
        }

        private IEnumerator PixellationTrapRoutine(float duration)
        {
            _pixellationActive = true;
            float t = duration;

            while (t > 0f)
            {
                // Match original behavior: pixelate only when banner is idle; otherwise restore temporarily for readability.
                if (messageBanner != null &&
                    messageBanner.GetComponent<MessageBanner>().state == messageBanner.GetComponent<MessageBanner>().State_Idle)
                {
                    FPCamera.stageCamera?.ResizeRenderTextures(0.25f);
                    t -= Time.deltaTime;
                }
                else
                {
                    FPCamera.stageCamera?.ResizeRenderTextures(FPSaveManager.screenInternalScale);
                }
                yield return null;
            }

            FPCamera.stageCamera?.ResizeRenderTextures(FPSaveManager.screenInternalScale);
            _pixellationActive = false;
        }

        private IEnumerator BufferedTrapLoop()
        {
            while(Application.isPlaying)
            {
                if (BufferedTraps.Count > 0 && BufferTrapTimer == -1)
                {
                    BufferTrapTimer = rng.Next(5, 31);
                    float delay = BufferTrapTimer;

                    // Wait the randomized delay; abort early if no player yet, but keep waiting next frame until player exists.
                    while (delay > 0f)
                    {
                        if (FPPlayerPatcher.player != null)
                            delay -= Time.deltaTime;
                        yield return null;
                    }

                    if (BufferedTraps.Count > 0)
                    {
                        var trap = BufferedTraps[0];
                        Helpers.HandleItem(new(trap, 1));

                        if (session != null && trap.Source == session.Players.GetPlayerName(session.ConnectionInfo.Slot))
                            sentMessageQueue.Add($"Activating your {trap.ItemName}.");
                        else
                            sentMessageQueue.Add($"Activating {trap.ItemName} from {trap.Source}.");

                        if (ItemSounds.ContainsKey(trap.ItemName.ToLower()))
                            FPAudio.PlaySfx(ItemSounds[trap.ItemName.ToLower()]);

                        BufferedTraps.RemoveAt(0);
                    }

                    BufferTrapTimer = -1;
                }
                yield return null;
            }
        }

        private IEnumerator TrapLinksLoop()
        {
            while(Application.isPlaying)
            {
                if (TrapLinks.Count > 0)
                {
                    Helpers.HandleItem(new(TrapLinks[0], 1), false, true);
                    TrapLinks.RemoveAt(0);
                }
                yield return null;
            }
        }

        private IEnumerator RailTrapLoop()
        {
            // Heavy operation guarded by an interval.
            var wait = new WaitForSeconds(1f);
            while(Application.isPlaying)
            {
                if (RailTrap)
                {
                    Collider2D[] colliderObjects = UnityEngine.Object.FindObjectsOfType<Collider2D>();
                    foreach (Collider2D colliderObject in colliderObjects)
                    {
                        if (colliderObject != null && colliderObject.gameObject.GetComponent<GrindRail>() == null)
                        {
                            GrindRail rail = colliderObject.gameObject.AddComponent<GrindRail>();
                            rail.sfxRailStart = apAssetBundle.LoadAsset<AudioClip>("GrindRail_Start");
                            rail.sfxRailLoop = apAssetBundle.LoadAsset<AudioClip>("GrindRail_Loop");
                        }
                    }
                }
                yield return wait;
            }
        }
    }
}
