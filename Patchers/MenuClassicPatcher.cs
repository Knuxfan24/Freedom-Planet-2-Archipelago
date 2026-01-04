using FP2Lib.Player;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuClassicPatcher
    {
        /// <summary>
        /// The counter used to track how many checks exist in a stage and how many are done.
        /// </summary>
        static GameObject CheckCounter;

        /// <summary>
        /// The sprites of the Archipelago logo for the counter.
        /// </summary>
        static Sprite[] CounterSprites;

        /// <summary>
        /// The counter used to track how many Battlesphere Keys the player has.
        /// </summary>
        static GameObject KeyCounter;

        /// <summary>
        /// The counter used to track how many Enemy Sanity checks exist and how many are done.
        /// </summary>
        static GameObject EnemyCounter;

        /// <summary>
        /// Sets up the classic menu.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "Start")]
        static void MenuSetup(MenuClassic __instance)
        {
            // Randomise the character if we need to.
            if (Plugin.usingRandomCharacter)
            {
                // Pick a character, not including the Random selection.
                int characterIndex = Plugin.rng.Next(MenuConnection.characters.Count - 1);

                // Set the character in the save and in FP2Lib's player handler.
                FPSaveManager.character = (FPCharacterID)MenuConnection.characters.ElementAt(characterIndex).Value;
                PlayerHandler.currentCharacter = PlayerHandler.GetPlayableCharaByFPCharacterId(FPSaveManager.character);

                // Save the files.
                Helpers.Save();
            }

            // Disable the Rail Trap if its active.
            if (Plugin.RailTrap)
                Plugin.RailTrap = false;

            #region Shop Setup
            // Create the array for Milla's shop, setting its length to the amount specified in the slot data then setting the index of each item linerally from 2.
            __instance.itemsForSale = new FPPowerup[(int)(long)Plugin.slotData["milla_shop_amount"]];
            for (int itemIndex = 0; itemIndex < __instance.itemsForSale.Length; itemIndex++)
                __instance.itemsForSale[itemIndex] = (FPPowerup)(itemIndex + 2);

            // Determine how much we need to multiply the location index for Milla's shop by.
            float multiplicationRange = 30 / (float)(long)Plugin.slotData["milla_shop_amount"];

            // Create the array for the Star Card requirements for Milla's shop, setting its length to the amount specified in the slot then setting the value of each item linerally by multiplying its index with multiplaction range and rounding the result up.
            __instance.starCardRequirements = new int[(int)(long)Plugin.slotData["milla_shop_amount"]];
            for (int requirementIndex = 0; requirementIndex < __instance.starCardRequirements.Length; requirementIndex++)
                __instance.starCardRequirements[requirementIndex] = (int)Math.Ceiling(multiplicationRange * (requirementIndex + 1));

            // Create the array for the Vinyl shop, setting its length to 60 then setting the index of each item linerally from 1.
            __instance.musicForSale = new FPMusicTrack[(int)(long)Plugin.slotData["vinyl_shop_amount"]];
            for (int itemIndex = 0; itemIndex < __instance.musicForSale.Length; itemIndex++)
                __instance.musicForSale[itemIndex] = ((FPMusicTrack)(itemIndex + 1));

            // Determine how much we need to multiply the location index for the Vinyl shop by.
            multiplicationRange = 30 / (float)(long)Plugin.slotData["vinyl_shop_amount"];

            // Create the array for the Star Card requirements for the Vinyl shop, setting its length to the amount specified in the slot then setting the value of each item linerally by multiplying its index with multiplaction range and rounding the result up.
            __instance.musicStarCardRequirements = new int[(int)(long)Plugin.slotData["vinyl_shop_amount"]];
            for (int requirementIndex = 0; requirementIndex < __instance.musicStarCardRequirements.Length; requirementIndex++)
                __instance.musicStarCardRequirements[requirementIndex] = (int)Math.Ceiling(multiplicationRange * (requirementIndex + 1));
            #endregion

            #region Stage Locks
            // Set the Star Card requirements for everything past Globe Opera 1 and Gravity Bubble, as only those two stages actually have one.
            __instance.stages[12].starCardRequirement = 11;
            __instance.stages[13].starCardRequirement = 11;
            __instance.stages[14].starCardRequirement = 11;
            __instance.stages[15].starCardRequirement = 11;
            __instance.stages[16].starCardRequirement = 11;
            __instance.stages[17].starCardRequirement = 11;
            __instance.stages[18].starCardRequirement = 11;
            __instance.stages[19].starCardRequirement = 11;
            __instance.stages[20].starCardRequirement = 11;
            __instance.stages[21].starCardRequirement = 11;
            __instance.stages[22].starCardRequirement = 11;
            __instance.stages[24].starCardRequirement = 23;
            __instance.stages[25].starCardRequirement = 23;
            __instance.stages[26].starCardRequirement = 23;
            __instance.stages[27].starCardRequirement = 23;
            __instance.stages[28].starCardRequirement = 23;
            __instance.stages[29].starCardRequirement = 23;
            __instance.stages[32].starCardRequirement = 23;

            // Handle Weapon's Core
            if (Plugin.save.StarCardCount < 32)
            {
                // Set Weapon's Core's panel sprite to the Star Card.
                __instance.stages[30].lockedPanel.gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = Plugin.apAssetBundle.LoadAsset<Sprite>("star_card");
                
                // Set Weapon's Core's Star Card Requirement up to 32.
                __instance.stages[30].starCardRequirement = 32;

                // Remove Weapon's Core's Time Capsule requirement so the tracker shows the Star Cards.
                __instance.stages[30].needsTimeCapsules = false;
            }
            else
            {
                // Set Weapon's Core's panel sprite to the Time Capsule in the asset bundle for consistency.
                __instance.stages[30].lockedPanel.gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = Plugin.apAssetBundle.LoadAsset<Sprite>("time_capsule");
            }

            // Get the lock panel template from the asset bundle.
            GameObject lockTemplate = Plugin.apAssetBundle.LoadAsset<GameObject>("Locked Panel");

            // Set up a list of hardcoded positions to place the lock panels.
            List<Vector3> positions =
            [
                // Prologue
                new(520, -400, 0),
                new(712, -400, 0),

                // Mystery in the Frozen North
                new(712, -232, 0),
                new(904, -232, 0),
                new(1096, -232, 0),
                new(1288, -232, 0),

                // Sky Pirate Panic
                new(904, -400, 0),
                new(1096, -400, 0),

                // Enter The Battlesphere
                new(712, -568, 0),
                new(904, -568, 0),
                new(1096, -568, 0),

                // Globe Opera (Act 1 already has one)
                new(1480, -400, 0),
                new(1672, -400, 0),
                new(1864, -400, 0),
                new(2056, -400, 0),
                
                // Justice in the Sky Paradise
                new(2248, -232, 0),
                new(2440, -232, 0),
                
                // Robot Wars! Snake VS Tarsier
                new(2248, -400, 0),
                new(2440, -400, 0),
                
                // Echoes of the Dragon War
                new(2056, -568, 0),
                new(2248, -568, 0),
                new(2440, -568, 0),
                
                // Bakunawa (Gravity Bubble and Weapon's Core already have one)
                new(2824, -400, 0),
                new(3016, -400, 0),
                new(3208, -400, 0),
                new(3400, -400, 0),
                new(3592, -400, 0),
                new(3784, -400, 0),
                new(3972, -400, 0),
            ];

            // Set up a list of the stage indices that need lock panels.
            List<int> stageIndices =
            [
                // Prologue
                0, 1,

                // Mystery in the Frozen North
                4, 5, 6, 7,
                
                // Sky Pirate Panic
                2, 3,
                
                // Enter The Battlesphere
                8, 9, 10,
                
                // Globe Opera (Act 1 already has one)
                12, 13, 14, 15,
                
                // Justice in the Sky Paradise
                18, 19,
                
                // Robot Wars! Snake VS Tarsier
                16, 17,
                
                // Echoes of the Dragon War
                20, 21, 22,
                
                // Bakunawa (Gravity Bubble and Weapon's Core already have one)
                32, 24, 25, 26, 27, 28, 29
            ];

            // Loop through our stored positions.
            for (int lockPanelIndex = 0; lockPanelIndex < positions.Count; lockPanelIndex++)
            {
                // Create a lock panel and set it to our current position.
                GameObject lockPanel = GameObject.Instantiate(lockTemplate);
                lockPanel.transform.position = positions[lockPanelIndex];

                // Set the lockedPanel and starCardText on the appropriate stage entry.
                __instance.stages[stageIndices[lockPanelIndex]].lockedPanel = lockPanel.GetComponent<SpriteRenderer>();
                __instance.stages[stageIndices[lockPanelIndex]].starCardText = lockPanel.transform.GetChild(0).GetComponent<MenuText>();
            }
            
            // If the chapter setup is set to open, then remove all the Star Card locks (minus Weapon's Core's).
            if ((long)Plugin.slotData["chapters"] == 2)
                for (int stageIndex = 0;  stageIndex < __instance.stages.Length; stageIndex++)
                    if (stageIndex != 30)
                        __instance.stages[stageIndex].starCardRequirement = 0;
            #endregion

            #region Counters
            // Kill the Star Card and Vinyl sprites (we leave the Chest one intact for Chest Tracers).
            for (int spriteIndex = 0; spriteIndex <= 3; spriteIndex++)
                __instance.hudCollectibleSprites[spriteIndex] = null;

            // If the Chest Tracers are disabled, then kill them as well.
            if ((long)Plugin.slotData["chest_tracers"] == 0)
            {
                __instance.hudCollectibleSprites[4] = null;
                __instance.hudCollectibleSprites[5] = null;
            }

            // Shift the Chest icon to the right to align it better with the Check Counter.
            __instance.hudCollectibles[2].gameObject.transform.localPosition = new(128, 1, 0);

            // Create the counters.
            CheckCounter = GameObject.Instantiate(Plugin.apAssetBundle.LoadAsset<GameObject>("Check Counter"));
            KeyCounter = GameObject.Instantiate(Plugin.apAssetBundle.LoadAsset<GameObject>("Key Counter"));
            EnemyCounter = GameObject.Instantiate(Plugin.apAssetBundle.LoadAsset<GameObject>("Enemy Counter"));

            // Load the Archipelago logo sprites.
            CounterSprites = Plugin.apAssetBundle.LoadAssetWithSubAssets<Sprite>("hud_ap");

            // Hide the key counter.
            KeyCounter.gameObject.SetActive(false);
            #endregion

            // Remove the two tutorial panels.
            __instance.stages[31].icon.gameObject.SetActive(false);
            __instance.stages[33].icon.gameObject.SetActive(false);

            // Remove any custom stage panels.
            if (__instance.stages.Length > 34)
                for (int extraStageIndex = 34; extraStageIndex < __instance.stages.Length; extraStageIndex++)
                    __instance.stages[extraStageIndex].icon.gameObject.SetActive(false);
        }

        /// <summary>
        /// Handles modifying or removing the lock panels.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "UpdateIcons")]
        static bool ManageLockPanels(MenuClassic __instance)
        {
            // Loop through the stage list.
            for (int stageIndex = 0; stageIndex < __instance.stages.Length; stageIndex++)
            {
                // Check that this stage actually has a lock panel.
                if (__instance.stages[stageIndex].starCardText != null)
                {
                    // Check that this stage needs Star Cards to unlock.
                    if (!__instance.stages[stageIndex].needsTimeCapsules)
                    {
                        // If we don't have enough Star Cards, then update the text on the panel.
                        // We also check that the Star Card requirement isn't set to 999, as that is set specifically as a hack to make the chapter locks work.
                        if (FPSaveManager.TotalStarCards() < __instance.stages[stageIndex].starCardRequirement && __instance.stages[stageIndex].starCardRequirement != 999)
                        {
                            __instance.stages[stageIndex].starCardText.GetComponent<TextMesh>().text = FPSaveManager.TotalStarCards() + " / " + __instance.stages[stageIndex].starCardRequirement;  
                        }

                        // If we do have enough Star Cards, then handle the chapter locking.
                        else
                        {
                            // Determine if this panel needs removing based on the stage index and chapter unlocks.
                            switch (stageIndex)
                            {
                                case 0: if (Plugin.save.StageUnlocks[0]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 1: if (Plugin.save.StageUnlocks[1]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;

                                case 4: if (Plugin.save.StageUnlocks[2]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 5: if (Plugin.save.StageUnlocks[3]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 6: if (Plugin.save.StageUnlocks[4]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 7: if (Plugin.save.StageUnlocks[5]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;

                                case 2: if (Plugin.save.StageUnlocks[6]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 3: if (Plugin.save.StageUnlocks[7]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;

                                case 8: if (Plugin.save.StageUnlocks[8]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 9: if (Plugin.save.StageUnlocks[9]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 10: if (Plugin.save.StageUnlocks[10]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;

                                case 11: if (Plugin.save.StageUnlocks[11]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 12: if (Plugin.save.StageUnlocks[12]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 13: if (Plugin.save.StageUnlocks[13]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 14: if (Plugin.save.StageUnlocks[14]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 15: if (Plugin.save.StageUnlocks[15]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;

                                case 18: if (Plugin.save.StageUnlocks[16]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 19: if (Plugin.save.StageUnlocks[17]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;

                                case 16: if (Plugin.save.StageUnlocks[18]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 17: if (Plugin.save.StageUnlocks[19]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;

                                case 20: if (Plugin.save.StageUnlocks[20]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 21: if (Plugin.save.StageUnlocks[21]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 22: if (Plugin.save.StageUnlocks[22]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;

                                case 23: if (Plugin.save.StageUnlocks[23]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 32: if (Plugin.save.StageUnlocks[24]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 24: if (Plugin.save.StageUnlocks[25]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 25: if (Plugin.save.StageUnlocks[26]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 26: if (Plugin.save.StageUnlocks[27]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 27: if (Plugin.save.StageUnlocks[28]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 28: if (Plugin.save.StageUnlocks[29]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;
                                case 29: if (Plugin.save.StageUnlocks[30]) __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false); break;

                                // Weapon's Core works differently, as it needs Time Capsules as well.
                                case 30:
                                    // Set the Star Card requirement down to 13.
                                    __instance.stages[stageIndex].starCardRequirement = 13;

                                    // Reenable the Time Capsule requirement.
                                    __instance.stages[stageIndex].needsTimeCapsules = true;

                                    // Change the Star Card sprite to the Time Capsule one.
                                    __instance.stages[stageIndex].lockedPanel.gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = Plugin.apAssetBundle.LoadAsset<Sprite>("time_capsule");

                                    break;
                            }

                            // Double check for the Time Capsule requirement, so we don't accidentally edit Weapon's Core's panel further.
                            if (__instance.stages[stageIndex].needsTimeCapsules)
                                continue;

                            // If the panel is still active (thus the chapter is locked), then set the Star Card requirement to 999.
                            if (__instance.stages[stageIndex].lockedPanel.gameObject.activeSelf)
                                __instance.stages[stageIndex].starCardRequirement = 999;

                            // If it isn't active, then set it to 0 to force unlock it.
                            else
                                __instance.stages[stageIndex].starCardRequirement = 0;

                            // Shift the text over to the left.
                            __instance.stages[stageIndex].starCardText.gameObject.transform.localPosition = Vector3.zero;

                            // Display the word "Chapter" or "Access" depending on the chapter mode.
                            if ((long)Plugin.slotData["chapters"] != 2)
                                __instance.stages[stageIndex].starCardText.GetComponent<TextMesh>().text = "Chapter";
                            else
                                __instance.stages[stageIndex].starCardText.GetComponent<TextMesh>().text = "Access";

                            // Get the Star Card Icon for this stage.
                            GameObject icon = __instance.stages[stageIndex].lockedPanel.gameObject.transform.GetChild(1).gameObject;

                            // Globe Opera 1's has the icon and text backwards, so factor that in if we haven't got the right object.
                            if (icon.name != "StarCard_Icon" && icon.name != "Capsule_Icon")
                                icon = __instance.stages[stageIndex].lockedPanel.gameObject.transform.GetChild(0).gameObject;

                            // Kill the icon.
                            icon.SetActive(false);
                        }
                    }

                    // Handle Weapon's Core's Time Capsule requirement.
                    else
                    {
                        // Either update the text on the panel if we don't have enough Time Capsules.
                        if (FPSaveManager.TotalLogs() < __instance.stages[stageIndex].starCardRequirement && __instance.stages[stageIndex].starCardRequirement != 999)
                        {
                            __instance.stages[stageIndex].starCardText.GetComponent<TextMesh>().text = FPSaveManager.TotalLogs() + " / 13";
                        }

                        // If we do have enough Time Capsules and the chapter is unlocked, then disable the lock.
                        else if (Plugin.save.StageUnlocks[31])
                        {
                            __instance.stages[stageIndex].starCardRequirement = 0;
                            __instance.stages[stageIndex].lockedPanel.gameObject.SetActive(false);
                        }

                        // If we do have enough Time Capsules, but the chapter is locked, then display the Chapter tag.
                        else
                        {
                            // Set the Time Capsule requirement up to 999.
                            __instance.stages[stageIndex].starCardRequirement = 999;

                            // Shift the text over to the left and display the word "Chapter".
                            __instance.stages[stageIndex].starCardText.gameObject.transform.localPosition = Vector3.zero;
                            __instance.stages[stageIndex].starCardText.GetComponent<TextMesh>().text = "Chapter";

                            // Get the Time Capsule icon and kill it.
                            __instance.stages[stageIndex].lockedPanel.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        }

                    }
                }
            }

            // Stop the original lock panel handler from running.
            return false;
        }

        /// <summary>
        /// Handles setting the sprite on the Chest icon depending on if a stage's Chest Tracer is received or not.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuClassic), "UpdateHeader")]
        static void ChestTracers(MenuClassic __instance, ref int ___currentTile)
        {
            // If this stage is locked or we just don't have chests enabled, then null out the sprite and return.
            if (__instance.stages[___currentTile].starCardRequirement > 0 || (long)Plugin.slotData["chests"] == 0)
            {
                __instance.hudCollectibles[2].sprite = null;
                return;
            }

            // Set up a value to determine if we have this stage's tracer.
            bool hasTracer;

            // Set hasTracer depending on the selected tile's index. If its not one that has a tracer at all, then null the sprite out.
            switch (___currentTile)
            {
                case 0: hasTracer = Plugin.save.ChestTracers[0]; break;
                case 1: hasTracer = Plugin.save.ChestTracers[1]; break;
                case 2: hasTracer = Plugin.save.ChestTracers[2]; break;
                case 3: hasTracer = Plugin.save.ChestTracers[3]; break;
                case 4: hasTracer = Plugin.save.ChestTracers[4]; break;
                case 5: hasTracer = Plugin.save.ChestTracers[5]; break;
                case 6: hasTracer = Plugin.save.ChestTracers[6]; break;
                case 8: hasTracer = Plugin.save.ChestTracers[7]; break;
                case 9: hasTracer = Plugin.save.ChestTracers[8]; break;
                case 11: hasTracer = Plugin.save.ChestTracers[9]; break;
                case 12: hasTracer = Plugin.save.ChestTracers[10]; break;
                case 14: hasTracer = Plugin.save.ChestTracers[11]; break;
                case 15: hasTracer = Plugin.save.ChestTracers[12]; break;
                case 16: hasTracer = Plugin.save.ChestTracers[13]; break;
                case 17: hasTracer = Plugin.save.ChestTracers[14]; break;
                case 18: hasTracer = Plugin.save.ChestTracers[15]; break;
                case 19: hasTracer = Plugin.save.ChestTracers[16]; break;
                case 20: hasTracer = Plugin.save.ChestTracers[17]; break;
                case 21: hasTracer = Plugin.save.ChestTracers[18]; break;
                case 23: hasTracer = Plugin.save.ChestTracers[19]; break;
                case 24: hasTracer = Plugin.save.ChestTracers[20]; break;
                case 26: hasTracer = Plugin.save.ChestTracers[21]; break;
                case 27: hasTracer = Plugin.save.ChestTracers[22]; break;
                case 28: hasTracer = Plugin.save.ChestTracers[23]; break;
                default: __instance.hudCollectibles[2].sprite = null; return;
            }

            // Select the correct chest sprite depending on whether or not we have this tile's tracer.
            if (hasTracer)
                __instance.hudCollectibles[2].sprite = __instance.hudCollectibleSprites[5];
            else
                __instance.hudCollectibles[2].sprite = __instance.hudCollectibleSprites[4];
        }

        /// <summary>
        /// Handle showing and updating the counters.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "UpdateIcons")]
        static void UpdateCheckIndicator(MenuClassic __instance, ref int ___currentTile)
        {
            // Set up values to track the total check count for this stage, as well as the total we have.
            int checkCount = 0;
            int completedCheckCount = 0;

            // Get the locations for the selected stage.
            switch (___currentTile)
            {
                case 0: GetLocations("Dragon Valley"); break;
                case 1: GetLocations("Shenlin Park"); break;
                case 2: GetLocations("Avian Museum"); break;
                case 3: GetLocations("Airship Sigwada"); break;
                case 4: GetLocations("Tiger Falls"); break;
                case 5: GetLocations("Robot Graveyard"); break;
                case 6: GetLocations("Shade Armory"); break;
                case 7: GetLocations("Snowfields"); break;
                case 8: GetLocations("Phoenix Highway"); break;
                case 9: GetLocations("Zao Land"); break;
                case 10: GetLocations("The Battlesphere"); break;
                case 11: GetLocations("Globe Opera 1"); break;
                case 12: GetLocations("Globe Opera 2"); break;
                case 13: GetLocations("Auditorium"); break;
                case 14: GetLocations("Palace Courtyard"); break;
                case 15: GetLocations("Tidal Gate"); break;
                case 16: GetLocations("Zulon Jungle"); break;
                case 17: GetLocations("Nalao Lake"); break;
                case 18: GetLocations("Sky Bridge"); break;
                case 19: GetLocations("Lightning Tower"); break;
                case 20: GetLocations("Ancestral Forge"); break;
                case 21: GetLocations("Magma Starscape"); break;
                case 22: GetLocations("Diamond Point"); break;
                case 23: GetLocations("Gravity Bubble"); break;
                case 24: GetLocations("Bakunawa Rush"); break;
                case 25: GetLocations("Refinery Room"); break;
                case 26: GetLocations("Clockwork Arboretum"); break;
                case 27: GetLocations("Inversion Dynamo"); break;
                case 28: GetLocations("Lunar Cannon"); break;
                case 29: GetLocations("Merga"); break;
                case 30: GetLocations("Weapon's Core"); break;
                case 32: GetLocations("Bakunawa Chase"); break;
                default: CheckCounter.gameObject.SetActive(false); KeyCounter.gameObject.SetActive(false); return;
            }

            // If we've completed all of this stage's checks, then fill the AP logo in.
            if (completedCheckCount == checkCount)
                CheckCounter.GetComponent<SpriteRenderer>().sprite = CounterSprites[1];
            else
                CheckCounter.GetComponent<SpriteRenderer>().sprite = CounterSprites[0];

            // Update the text on the counter.
            CheckCounter.transform.GetChild(0).GetComponent<TextMesh>().text = $"{completedCheckCount} / {checkCount}";

            // If our selected stage is locked, then just hide the counter entirely.
            if (__instance.stages[___currentTile].starCardRequirement != 0)
            {
                CheckCounter.gameObject.SetActive(false);
                KeyCounter.gameObject.SetActive(false);
                EnemyCounter.gameObject.SetActive(false);
                return;
            }

            // If no checks exist for the highlighted stage, then just hide the counter entirely.
            if (checkCount == 0)
                CheckCounter.gameObject.SetActive(false);
            else
                CheckCounter.gameObject.SetActive(true);

            // Check if we need to show the enemy counter.
            if ((long)Plugin.slotData["enemies"] == 1 || (long)Plugin.slotData["bosses"] == 1)
            {
                // Show the counter.
                EnemyCounter.gameObject.SetActive(true);

                // Set up values to calculate how many enemy checks exist and how many are done.
                int enemyCount = 0;
                int defeatedEnemyCount = 0;

                // Get the values for the enemies.
                if ((long)Plugin.slotData["enemies"] == 1)
                {
                    EnemyLocations("Aqua Trooper");
                    EnemyLocations("Corrupted Aqua Trooper");
                    EnemyLocations("Beartle");
                    EnemyLocations("Corrupted Beartle");
                    EnemyLocations("Blast Cone");
                    EnemyLocations("Bonecrawler");
                    EnemyLocations("Bonespitter");
                    EnemyLocations("Boom Beth");
                    EnemyLocations("Bubblorbiter");
                    EnemyLocations("Burro");
                    EnemyLocations("Cocoon");
                    EnemyLocations("Cow Horn");
                    EnemyLocations("Crowitzer");
                    EnemyLocations("Crustaceon");
                    EnemyLocations("Dart Hog");
                    EnemyLocations("Dino Walker");
                    EnemyLocations("Corrupted Dino Walker");
                    EnemyLocations("Drake Fly");
                    EnemyLocations("Corrupted Drake Fly");
                    EnemyLocations("Droplet Ship");
                    EnemyLocations("Durugin");
                    EnemyLocations("Fire Hopper");
                    EnemyLocations("Flamingo");
                    EnemyLocations("Flash Mouth");
                    EnemyLocations("Corrupted Flash Mouth");
                    EnemyLocations("Flying Saucer");
                    EnemyLocations("Folding Snake");
                    EnemyLocations("Gat Hog");
                    EnemyLocations("Girder");
                    EnemyLocations("Hellpo");
                    EnemyLocations("Hijacked Police Car");
                    EnemyLocations("Hot Plate");
                    EnemyLocations("Iris");
                    EnemyLocations("Jawdrop");
                    EnemyLocations("Keon");
                    EnemyLocations("Koi Cannon");
                    EnemyLocations("Line Cutter");
                    EnemyLocations("Macer");
                    EnemyLocations("Manpowa");
                    EnemyLocations("Mantis");
                    EnemyLocations("Meteor Roller");
                    EnemyLocations("Peller");
                    EnemyLocations("Pendurum");
                    EnemyLocations("Corrupted Pendurum");
                    EnemyLocations("Pogo Snail");
                    EnemyLocations("Prawn");
                    EnemyLocations("Prawn To Be Wild");
                    EnemyLocations("Raytracker");
                    EnemyLocations("Corrupted Raytracker");
                    EnemyLocations("Rifle Trooper");
                    EnemyLocations("Saw Shrimp");
                    EnemyLocations("Sentinel");
                    EnemyLocations("Shockula");
                    EnemyLocations("Softballer");
                    EnemyLocations("Spy Turretus");
                    EnemyLocations("Corrupted Spy Turretus");
                    EnemyLocations("Stahp");
                    EnemyLocations("Sword Trooper");
                    EnemyLocations("Sword Wing");
                    EnemyLocations("Tombstone Turretus");
                    EnemyLocations("Torcher");
                    EnemyLocations("Tower Cannon");
                    EnemyLocations("Toy Decoy");
                    EnemyLocations("Traumagotcha");
                    EnemyLocations("Corrupted Traumagotcha");
                    EnemyLocations("Troopish");
                    EnemyLocations("Turretus");
                    EnemyLocations("Corrupted Turretus");
                    EnemyLocations("Water Hopper");
                    EnemyLocations("Wood Hopper");
                    EnemyLocations("Zombie Trooper");
                }

                // Get the values for the bosses.
                if ((long)Plugin.slotData["bosses"] == 1)
                {
                    EnemyLocations("Acrabelle");
                    EnemyLocations("Askal");
                    EnemyLocations("Astral Golmech (Aaa)");
                    EnemyLocations("Astral Golmech (Askal)");
                    EnemyLocations("Beast One");
                    EnemyLocations("Beast Two");
                    EnemyLocations("Beast Three");
                    EnemyLocations("BFF2000");
                    EnemyLocations("Captain Kalaw");
                    EnemyLocations("Carol");
                    EnemyLocations("Corazon");
                    EnemyLocations("Crabulon");
                    EnemyLocations("Discord");
                    EnemyLocations("Drake Cocoon");
                    EnemyLocations("Duality");
                    EnemyLocations("General Gong");
                    EnemyLocations("Gnawsa Lock");
                    EnemyLocations("Herald");
                    EnemyLocations("Hundred Drillion");
                    EnemyLocations("Kakugan");
                    EnemyLocations("Lemon Bread");
                    EnemyLocations("Lilac");
                    EnemyLocations("Merga (Blue Moon)");
                    EnemyLocations("Merga (Blood Moon)");
                    EnemyLocations("Merga (Super Moon)");
                    EnemyLocations("Merga (Eclipse)");
                    EnemyLocations("Merga (Lilith)");
                    EnemyLocations("Merga");
                    EnemyLocations("Milla");
                    EnemyLocations("Monster Cube");
                    EnemyLocations("Neera");
                    EnemyLocations("Proto Pincer");
                    EnemyLocations("Rail Driver");
                    EnemyLocations("Rosebud");
                    EnemyLocations("Serpentine");
                    EnemyLocations("Shell Growth");
                    EnemyLocations("Storm Slider");
                    EnemyLocations("Syntax Spider");
                    EnemyLocations("Titan Armor");
                    EnemyLocations("Trigger Joy");
                    EnemyLocations("Trigger Lancer");
                    EnemyLocations("Tunnel Driver");
                    EnemyLocations("Weather Face");
                    EnemyLocations("Wolf Armour");
                }

                // Set the text on the counter.
                EnemyCounter.transform.GetChild(0).GetComponent<TextMesh>().text = $"{defeatedEnemyCount} / {enemyCount}";

                void EnemyLocations(string locationName)
                {
                    // Get the index of the location for this enemy.
                    long locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", locationName);

                    // Check if this location exists.
                    if (Helpers.CheckLocationExists(locationIndex))
                    {
                        // Increment the enemy count.
                        enemyCount++;

                        // If this location is already checked, then also increment the defeated enemy count.
                        if (Plugin.session.Locations.AllLocationsChecked.Contains(locationIndex))
                            defeatedEnemyCount++;
                    }
                }
            }

            void GetLocations(string stageName)
            {
                // Set up a value to hold our location indices.
                #pragma warning disable IDE0059 // Unnecessary assignment of a value, this actually DOES need to start as -1.
                long locationIndex = -1;
                #pragma warning restore IDE0059

                // Check if the highlighted stage is the Battlesphere, as we handle that differently.
                if (stageName == "The Battlesphere")
                {
                    // Get and add the locations for each of the Battlesphere challenges.
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Beginner's Gauntlet"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Battlebot Battle Royale"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Hero Battle Royale"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Kalaw's Challenge"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Army of One"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Ring-Out Challenge"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Flip Fire Gauntlet"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Vanishing Maze"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Mondo Condo"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Birds of Prey"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Battlebot Revenge"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Mach Speed Melee"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Galactic Rumble"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Stop and Go"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Mecha Madness"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Rolling Thunder"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Blast from the Past"); AddLocation();
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", "Bubble Battle"); AddLocation();

                    // Enable the key counter and update its text.
                    KeyCounter.gameObject.SetActive(true);
                    KeyCounter.transform.GetChild(0).GetComponent<TextMesh>().text = $"{Plugin.save.BattlesphereKeyCount} / 18";

                    // Stop here.
                    return;
                }

                // Hide the key counter.
                KeyCounter.gameObject.SetActive(false);

                // Get the location index for this stage's clear check.
                locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", $"{stageName} - Clear");
                AddLocation();

                // Loop through for the chests (we do up to 8 because Tidal Gate has that many).
                for (int chestIndex = 0; chestIndex <= 8; chestIndex++)
                {
                    // Get the location index for this chest index.
                    locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", $"{stageName} - Chest {chestIndex}");

                    // If this index is 0, then check for Chest without a number (for stages with only a single chest).
                    if (chestIndex == 0)
                        locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", $"{stageName} - Chest");

                    AddLocation();
                }

                void AddLocation()
                {
                    // Check that this location exists in the multiworld.
                    if (Helpers.CheckLocationExists(locationIndex))
                    {
                        // Increment our check count.
                        checkCount++;

                        // If this location has already been checked, then increment the completed check count too.
                        if (Plugin.session.Locations.AllLocationsChecked.Contains(locationIndex))
                            completedCheckCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Hides the counters if a menu is up.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuClassic), "State_WaitForMenu")]
        static void HideCountersInMenu(ref GameObject ___targetMenu)
        {
            if (___targetMenu != null)
            {
                CheckCounter.gameObject.SetActive(false);
                KeyCounter.gameObject.SetActive(false);
                EnemyCounter.gameObject.SetActive(false);
            }
            else
            {
                CheckCounter.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Removes the code that causes the Time Capsule animation to play after Tidal Gate.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuClassic), "State_Intro")]
        static IEnumerable<CodeInstruction> RemoveTidalGateTimeCapsule(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int codeIndex = 10; codeIndex <= 45; codeIndex++)
                codes[codeIndex].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }
    }
}
