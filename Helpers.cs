using Archipelago.MultiClient.Net.Models;
using FP2Lib.Player;
using Freedom_Planet_2_Archipelago.Patchers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

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
                    // Key Items.
                    case "Star Card": return Plugin.apAssetBundle.LoadAsset<Sprite>("star_card");
                    case "Time Capsule": return Plugin.apAssetBundle.LoadAsset<Sprite>("time_capsule");
                    case "Battlesphere Key": return Plugin.apAssetBundle.LoadAsset<Sprite>("battlesphere_key");

                    // Filler Items.
                    case "Gold Gem": return Plugin.apAssetBundle.LoadAsset<Sprite>("gold_gem");
                    case "Crystals": return Plugin.apAssetBundle.LoadAsset<Sprite>("crystals");
                    case "Invincibility": return Plugin.apAssetBundle.LoadAsset<Sprite>("invincibility");
                    case "Wood Shield": return Plugin.apAssetBundle.LoadAsset<Sprite>("wood_shield");
                    case "Earth Shield": return Plugin.apAssetBundle.LoadAsset<Sprite>("earth_shield");
                    case "Water Shield": return Plugin.apAssetBundle.LoadAsset<Sprite>("water_shield");
                    case "Fire Shield": return Plugin.apAssetBundle.LoadAsset<Sprite>("fire_shield");
                    case "Metal Shield": return Plugin.apAssetBundle.LoadAsset<Sprite>("metal_shield");

                    // Extra Life, done seperately as we swap it out depending on the player character.
                    case "Extra Life":
                        switch (FPSaveManager.character)
                        {
                            case FPCharacterID.LILAC: return Plugin.apAssetBundle.LoadAsset<Sprite>("extra_life_lilac");
                            case FPCharacterID.CAROL: case FPCharacterID.BIKECAROL: return Plugin.apAssetBundle.LoadAsset<Sprite>("extra_life_carol");
                            case FPCharacterID.MILLA: return Plugin.apAssetBundle.LoadAsset<Sprite>("extra_life_milla");
                            case FPCharacterID.NEERA: return Plugin.apAssetBundle.LoadAsset<Sprite>("extra_life_neera");
                            default: return PlayerHandler.GetPlayableCharaByFPCharacterId(FPSaveManager.character).livesIconAnim[0];
                        }

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

                    // Powerup and Powerup Start, done seperately as we swap it out depending on the player character.
                    case "Powerup":
                    case "Powerup Start":
                        switch (FPSaveManager.character)
                        {
                            case FPCharacterID.LILAC: return Plugin.apAssetBundle.LoadAsset<Sprite>("powerup_start_lilac");
                            case FPCharacterID.CAROL: case FPCharacterID.BIKECAROL: return Plugin.apAssetBundle.LoadAsset<Sprite>("powerup_start_carol");
                            case FPCharacterID.MILLA: return Plugin.apAssetBundle.LoadAsset<Sprite>("powerup_start_milla");
                            case FPCharacterID.NEERA: return Plugin.apAssetBundle.LoadAsset<Sprite>("powerup_start_neera");
                            default: return PlayerHandler.GetPlayableCharaByFPCharacterId(FPSaveManager.character).itemFuel;
                        }

                    // Traps
                    case "Aaa Trap": return Plugin.apAssetBundle.LoadAsset<Sprite>("aaa_trap");
                    case "Pie Trap": return Plugin.apAssetBundle.LoadAsset<Sprite>("pie_trap");
                    case "Spring Trap": return Plugin.apAssetBundle.LoadAsset<Sprite>("spring_trap");
                    case "Zoom Trap": return Plugin.apAssetBundle.LoadAsset<Sprite>("zoom_trap");
                    case "Spike Ball Trap": return Plugin.apAssetBundle.LoadAsset<Sprite>("spike_ball_trap");
                }

            }

            // Return this sprite.
            return sprite;
        }

        /// <summary>
        /// Handles sorting items that are stored in the AP save.
        /// TODO: This feels really messy...
        /// </summary>
        public static void HandleStartItems()
        {
            // Set up counts for the items that need to not be over given.
            int mirrorTrapCount = 0;
            int powerPointTrapCount = 0;
            int zoomTrapCount = 0;
            int aaaTrapCount = 0;
            int goldGemCount = 0;
            int crystalCount = 0;
            int extraLifeCount = 0;
            int invincibilityCount = 0;
            int shieldCount = 0;
            int powerupCount = 0;

            // Get the current save values for these items too.
            int saveMirrorTrapCount = Plugin.save.MirrorTrapCount;
            int savePowerPointTrapCount = Plugin.save.PowerPointTrapCount;
            int saveZoomTrapCount = Plugin.save.ZoomTrapCount;
            int saveAaaTrapCount = Plugin.save.AaaTrapCount;
            int saveGoldGemCount = Plugin.save.GoldGemCount;
            int fp2SaveGoldGemCount = FPSaveManager.totalGoldGems;
            int saveCrystalCount = Plugin.save.CrystalCount;
            int fp2SaveCrystalCount = FPSaveManager.totalCrystals;
            int saveExtraLifeCount = Plugin.save.ExtraLifeCount;
            int saveInvincibilityCount = Plugin.save.InvincibilityCount;
            int saveShieldCount = Plugin.save.ShieldCount;
            int savePowerupCount = Plugin.save.PowerupCount;

            // Loop through each item and see if its one of the problem items. If so, then increment its count.
            foreach (KeyValuePair<ArchipelagoItem, int> item in Plugin.itemQueue)
            {
                switch (item.Key.ItemName)
                {
                    case "Mirror Trap": mirrorTrapCount += item.Value; break;
                    case "PowerPoint Trap": powerPointTrapCount += item.Value; break;
                    case "Zoom Trap": zoomTrapCount += item.Value; break;
                    case "Aaa Trap": aaaTrapCount += item.Value; break;

                    case "Gold Gem": goldGemCount += item.Value; break;
                    case "Crystals": crystalCount += item.Value * 100; break;
                    case "Extra Life": extraLifeCount += item.Value; break;
                    case "Invincibility": invincibilityCount += item.Value; break;
                    case "Wood Shield": case "Earth Shield": case "Water Shield": case "Fire Shield": case "Metal Shield": shieldCount += item.Value; break;
                    case "Powerup": powerupCount += item.Value; break;
                }
            }

            // Give all the items the server has to us.
            foreach (KeyValuePair<ArchipelagoItem, int> item in Plugin.itemQueue)
                HandleItem(item);

            // Clear out the Aaa Trap queue.
            for (int i = 0; i < Plugin.AaaTrap.GetComponent<PlayerDialog>().queue.Length; i++)
                Plugin.AaaTrap.GetComponent<PlayerDialog>().queue[i] = new();

            // Clear out the buffered trap queue.
            Plugin.BufferedTraps.Clear();

            // Calculate the true counts for the multitude items.
            int trueGoldGemCount = goldGemCount - saveGoldGemCount;
            int trueMirrorTrapCount = mirrorTrapCount - saveMirrorTrapCount;
            int truePowerPointTrapCount = powerPointTrapCount - savePowerPointTrapCount;
            int trueZoomTrapCount = zoomTrapCount - saveZoomTrapCount;
            int trueAaaTrapCount = aaaTrapCount - saveAaaTrapCount;
            int trueCrystalCount = crystalCount - saveCrystalCount;
            int trueExtraLifeCount = extraLifeCount - saveExtraLifeCount;
            int trueInvincibilityCount = invincibilityCount - saveInvincibilityCount;
            int trueShieldCount = shieldCount - saveShieldCount;
            int truePowerupCount = powerupCount - savePowerupCount;

            // Set the AP save item counts to the correct values.
            Plugin.save.GoldGemCount = saveGoldGemCount + trueGoldGemCount;
            Plugin.save.MirrorTrapCount = saveMirrorTrapCount + trueMirrorTrapCount;
            Plugin.save.PowerPointTrapCount = savePowerPointTrapCount + truePowerPointTrapCount;
            Plugin.save.ZoomTrapCount = saveZoomTrapCount + trueZoomTrapCount;
            Plugin.save.AaaTrapCount = saveAaaTrapCount + trueAaaTrapCount;
            Plugin.save.CrystalCount = saveCrystalCount + trueCrystalCount;
            Plugin.save.ExtraLifeCount = saveExtraLifeCount + trueExtraLifeCount;
            Plugin.save.InvincibilityCount = saveInvincibilityCount + trueInvincibilityCount;
            Plugin.save.ShieldCount = saveShieldCount + trueShieldCount;
            Plugin.save.PowerupCount = savePowerupCount + truePowerupCount;

            // Set the trap timers to the correct value.
            Plugin.MirrorTrapTimer = trueMirrorTrapCount * 30;
            Plugin.PowerPointTrapTimer = truePowerPointTrapCount * 30;
            Plugin.ZoomTrapTimer = trueZoomTrapCount * 30;

            // Set our number of total gold gems to the correct value.
            FPSaveManager.totalGoldGems = fp2SaveGoldGemCount + trueGoldGemCount;

            // Set our number of total crystals to the correct value.
            FPSaveManager.totalCrystals = fp2SaveCrystalCount + trueCrystalCount;

            // If we don't actually have any extra lives, then reset the value in the player patcher.
            if (trueExtraLifeCount == 0) FPPlayerPatcher.hasBufferedExtraLives = 0;

            // If we don't actually have an invincibility, then reset the flag in the player patcher.
            if (trueInvincibilityCount == 0) FPPlayerPatcher.hasBufferedInvincibility = false;

            // If we don't actually have a shield, then reset the flag in the player patcher.
            if (trueShieldCount == 0) FPPlayerPatcher.hasBufferedShield = FPItemBoxTypes.BOX_CRATE;

            // If we don't actually have a powerup, then reset the flag in the player patcher.
            if (truePowerupCount == 0) FPPlayerPatcher.hasBufferedPowerup = false;

            // Loop through the amount of Aaa Traps we got.
            for (int trapIndex = 0; trapIndex < trueAaaTrapCount; trapIndex++)
            {
                // Loop through between 3 to 10 times.
                for (int voiceLineIndex = 0; voiceLineIndex < Plugin.rng.Next(3, 11); voiceLineIndex++)
                {
                    // Loop through each entry in the queue.
                    for (int queueIndex = 0; queueIndex < Plugin.AaaTrap.GetComponent<PlayerDialog>().queue.Length; queueIndex++)
                    {
                        // If this entry isn't populated already, then add a random line to it, mark it as active, then stop looping.
                        if (Plugin.AaaTrap.GetComponent<PlayerDialog>().queue[queueIndex].name != "Aaa")
                        {
                            Plugin.AaaTrap.GetComponent<PlayerDialog>().queue[queueIndex] = Plugin.AaaTrapLines[Plugin.rng.Next(Plugin.AaaTrapLines.Count)];
                            Plugin.AaaTrap.GetComponent<PlayerDialog>().queue[queueIndex].active = true;
                            break;
                        }
                    }
                }
            }

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

                // Filler Items that (mostly) activate on the player.
                case "Gold Gem":
                    Plugin.save.GoldGemCount += item.Value;
                    FPSaveManager.totalGoldGems += item.Value;
                    break;

                case "Crystals":
                    Plugin.save.CrystalCount += item.Value * 100;

                    if (FPPlayerPatcher.player != null)
                        for (int i = 0; i < item.Value * 100; i++)
                            FPSaveManager.AddCrystal(FPPlayerPatcher.player);
                    else
                        FPSaveManager.totalCrystals += item.Value * 100;

                    break;

                case "Extra Life":
                    Plugin.save.ExtraLifeCount += item.Value;
                    FPPlayerPatcher.hasBufferedExtraLives += item.Value;
                    break;

                case "Invincibility":
                    Plugin.save.InvincibilityCount += item.Value;
                    FPPlayerPatcher.hasBufferedInvincibility = true;
                    break;

                case "Wood Shield":
                    Plugin.save.ShieldCount += item.Value;
                    FPPlayerPatcher.hasBufferedShield = FPItemBoxTypes.BOX_WOODSHIELD;
                    break;
                case "Earth Shield":
                    Plugin.save.ShieldCount += item.Value;
                    FPPlayerPatcher.hasBufferedShield = FPItemBoxTypes.BOX_EARTHSHIELD;
                    break;
                case "Water Shield":
                    Plugin.save.ShieldCount += item.Value;
                    FPPlayerPatcher.hasBufferedShield = FPItemBoxTypes.BOX_WATERSHIELD;
                    break;
                case "Fire Shield":
                    Plugin.save.ShieldCount += item.Value;
                    FPPlayerPatcher.hasBufferedShield = FPItemBoxTypes.BOX_FIRESHIELD;
                    break;
                case "Metal Shield":
                    Plugin.save.ShieldCount += item.Value;
                    FPPlayerPatcher.hasBufferedShield = FPItemBoxTypes.BOX_METALSHIELD;
                    break;

                case "Powerup":
                    Plugin.save.PowerupCount += item.Value;
                    FPPlayerPatcher.hasBufferedPowerup = true;
                    break;

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
                    else
                        Plugin.BufferedTraps.Add(item.Key);
                    break;

                case "Mirror Trap":
                    Plugin.MirrorTrapTimer += 30f * item.Value;
                    Plugin.save.MirrorTrapCount += item.Value;
                    break;

                case "PowerPoint Trap":
                    Plugin.PowerPointTrapTimer += 30f * item.Value;
                    Plugin.save.PowerPointTrapCount += item.Value;
                    break;

                case "Zoom Trap":
                    Plugin.ZoomTrapTimer += 30f * item.Value;
                    Plugin.save.ZoomTrapCount += item.Value;
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
                        else
                            Plugin.BufferedTraps.Add(item.Key);
                    }
                    break;

                case "Spring Trap":
                    // Loop through to create as many springs as we received (for the lols).
                    for (int springIndex = 0; springIndex < item.Value; springIndex++)
                    {
                        // Check that the player exists and that the stage has finished registering its objects.
                        if (FPPlayerPatcher.player != null && FPStage.objectsRegistered)
                        {
                            // Create a spring from the prefab and set its name to APSpringTrap.
                            GameObject trapSpring = GameObject.Instantiate(Plugin.apAssetBundle.LoadAsset<GameObject>("SpringTrap"));
                            trapSpring.name = "APSpringTrap";

                            // Set the spring's position, angle and rotation depending on the player's direction.
                            if (FPPlayerPatcher.player.direction == FPDirection.FACING_RIGHT)
                            {
                                trapSpring.transform.position = new(FPPlayerPatcher.player.position.x + 64, FPPlayerPatcher.player.position.y, 0);
                                trapSpring.GetComponent<Spring>().angle = 90;
                                trapSpring.transform.rotation = Quaternion.Euler(0, 0, 90);
                            }
                            else
                            {
                                trapSpring.transform.position = new(FPPlayerPatcher.player.position.x - 64, FPPlayerPatcher.player.position.y, 0);
                            }
                        }
                        else
                            Plugin.BufferedTraps.Add(item.Key);
                    }
                    break;

                case "Aaa Trap":
                    // Increment the trap count in the save.
                    Plugin.save.AaaTrapCount += item.Value;

                    // Loop through based on the amount of traps.
                    for (int trapIndex = 0; trapIndex < item.Value; trapIndex++)
                    {
                        // Loop through between 3 to 10 times.
                        for (int voiceLineIndex = 0; voiceLineIndex < Plugin.rng.Next(3, 11); voiceLineIndex++)
                        {
                            // Loop through each entry in the queue.
                            for (int queueIndex = 0; queueIndex < Plugin.AaaTrap.GetComponent<PlayerDialog>().queue.Length; queueIndex++)
                            {
                                // If this entry isn't populated already, then add a random line to it, mark it as active, then stop looping.
                                if (Plugin.AaaTrap.GetComponent<PlayerDialog>().queue[queueIndex].name != "Aaa")
                                {
                                    Plugin.AaaTrap.GetComponent<PlayerDialog>().queue[queueIndex] = Plugin.AaaTrapLines[Plugin.rng.Next(Plugin.AaaTrapLines.Count)];
                                    Plugin.AaaTrap.GetComponent<PlayerDialog>().queue[queueIndex].active = true;
                                    break;
                                }
                            }
                        }
                    }
                    break;


                case "Spike Ball Trap":
                    // Check that the player exists and that the stage has finished registering its objects.
                    if (FPPlayerPatcher.player != null && FPStage.objectsRegistered)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            SpawnSpikeBall(new(FPPlayerPatcher.player.transform.position.x - 400, FPPlayerPatcher.player.transform.position.y, FPPlayerPatcher.player.transform.position.z), 10f);
                            SpawnSpikeBall(new(FPPlayerPatcher.player.transform.position.x - 400, FPPlayerPatcher.player.transform.position.y + 64, FPPlayerPatcher.player.transform.position.z), 10f);
                            SpawnSpikeBall(new(FPPlayerPatcher.player.transform.position.x - 400, FPPlayerPatcher.player.transform.position.y + 128, FPPlayerPatcher.player.transform.position.z), 10f);
                            SpawnSpikeBall(new(FPPlayerPatcher.player.transform.position.x - 400, FPPlayerPatcher.player.transform.position.y + 192, FPPlayerPatcher.player.transform.position.z), 10f);
                            SpawnSpikeBall(new(FPPlayerPatcher.player.transform.position.x + 400, FPPlayerPatcher.player.transform.position.y, FPPlayerPatcher.player.transform.position.z), -10f);
                            SpawnSpikeBall(new(FPPlayerPatcher.player.transform.position.x + 400, FPPlayerPatcher.player.transform.position.y + 64, FPPlayerPatcher.player.transform.position.z), -10f);
                            SpawnSpikeBall(new(FPPlayerPatcher.player.transform.position.x + 400, FPPlayerPatcher.player.transform.position.y + 128, FPPlayerPatcher.player.transform.position.z), -10f);
                            SpawnSpikeBall(new(FPPlayerPatcher.player.transform.position.x + 400, FPPlayerPatcher.player.transform.position.y + 192, FPPlayerPatcher.player.transform.position.z), -10f);
                        }
                    }
                    else
                        Plugin.BufferedTraps.Add(item.Key);
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
            // Determine our character ID.
            int character = -1;

            if (FPPlayerPatcher.player != null)
                character = (int)FPPlayerPatcher.player.characterID;
            else
                character = (int)FPSaveManager.character;

            switch ((FPCharacterID)character)
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

        public static void SpawnSpikeBall(Vector3 position, float velocity)
        {
            // Create a spikeball from the prefab.
            GameObject trapSpikeBall = GameObject.Instantiate(Plugin.apAssetBundle.LoadAsset<GameObject>("SpikeBallTrap"));

            // Set the spikeball's name.
            trapSpikeBall.name = "APSpikeBallTrap";
            
            // Set the position and x velocity.
            trapSpikeBall.transform.position = position;
            trapSpikeBall.GetComponent<MacerBall>().velocity.x = velocity;

            // Set the explode timer.
            trapSpikeBall.GetComponent<MacerBall>().explodeTimer = Plugin.rng.Next(120, 241);

            // Make the spikeball rebound when hitting the player.
            trapSpikeBall.GetComponent<MacerBall>().reboundOnHit = true;
        }

        /// <summary>
        /// Converts a file path to a URL so that Unity's audio loader can get it, taken from https://github.com/Kuborros/MusicReplacer/blob/master/MusicReplacer/Plugin.cs#L30.
        /// </summary>
        /// <param name="filePath">The path to the file we're wanting to load.</param>
        /// <returns>The "URL" of the file.</returns>
        public static string FilePathToFileUrl(string filePath)
        {
            StringBuilder uri = new();
            foreach (char v in filePath)
            {
                if ((v >= 'a' && v <= 'z') || (v >= 'A' && v <= 'Z') || (v >= '0' && v <= '9') ||
                  v == '+' || v == '/' || v == ':' || v == '.' || v == '-' || v == '_' || v == '~' ||
                  v > '\xFF')
                {
                    uri.Append(v);
                }
                else if (v == Path.DirectorySeparatorChar || v == Path.AltDirectorySeparatorChar)
                {
                    uri.Append('/');
                }
                else
                {
                    uri.Append(string.Format("%{0:X2}", (int)v));
                }
            }
            if (uri.Length >= 2 && uri[0] == '/' && uri[1] == '/') // UNC path
                uri.Insert(0, "file:");
            else
                uri.Insert(0, "file:///");
            return uri.ToString();
        }
    }
}
