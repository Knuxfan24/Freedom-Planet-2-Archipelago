// TODO: Check if the BFF2000 needs a special DeathLink too.
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
using System.Collections;
using System.Threading;

namespace Freedom_Planet_2_Archipelago
{
    [BepInPlugin("K24_FP2_Archipelago", "Archipelago", "0.1.1")]
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

        // Dictionary of custom sounds for item receives.
        public static Dictionary<string, AudioClip> ItemSounds = [];

        // Arrays of audio clips for each character finding a trap item in a chest.
        public static AudioClip[] LilacTrapSounds = [];
        public static AudioClip[] CarolTrapSounds = [];
        public static AudioClip[] MillaTrapSounds = [];
        public static AudioClip[] NeeraTrapSounds = [];

        public static List<DialogQueue> WeaponsCoreUnlockLines = [];
        
        // Background bounce-packet sender to keep SendPacket off the main thread.
        private static readonly Queue<BouncePacket> BounceQueue = new Queue<BouncePacket>();
        private static readonly AutoResetEvent BounceSignal = new AutoResetEvent(false);
        private static Thread bounceThread;

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
        }

        private IEnumerator SceneGuardLoop()
        {
            var wait = new WaitForSeconds(0.1f);
            while (true)
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

            while (true)
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
        
        public static void EnqueueBounce(BouncePacket packet)
        {
            lock (BounceQueue)
            {
                BounceQueue.Enqueue(packet);
                BounceSignal.Set();
            }
        }

        private static void BounceSenderLoop()
        {
            while (true)
            {
                BounceSignal.WaitOne();
                while (true)
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

        private IEnumerator RingLinkLoop()
        {
            var tick = new WaitForSeconds(0.25f);
            while (true)
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
            while (true)
            {
                if (MirrorTrapTimer > 0 && FPPlayerPatcher.player != null)
                    MirrorTrapTimer -= Time.deltaTime;
                yield return null;
            }
        }

        private bool _powerPointActive;
        private IEnumerator PowerPointTrapWatcher()
        {
            while (true)
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
            int originalFps = Application.targetFrameRate;
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
            while (true)
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
                    if (FPCamera.stageCamera != null)
                        FPCamera.stageCamera.RequestZoom(1f, FPCamera.ZoomPriority_VeryHigh);
                }
                yield return null;
            }
        }

        private IEnumerator ZoomTrapRoutine(float duration)
        {
            _zoomActive = true;
            if (FPCamera.stageCamera != null)
                FPCamera.stageCamera.RequestZoom(0.5f, FPCamera.ZoomPriority_VeryHigh);

            float t = duration;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                yield return null;
            }

            if (FPCamera.stageCamera != null)
                FPCamera.stageCamera.RequestZoom(1f, FPCamera.ZoomPriority_VeryHigh);
            _zoomActive = false;
        }

        private bool _pixellationActive;
        private IEnumerator PixellationTrapWatcher()
        {
            while (true)
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
                    if (FPCamera.stageCamera != null)
                        FPCamera.stageCamera.ResizeRenderTextures(FPSaveManager.screenInternalScale);
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
                    if (FPCamera.stageCamera != null)
                        FPCamera.stageCamera.ResizeRenderTextures(0.25f);
                    t -= Time.deltaTime;
                }
                else
                {
                    if (FPCamera.stageCamera != null)
                        FPCamera.stageCamera.ResizeRenderTextures(FPSaveManager.screenInternalScale);
                }
                yield return null;
            }

            if (FPCamera.stageCamera != null)
                FPCamera.stageCamera.ResizeRenderTextures(FPSaveManager.screenInternalScale);
            _pixellationActive = false;
        }

        private IEnumerator BufferedTrapLoop()
        {
            while (true)
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
            while (true)
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
            while (true)
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
