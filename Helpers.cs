﻿using Archipelago.MultiClient.Net.Models;
using BepInEx;
using FP2Lib.Player;
using Freedom_Planet_2_Archipelago.Patchers;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Freedom_Planet_2_Archipelago
{
    public class Helpers
    {
        /// <summary>
        /// Saves our AP data then forces the game itself to save.
        /// </summary>
        public static void Save()
        {
            // Save our AP data.
            File.WriteAllText($@"{Paths.GameRootPath}\Archipelago Saves\{Plugin.session.RoomState.Seed}_Save.json", JsonConvert.SerializeObject(Plugin.save, Formatting.Indented));

            // Force the game to save to the slot specified by our AP data.
            FPSaveManager.SaveToFile(Plugin.save.SaveSlot);
        }

        /// <summary>
        /// Gets the appropriate sprite for an item.
        /// </summary>
        /// <param name="scoutedLocationInfo">The information on the location we're getting a sprite for.</param>
        public static Sprite GetItemSprite(ScoutedItemInfo scoutedLocationInfo, bool respectInfoSetting = false)
        {
            // Load the Archipelago logo.
            Sprite[] apLogo = Plugin.apAssetBundle.LoadAssetWithSubAssets<Sprite>("archipelago");

            // Create a sprite using the generic Archipelago logo by default.
            Sprite sprite = apLogo[0];

            // If the Show Item Names in Shops setting is set to either Hidden or Nothing, then return the base sprite no matter what.
            if (respectInfoSetting)
                if ((long)Plugin.slotData["shop_information"] >= 2)
                    return sprite;

            // Swap to the Progression or Trap sprite if needed.
            if (scoutedLocationInfo.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Advancement)
                sprite = apLogo[1];
            if (scoutedLocationInfo.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Trap)
                sprite = apLogo[2];

            // If the Show Item Names in Shops setting is set to Flags, then return whichever AP Logo we have loaded.
            if (respectInfoSetting)
                if ((long)Plugin.slotData["shop_information"] == 1)
                    return sprite;

            // Check if an items file exists for the game this item is for.
            if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{scoutedLocationInfo.Player.Game}\items.json"))
            {
                // Load the item.json file.
                ItemDescriptor[] itemDescriptors = JsonConvert.DeserializeObject<ItemDescriptor[]>(File.ReadAllText($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{scoutedLocationInfo.Player.Game}\items.json"));

                // Loop through each item descriptor in the json file.
                foreach (ItemDescriptor item in itemDescriptors)
                {
                    // Check if this item descriptor's name array contains this item's name.
                    if (item.ItemNames.Contains(scoutedLocationInfo.ItemName))
                    {
                        // Check if the sprite called for in this item descriptor actually exists.
                        if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{scoutedLocationInfo.Player.Game}\{item.SpriteName}.png"))
                        {
                            // Set up a texture with this sprite.
                            Texture2D texture = GetCustomSprite($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{scoutedLocationInfo.Player.Game}\{item.SpriteName}.png");

                            // Return our custom sprite.
                            return Sprite.Create(GetCustomSprite($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{scoutedLocationInfo.Player.Game}\{item.SpriteName}.png"), new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

                            static Texture2D GetCustomSprite(string file)
                            {
                                // Set up a new texture using point filtering.
                                Texture2D texture = new(32, 32) { filterMode = FilterMode.Point };

                                // Read the sprite for this texture.
                                texture.LoadImage(File.ReadAllBytes(file));

                                // Return our custom texture.
                                return texture;
                            }
                        }
                    }
                }
            }

            // Check if this item is for Freedom Planet 2.
            if (scoutedLocationInfo.Player.Game == "Manual_FreedomPlanet2_Knuxfan24")
            {
                switch (scoutedLocationInfo.ItemName)
                {
                    // Multitude Items.
                    case "Gold Gem": return Plugin.apAssetBundle.LoadAsset<Sprite>("gold_gem");
                    case "Star Card": return Plugin.apAssetBundle.LoadAsset<Sprite>("star_card");
                    case "Time Capsule": return Plugin.apAssetBundle.LoadAsset<Sprite>("time_capsule");
                    case "Battlesphere Key": return Plugin.apAssetBundle.LoadAsset<Sprite>("battlesphere_key");

                    // Extra Item Slots.
                    case "Extra Item Slot": case "Extra Potion Slot": return Plugin.apAssetBundle.LoadAsset<Sprite>("extra_slots");

                    // Potions.
                    case "Potion - Accelerator": return Plugin.apAssetBundle.LoadAsset<Sprite>("speed_up");
                    case "Potion - Attack Up": return Plugin.apAssetBundle.LoadAsset<Sprite>("attack_up");
                    case "Potion - Cheaper Stocks": return Plugin.apAssetBundle.LoadAsset<Sprite>("cheaper_stocks");
                    case "Potion - Extra Stock": return Plugin.apAssetBundle.LoadAsset<Sprite>("extra_stock");
                    case "Potion - Healing Strike": return Plugin.apAssetBundle.LoadAsset<Sprite>("regeneration");
                    case "Potion - Strong Revivals": return Plugin.apAssetBundle.LoadAsset<Sprite>("full_revivals");
                    case "Potion - Strong Shields": return Plugin.apAssetBundle.LoadAsset<Sprite>("strong_shields");
                    case "Potion - Super Feather": return Plugin.apAssetBundle.LoadAsset<Sprite>("jump_up");

                    // Brave Stones.
                    case "Crystals to Petals": return Plugin.apAssetBundle.LoadAsset<Sprite>("more_petals");
                    case "Double Damage": return Plugin.apAssetBundle.LoadAsset<Sprite>("double_damage");
                    case "Earth Charm": return Plugin.apAssetBundle.LoadAsset<Sprite>("charm_earth");
                    case "Element Burst": return Plugin.apAssetBundle.LoadAsset<Sprite>("element_burst");
                    case "Expensive Stocks": return Plugin.apAssetBundle.LoadAsset<Sprite>("expensive_stocks");
                    case "Fire Charm": return Plugin.apAssetBundle.LoadAsset<Sprite>("charm_fire");
                    case "Items To Bombs": return Plugin.apAssetBundle.LoadAsset<Sprite>("items_to_bombs");
                    case "Life Oscillation": return Plugin.apAssetBundle.LoadAsset<Sprite>("bipolar_life");
                    case "Max Life Up": return Plugin.apAssetBundle.LoadAsset<Sprite>("max_life_up");
                    case "Metal Charm": return Plugin.apAssetBundle.LoadAsset<Sprite>("charm_metal");
                    case "No Guarding": return Plugin.apAssetBundle.LoadAsset<Sprite>("no_guarding");
                    case "No Petals": return Plugin.apAssetBundle.LoadAsset<Sprite>("petals_to_crystals");
                    case "No Revivals": return Plugin.apAssetBundle.LoadAsset<Sprite>("no_revivals");
                    case "No Stocks": return Plugin.apAssetBundle.LoadAsset<Sprite>("no_stocks");
                    case "One Hit KO": return Plugin.apAssetBundle.LoadAsset<Sprite>("one_hit_ko");
                    case "Payback Ring": return Plugin.apAssetBundle.LoadAsset<Sprite>("payback_ring");
                    case "Petal Armor": return Plugin.apAssetBundle.LoadAsset<Sprite>("petal_armor");
                    case "Rainbow Charm": return Plugin.apAssetBundle.LoadAsset<Sprite>("rainbow_charm");
                    case "Shadow Guard": return Plugin.apAssetBundle.LoadAsset<Sprite>("shadow_guard");
                    case "Time Limit": return Plugin.apAssetBundle.LoadAsset<Sprite>("time_limit");
                    case "Water Charm": return Plugin.apAssetBundle.LoadAsset<Sprite>("charm_water");
                    case "Wood Charm": return Plugin.apAssetBundle.LoadAsset<Sprite>("charm_wood");

                    // Chapters.
                    case "Progressive Chapter":
                    case "Mystery of the Frozen North":
                    case "Sky Pirate Panic":
                    case "Enter the Battlesphere":
                    case "Globe Opera":
                    case "Justice in the Sky Paradise":
                    case "Robot Wars! Snake VS Tarsier":
                    case "Echoes of the Dragon War":
                    case "Bakunawa": return Plugin.apAssetBundle.LoadAsset<Sprite>("chapter");

                    // Chest Tracers.
                    case "Chest Tracer":
                    case "Chest Tracer - Dragon Valley":
                    case "Chest Tracer - Shenlin Park":
                    case "Chest Tracer - Tiger Falls":
                    case "Chest Tracer - Robot Graveyard":
                    case "Chest Tracer - Shade Armory":
                    case "Chest Tracer - Avian Museum":
                    case "Chest Tracer - Airship Sigwada":
                    case "Chest Tracer - Phoenix Highway":
                    case "Chest Tracer - Zao Land":
                    case "Chest Tracer - Globe Opera 1":
                    case "Chest Tracer - Globe Opera 2":
                    case "Chest Tracer - Palace Courtyard":
                    case "Chest Tracer - Tidal Gate":
                    case "Chest Tracer - Sky Bridge":
                    case "Chest Tracer - Lightning Tower":
                    case "Chest Tracer - Zulon Jungle":
                    case "Chest Tracer - Nalao Lake":
                    case "Chest Tracer - Ancestral Forge":
                    case "Chest Tracer - Magma Starscape":
                    case "Chest Tracer - Gravity Bubble":
                    case "Chest Tracer - Bakunawa Rush":
                    case "Chest Tracer - Clockwork Arboretum":
                    case "Chest Tracer - Inversion Dynamo":
                    case "Chest Tracer - Lunar Cannon": return Plugin.apAssetBundle.LoadAsset<Sprite>("chest_tracer");

                    // Powerup Start, done seperately as we swap it out depending on the player character.
                    case "Powerup Start":
                        switch (FPSaveManager.character)
                        {
                            case FPCharacterID.LILAC: return Plugin.apAssetBundle.LoadAsset<Sprite>("powerup_start_lilac");
                            case FPCharacterID.CAROL: case FPCharacterID.BIKECAROL: return Plugin.apAssetBundle.LoadAsset<Sprite>("powerup_start_carol");
                            case FPCharacterID.MILLA: return Plugin.apAssetBundle.LoadAsset<Sprite>("powerup_start_milla");
                            case FPCharacterID.NEERA: return Plugin.apAssetBundle.LoadAsset<Sprite>("powerup_start_neera");
                            default: return PlayerHandler.GetPlayableCharaByFPCharacterId(FPSaveManager.character).itemFuel;
                        }
                }

            }

            // Return this sprite.
            return sprite;
        }

        /// <summary>
        /// Handles sorting items that are stored in the AP save.
        /// </summary>
        public static void HandleStartItems()
        {
            // Set up counts for the items that need to not be over given.
            int goldGemCount = 0;
            int mirrorTrapCount = 0;

            // Get the current save values for these items too.
            int saveGoldGemCount = Plugin.save.GoldGemCount;
            int fp2SaveGoldGemCount = FPSaveManager.totalGoldGems;
            int saveMirrorTrapCount = Plugin.save.MirrorTrapCount;

            // Loop through each item and see if its one of the problem items. If so, then increment its count.
            foreach (KeyValuePair<ArchipelagoItem, int> item in Plugin.itemQueue)
            {
                switch (item.Key.ItemName)
                {
                    case "Gold Gem": goldGemCount += item.Value; break;
                    case "Mirror Trap": mirrorTrapCount += item.Value; break;
                }
            }

            // Give all the items the server has to us.
            foreach (KeyValuePair<ArchipelagoItem, int> item in Plugin.itemQueue)
                HandleItem(item);

            // Calculate the true counts for the multitude items.
            int trueGoldGemCount = goldGemCount - saveGoldGemCount;
            int trueMirrorTrapCount = mirrorTrapCount - saveMirrorTrapCount;

            // Set the AP save item counts to the correct values.
            Plugin.save.GoldGemCount = saveGoldGemCount + trueGoldGemCount;
            Plugin.save.MirrorTrapCount = saveMirrorTrapCount + trueMirrorTrapCount;

            // Set our number of total gold gems to the correct value.
            FPSaveManager.totalGoldGems = fp2SaveGoldGemCount + trueGoldGemCount;

            // Set the mirror trap timer to the correct value.
            Plugin.MirrorTrapTimer = trueMirrorTrapCount * 30;

            // Save the two files.
            Save();
        }

        /// <summary>
        /// Handle actually receiving items from the multiworld.
        /// </summary>
        /// <param name="item">The information on this item and the quantity.</param>
        public static void HandleItem(KeyValuePair<ArchipelagoItem, int> item)
        {
            switch (item.Key.ItemName)
            {
                // Multitude Items that simply add to the save's value.
                case "Star Card": Plugin.save.StarCardCount += item.Value; break;
                case "Time Capsule": Plugin.save.TimeCapsuleCount += item.Value; break;
                case "Battlesphere Key": Plugin.save.BattlesphereKeyCount += item.Value; break;
                case "Gold Gem": Plugin.save.GoldGemCount += item.Value; FPSaveManager.totalGoldGems += item.Value; break;

                // Progressive Chapters, which need a for loop to make sure we get them all, then a loop through the save to find the first locked chapter and unlock it.
                case "Progressive Chapter":
                    for (int itemIndex = 0; itemIndex < item.Value; itemIndex++)
                    {
                        for (int chapterIndex = 0; chapterIndex < Plugin.save.ChapterUnlocks.Length; chapterIndex++)
                        {
                            if (!Plugin.save.ChapterUnlocks[chapterIndex])
                            {
                                Plugin.save.ChapterUnlocks[chapterIndex] = true;
                                break;
                            }
                        }
                    }
                    break;

                // Chapter Unlocks, which just change the value of their index in the chapter unlocks array.
                case "Mystery of the Frozen North": Plugin.save.ChapterUnlocks[0] = true; break;
                case "Sky Pirate Panic": Plugin.save.ChapterUnlocks[1] = true; break;
                case "Enter the Battlesphere": Plugin.save.ChapterUnlocks[2] = true; break;
                case "Globe Opera": Plugin.save.ChapterUnlocks[3] = true; break;
                case "Justice in the Sky Paradise": Plugin.save.ChapterUnlocks[4] = true; break;
                case "Robot Wars! Snake VS Tarsier": Plugin.save.ChapterUnlocks[5] = true; break;
                case "Echoes of the Dragon War": Plugin.save.ChapterUnlocks[6] = true; break;
                case "Bakunawa": Plugin.save.ChapterUnlocks[7] = true; break;

                // Chest Tracers, which just change the value of their index in the chest tracer array.
                case "Chest Tracer - Dragon Valley": Plugin.save.ChestTracers[0] = true; break;
                case "Chest Tracer - Shenlin Park": Plugin.save.ChestTracers[1] = true; break;
                case "Chest Tracer - Avian Museum": Plugin.save.ChestTracers[2] = true; break;
                case "Chest Tracer - Airship Sigwada": Plugin.save.ChestTracers[3] = true; break;
                case "Chest Tracer - Tiger Falls": Plugin.save.ChestTracers[4] = true; break;
                case "Chest Tracer - Robot Graveyard": Plugin.save.ChestTracers[5] = true; break;
                case "Chest Tracer - Shade Armory": Plugin.save.ChestTracers[6] = true; break;
                case "Chest Tracer - Phoenix Highway": Plugin.save.ChestTracers[7] = true; break;
                case "Chest Tracer - Zao Land": Plugin.save.ChestTracers[8] = true; break;
                case "Chest Tracer - Globe Opera 1": Plugin.save.ChestTracers[9] = true; break;
                case "Chest Tracer - Globe Opera 2": Plugin.save.ChestTracers[10] = true; break;
                case "Chest Tracer - Palace Courtyard": Plugin.save.ChestTracers[11] = true; break;
                case "Chest Tracer - Tidal Gate": Plugin.save.ChestTracers[12] = true; break;
                case "Chest Tracer - Zulon Jungle": Plugin.save.ChestTracers[13] = true; break;
                case "Chest Tracer - Nalao Lake": Plugin.save.ChestTracers[14] = true; break;
                case "Chest Tracer - Sky Bridge": Plugin.save.ChestTracers[15] = true; break;
                case "Chest Tracer - Lightning Tower": Plugin.save.ChestTracers[16] = true; break;
                case "Chest Tracer - Ancestral Forge": Plugin.save.ChestTracers[17] = true; break;
                case "Chest Tracer - Magma Starscape": Plugin.save.ChestTracers[18] = true; break;
                case "Chest Tracer - Gravity Bubble": Plugin.save.ChestTracers[19] = true; break;
                case "Chest Tracer - Bakunawa Rush": Plugin.save.ChestTracers[20] = true; break;
                case "Chest Tracer - Clockwork Arboretum": Plugin.save.ChestTracers[21] = true; break;
                case "Chest Tracer - Inversion Dynamo": Plugin.save.ChestTracers[22] = true; break;
                case "Chest Tracer - Lunar Cannon": Plugin.save.ChestTracers[23] = true; break;

                // The Global Chest Tracer, which simply sets all the values in the chest tracer array to true.
                case "Chest Tracer":
                    for (int tracerIndex = 0; tracerIndex < Plugin.save.ChestTracers.Length; tracerIndex++)
                        Plugin.save.ChestTracers[tracerIndex] = true;
                    break;

                // Extra Item Slots, which simply set a value in the game's actual save.
                case "Extra Item Slot": FPSaveManager.itemSlotExpansionLevel += (byte)Mathf.Clamp(item.Value, 0, 2); break;
                case "Extra Potion Slot": FPSaveManager.potionSlotExpansionLevel += (byte)Mathf.Clamp(item.Value, 0, 2); break;

                // Useful Brave Stones, which just change the value of their index in the brave stone array.
                case "Crystals to Petals": Plugin.save.BraveStones[7] = true; break;
                case "Earth Charm": Plugin.save.BraveStones[13] = true; break;
                case "Element Burst": Plugin.save.BraveStones[4] = true; break;
                case "Fire Charm": Plugin.save.BraveStones[15] = true; break;
                case "Max Life Up": Plugin.save.BraveStones[6] = true; break;
                case "Metal Charm": Plugin.save.BraveStones[16] = true; break;
                case "Payback Ring": Plugin.save.BraveStones[10] = true; break;
                case "Petal Armor": Plugin.save.BraveStones[5] = true; break;
                case "Powerup Start": Plugin.save.BraveStones[8] = true; break;
                case "Rainbow Charm": Plugin.save.BraveStones[17] = true; break;
                case "Shadow Guard": Plugin.save.BraveStones[9] = true; break;
                case "Water Charm": Plugin.save.BraveStones[14] = true; break;
                case "Wood Charm": Plugin.save.BraveStones[12] = true; break;

                // Trap Brave Stones, which set their value in the array and auto apply to the player if the option is set in the slot data.
                case "Double Damage": Plugin.save.BraveStones[20] = true; SetTrapBraveStone(FPPowerup.DOUBLE_DAMAGE); break;
                case "Expensive Stocks": Plugin.save.BraveStones[19] = true; SetTrapBraveStone(FPPowerup.EXPENSIVE_STOCKS); break;
                case "Items To Bombs": Plugin.save.BraveStones[26] = true; SetTrapBraveStone(FPPowerup.ITEMS_TO_BOMBS); break;
                case "Life Oscillation": Plugin.save.BraveStones[27] = true; SetTrapBraveStone(FPPowerup.BIPOLAR_LIFE); break;
                case "No Guarding": Plugin.save.BraveStones[22] = true; SetTrapBraveStone(FPPowerup.NO_GUARDING); break;
                case "No Petals": Plugin.save.BraveStones[23] = true; SetTrapBraveStone(FPPowerup.NO_PETALS); break;
                case "No Revivals": Plugin.save.BraveStones[21] = true; SetTrapBraveStone(FPPowerup.NO_REVIVALS); break;
                case "No Stocks": Plugin.save.BraveStones[18] = true; SetTrapBraveStone(FPPowerup.STOCK_DRAIN); break;
                case "One Hit KO": Plugin.save.BraveStones[28] = true; SetTrapBraveStone(FPPowerup.ONE_HIT_KO); break;
                case "Time Limit": Plugin.save.BraveStones[24] = true; SetTrapBraveStone(FPPowerup.TIME_LIMIT); break;

                // Potions, which just change the value of their index in the potion array.
                case "Potion - Accelerator": Plugin.save.Potions[7] = true; break;
                case "Potion - Attack Up": Plugin.save.Potions[5] = true; break;
                case "Potion - Cheaper Stocks": Plugin.save.Potions[3] = true; break;
                case "Potion - Extra Stock": Plugin.save.Potions[1] = true; break;
                case "Potion - Healing Strike": Plugin.save.Potions[4] = true; break;
                case "Potion - Strong Revivals": Plugin.save.Potions[2] = true; break;
                case "Potion - Strong Shields": Plugin.save.Potions[6] = true; break;
                case "Potion - Super Feather": Plugin.save.Potions[8] = true; break;

                // Traps.
                case "Swap Trap":
                    if (FPPlayerPatcher.player != null)
                        FPPlayerPatcher.SwapTrap();
                    break;

                case "Mirror Trap":
                    Plugin.MirrorTrapTimer += 30f * item.Value;
                    Plugin.save.MirrorTrapCount += item.Value;
                    break;

                case "Pie Trap":
                    // Loop through to create as many pies as we received (for the lols).
                    for (int pieIndex = 0; pieIndex < item.Value; pieIndex++)
                    {
                        // Check that the player exists and that the stage has finished registering its objects.
                        if (FPPlayerPatcher.player != null && FPStage.objectsRegistered)
                        {
                            // Create a pie from the prefab and set its name to APPieTrap.
                            GameObject trapPie = GameObject.Instantiate(Plugin.apAssetBundle.LoadAsset<GameObject>("PieTrap"));
                            trapPie.name = "APPieTrap";
                        }
                    }
                    break;

                // Unhandled items, throw an error into the console.
                default: Plugin.consoleLog.LogError($"Item Type '{item.Key.ItemName}' (sent by '{item.Key.Source}' {item.Value} time(s)) not yet handled!"); return;
            }

            // If this is a chest tracer and the player is active, then recreate the tracers.
            if (item.Key.ItemName.StartsWith("Chest Tracer") && FPPlayerPatcher.player != null)
                FPPlayerPatcher.CreateChestTracers();

            // Save the two files.
            Save();

            void SetTrapBraveStone(FPPowerup item)
            {
                // Check that we have the trap brave stones option and that the player actually exists.
                if ((long)Plugin.slotData["trap_stones"] != 0 && FPPlayerPatcher.player != null)
                {
                    // If this Brave Stone is already equipped on the player, then don't add a second copy of it.
                    if (FPPlayerPatcher.player.powerups.Contains(item))
                        return;

                    // Add this item to the player.
                    FPPlayerPatcher.player.powerups = FPPlayerPatcher.player.powerups.AddItem(item).ToArray();

                    // Apply extra edits for items that need them.
                    switch (item)
                    {
                        case FPPowerup.STOCK_DRAIN: FPPlayerPatcher.player.lives = 0; break;
                        case FPPowerup.ONE_HIT_KO: FPPlayerPatcher.player.health = 0; break;

                        case FPPowerup.EXPENSIVE_STOCKS:
                            FPPlayerPatcher.player.extraLifeCost += 250;
                            FPPlayerPatcher.player.crystals += 250;
                            break;

                        case FPPowerup.TIME_LIMIT:
                            // Find the HUD that we need to attach the timer to.
                            FPHudMaster hud = UnityEngine.GameObject.FindObjectOfType<FPHudMaster>();

                            // Check that the HUD actually exists.
                            if (hud != null)
                            {
                                // Check that this stage isn't set to only show health.
                                if (!hud.onlyShowHealth)
                                {
                                    if (FPSaveManager.GetStageParTime(FPStage.currentStage.stageID) - (FPStage.currentStage.minutes * 6000 + FPStage.currentStage.seconds * 100 + FPStage.currentStage.milliSeconds) < 0)
                                    {
                                        Plugin.sentMessageQueue.Add("But the Time Limit has already elapsed.");
                                        return;
                                    }

                                    // Instantiate the time limit prefab from the HUD and parent it to it.
                                    GameObject timeLimit = UnityEngine.GameObject.Instantiate(hud.pfHudTimeLimit);
                                    timeLimit.transform.parent = hud.transform;

                                    // Update the HUD's reference to the bar.
                                    Traverse.Create(hud).Field("hudTimeLimitBar").SetValue(timeLimit);

                                    // Set up an array of positions for the digits, as well as an array to store them.
                                    float[] xPositions = [83, 93, 109, 119, 135, 145];
                                    FPHudDigit[] hudTimeLimit = new FPHudDigit[6];

                                    // Loop through the six digits in the timer.
                                    for (int digitIndex = 0; digitIndex <= 5; digitIndex++)
                                    {
                                        // Instantiate the time limit digit prefab from the HUD and parent it to it.
                                        timeLimit = UnityEngine.GameObject.Instantiate(hud.pfHudTimeLimitDigit, new Vector3(xPositions[digitIndex], -62f, 0f), default);
                                        timeLimit.transform.parent = hud.transform;

                                        // Add this digit to our array.
                                        hudTimeLimit[digitIndex] = timeLimit.GetComponent<FPHudDigit>();
                                    }

                                    // Set the HUD's digit array to our one.
                                    Traverse.Create(hud).Field("hudTimeLimit").SetValue(hudTimeLimit);

                                    // Set the HUD's countdown threshold.
                                    Traverse.Create(hud).Field("countdownThreshold").SetValue(2000);
                                }
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a location exists in the multiworld.
        /// </summary>
        public static bool CheckLocationExists(long locationIndex) => locationIndex != -1 && Plugin.session.Locations.AllLocations.Contains(locationIndex);

        /// <summary>
        /// Get the name of the active character.
        /// </summary>
        /// <returns>The character name.</returns>
        public static string GetPlayer()
        {
            switch (FPSaveManager.character)
            {
                // Return a vanilla character's name.
                case FPCharacterID.LILAC: return "Lilac";
                case FPCharacterID.CAROL: case FPCharacterID.BIKECAROL: return "Carol";
                case FPCharacterID.MILLA: return "Milla";
                case FPCharacterID.NEERA: return "Neera";

                // If we're none of the vanilla characters, then look for our current ID in FP2Lib's list and return their name.
                default:
                    foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
                        if (chara.id == (int)FPSaveManager.character)
                            return chara.Name;

                    // If even that failed to turn up a name, then return a generic one.
                    return "Somebody we have no knowledge of";
            }
        }
    }
}
