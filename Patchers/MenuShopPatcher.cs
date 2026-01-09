namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuShopPatcher
    {
        /// <summary>
        /// The sprites for the items in this shop.
        /// </summary>
        public static Sprite[] Sprites;

        /// <summary>
        /// The locations that have been scouted for by this shop.
        /// </summary>
        public static Dictionary<long, ScoutedItemInfo> _ScoutedLocationInfo = [];

        /// <summary>
        /// The sprite for the selected item.
        /// </summary>
        public static Sprite SelectedItemSprite;

        /// <summary>
        /// The name for the selected item.
        /// </summary>
        public static string SelectedItemName;

        /// <summary>
        /// Stops the shop menu from sorting purchased items to the end of the list.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuShop), "SortItems")]
        static bool StopShopSorting() => false;

        /// <summary>
        /// Sets the shop prices to the value chosen in the player YAML and sets up both item arrays.
        /// TODO: High location amounts seem to have the last item break. Figure out
            /// A: Why that happens.
            /// B: What the limit is.
        /// and fix them.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuShop), "Start")]
        static void SetShopPrices(ref bool ___payWithCrystals, ref int[] ___itemCosts, ref FPPowerup[] ___itemsForSale, ref FPMusicTrack[] ___musicID)
        {
            if (___payWithCrystals)
            {
                ___itemCosts = new int[(int)(long)Plugin.slotData["vinyl_shop_amount"]];
                for (int costIndex = 0; costIndex < ___itemCosts.Length; costIndex++)
                    ___itemCosts[costIndex] = (int)(long)Plugin.slotData["vinyl_shop_price"];

                ___itemsForSale = new FPPowerup[(int)(long)Plugin.slotData["vinyl_shop_amount"]];
            }

            else
            {
                ___itemCosts = new int[(int)(long)Plugin.slotData["milla_shop_amount"]];

                for (int costIndex = 0; costIndex < ___itemCosts.Length; costIndex++)
                    ___itemCosts[costIndex] = (int)(long)Plugin.slotData["milla_shop_price"];

                ___musicID = new FPMusicTrack[(int)(long)Plugin.slotData["milla_shop_amount"]];
            }
        }

        /// <summary>
        /// Gets the sprites for this shop.
        /// </summary>
        /// <param name="___payWithCrystals">Whether or not this shop pays with crystals (and thus is the vinyl shop).</param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuShop), "Start")]
        static void GetShopSprites(ref bool ___payWithCrystals)
        {
            // Set up a dictionary of scouted locations
            _ScoutedLocationInfo = [];

            // Set up a list of location indices.
            List<long> locationIDs = [];

            // Get the data depending on the shop type.
            if (!___payWithCrystals)
                GatherSpritesAndLocations((int)(long)Plugin.slotData["milla_shop_amount"], "Milla");
            else
                GatherSpritesAndLocations((int)(long)Plugin.slotData["vinyl_shop_amount"], "Vinyl");

            void GatherSpritesAndLocations(int itemCount, string shop)
            {
                // Loop through and get the location indices for this shop.
                for (int itemIndex = 1; itemIndex <= itemCount; itemIndex++)
                    locationIDs.Add(Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", $"{shop} Shop Item {itemIndex}"));

                // Scout the locations for this shop.
                Plugin.session.Locations.ScoutLocationsAsync(HandleScoutInfo, [.. locationIDs]);

                // Wait for the scout to finish before continuing.
                while (_ScoutedLocationInfo.Count < itemCount)
                    Thread.Sleep(1);

                // Get the sprites for the items in this shop.
                List<Sprite> sprites = [];
                for (int spriteIndex = 0; spriteIndex < itemCount; spriteIndex++)
                    sprites.Add(Helpers.GetItemSprite(_ScoutedLocationInfo.ElementAt(spriteIndex).Value, true));
                Sprites = [.. sprites];

                // If our shop information setting is set to full and the shop hints are enabled, then also send them.
                if ((long)Plugin.slotData["shop_information"] == 0 && Plugin.configShopHints.Value > 0)
                {
                    // Reset the location ID lost.
                    locationIDs = [];

                    // Calculate how many items are valid hints.
                    // TODO: Redo this setup, as its hardcoded to 30 and 60.
                    int hintableItems = FPSaveManager.TotalStarCards();
                    if (shop == "Vinyl")
                        hintableItems *= 2;

                    // Loop through and get the location indices for this shop's hints.
                    for (int hintIndex = 1; hintIndex <= hintableItems; hintIndex++)
                        locationIDs.Add(Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", $"{shop} Shop Item {hintIndex}"));

                    // Remove any location IDs that have already been checked (used to stop the game from repeatadly sending hints for items that are unlocked and brought in the same batch).
                    for (int locationIndex = locationIDs.Count - 1; locationIndex >= 0; locationIndex--)
                        if (Plugin.session.Locations.AllLocationsChecked.Contains(locationIDs[locationIndex]))
                            locationIDs.RemoveAt(locationIndex);

                    // If we're only sending progression item hints, then loop through the previous scout and remove ones without the Advancement flag.
                    if (Plugin.configShopHints.Value == 1)
                        foreach (KeyValuePair<long, ScoutedItemInfo> location in _ScoutedLocationInfo)
                            if (location.Value.Flags != Archipelago.MultiClient.Net.Enums.ItemFlags.Advancement)
                                locationIDs.Remove(location.Key);

                    // Scout for the hints for these locations.
                    Plugin.session.Locations.ScoutLocationsAsync(HandleScoutInfoHint, Archipelago.MultiClient.Net.Enums.HintCreationPolicy.CreateAndAnnounceOnce, [.. locationIDs]);
                }
            }

            void HandleScoutInfo(Dictionary<long, ScoutedItemInfo> scoutedLocationInfo) => _ScoutedLocationInfo = scoutedLocationInfo;
            void HandleScoutInfoHint(Dictionary<long, ScoutedItemInfo> dummy) { };
        }

        /// <summary>
        /// Set the sprites, names and descriptions for items in this shop.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuShop), "UpdateItemList")]
        static void SetShopVisuals(ref int ___currentDetail, ref int ___detailListOffset, ref bool ___payWithCrystals, ref FPHudDigit[] ___powerups,
                                   ref FPHudDigit[] ___vinyls, ref MenuText[] ___detailName, ref MenuText ___itemDescription)
        {
            // Calculate the highlighted item.
            int selectedItem = ___currentDetail + ___detailListOffset;

            // Handle replacing the sprite on the item depending on the shop type.
            if (!___payWithCrystals)
            {
                for (int powerupIndex = 0; powerupIndex < ___powerups.Length; powerupIndex++)
                    if (___powerups[powerupIndex].digitValue > 1 && (powerupIndex + ___detailListOffset) < (int)(long)Plugin.slotData["milla_shop_amount"])
                        ___powerups[powerupIndex].GetComponent<SpriteRenderer>().sprite = Sprites[powerupIndex + ___detailListOffset];
            }
            else
            {
                for (int vinylIndex = 0; vinylIndex < ___vinyls.Length; vinylIndex++)
                    if (___vinyls[vinylIndex].digitValue != 0 && (vinylIndex + ___detailListOffset) < (int)(long)Plugin.slotData["vinyl_shop_amount"])
                        ___vinyls[vinylIndex].GetComponent<SpriteRenderer>().sprite = Sprites[vinylIndex + ___detailListOffset];
            }

            // Only replace the item name and description if it's unlocked.
            if (___detailName[0].GetComponent<TextMesh>().text != "? ? ? ? ?")
            {
                // Don't try and replace the item name and description if it would end up out of bounds (likely because of FP2Lib adding Vinyls from other mods).
                if ((___payWithCrystals && selectedItem >= (int)(long)Plugin.slotData["vinyl_shop_amount"]) || (!___payWithCrystals && selectedItem >= (int)(long)Plugin.slotData["milla_shop_amount"]))
                    return;

                // Get the location for this item.
                ScoutedItemInfo location = _ScoutedLocationInfo.ElementAt(selectedItem).Value;

                // Store the selected item's sprite and name for the ItemGet menu.
                SelectedItemSprite = Sprites[selectedItem];
                SelectedItemName = GetItemName(location);

                // Replace the item name.
                ___detailName[0].GetComponent<TextMesh>().text = SelectedItemName;

                // Replace the item description based on whether its for us or another player.
                if (_ScoutedLocationInfo.ElementAt(selectedItem).Value.Player.Name != Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot))
                    ___itemDescription.GetComponent<TextMesh>().text = FPStage.WrapText(GetItemDescription(location, $"An item for {location.Player.Name}'s {location.ItemGame}."), 40);
                else
                    ___itemDescription.GetComponent<TextMesh>().text = FPStage.WrapText(GetItemDescription(location, "An item for you."), 40);

            }

            static string GetItemName(ScoutedItemInfo location)
            {
                // Determine who this item is for.
                string itemTarget = $"{location.Player}'s ";
                if (location.Player.Name == Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot))
                    itemTarget = "";

                // Determine the progression level of this item.
                string itemType = "";
                if (location.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Advancement) itemType = "Progression ";
                if (location.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Trap) itemType = "Trap ";

                // Return the right string for the Show Item Names in Shops setting.
                switch ((long)Plugin.slotData["shop_information"])
                {
                    default: return $"{itemTarget}{location.ItemName}";
                    case 1: return $"{itemTarget}{itemType}Item";
                    case 2: return $"{itemTarget}Item";
                    case 3: return "Item";
                }
            }

            static string GetItemDescription(ScoutedItemInfo location, string defaultDescription)
            {
                // Determine who this item is for.
                string itemTarget = $"{location.Player.Name}'s ";
                if (location.Player.Name == Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot))
                    itemTarget = "you";

                // If the shop is set to hide information, then determine what we need to return.
                switch ((long)Plugin.slotData["shop_information"])
                {
                    case 1:
                        if (location.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Advancement)
                            return $"A progression item for {itemTarget}.";
                        else if (location.Flags == Archipelago.MultiClient.Net.Enums.ItemFlags.Trap)
                            return $"A trap item for {itemTarget}.";
                        else
                            return $"An item for {itemTarget}.";

                    case 2:
                        return $"An item for {itemTarget}.";

                    case 3:
                        return $"An item for somebody.";
                }

                // Check if an items file exists for the game this item is for.
                if (File.Exists($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.ItemGame}\items.json"))
                {
                    // Load the item.json file.
                    ItemDescriptor[] itemDescriptors = JsonConvert.DeserializeObject<ItemDescriptor[]>(File.ReadAllText($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Sprites\{location.ItemGame}\items.json"));

                    // Loop through each item descriptor in the json file to find one with this item's name. If we find one, then return its description, if it has one.
                    foreach (ItemDescriptor item in itemDescriptors)
                        if (item.ItemNames.Contains(location.ItemName))
                            if (item.Description != null)
                                if (item.Description != string.Empty)
                                    return item.Description;
                }

                // Check if this item is for Freedom Planet 2.
                // If so, then either read the game's own descriptions, or read my own.
                if (location.ItemGame == "Freedom Planet 2")
                {
                    switch (location.ItemName)
                    {
                        // Key Items.
                        case "Star Card": return "Keys for unlocking distant lands.";
                        case "Time Capsule": return FPSaveManager.GetCollectableDescription(7);
                        case "Battlesphere Key": return "Unlocks a challenge in The Battlesphere.";

                        // Filler Items.
                        case "Gold Gem": return FPSaveManager.GetCollectableDescription(1);
                        case "Crystals": return FPSaveManager.GetCollectableDescription(0);
                        case "Extra Life": return "Grants an additional stock.";
                        case "Invincibility": return "Grants temporary invincibility.";
                        case "Wood Shield": return "Grants a Wood Shield.";
                        case "Earth Shield": return "Grants an Earth Shield.";
                        case "Water Shield": return "Grants a Water Shield.";
                        case "Fire Shield": return "Grants a Fire Shield.";
                        case "Metal Shield": return "Grants a Metal Shield.";

                        // Powerup, done seperately as we swap it out depending on the player character.
                        case "Powerup":
                            switch (FPSaveManager.character)
                            {
                                case FPCharacterID.LILAC: return "Grants an Energizer Sphere";
                                case FPCharacterID.CAROL: case FPCharacterID.BIKECAROL: return "Grants a Fuel Tank";
                                case FPCharacterID.MILLA: return "Grants a Multi Cube";
                                case FPCharacterID.NEERA: return "Grants Speed Skates";
                                default: return "Grants your character's unique powerup";
                            }

                        // Extra Item Slots.
                        case "Extra Item Slot": return "Allows equipping an extra Brave Stone.";
                        case "Extra Potion Slot": return "Allows equipping some extra Potions.";

                        // Potions.
                        case "Potion - Extra Stock": return FPSaveManager.GetItemDescription(FPPowerup.EXTRA_STOCK);
                        case "Potion - Strong Revivals": return FPSaveManager.GetItemDescription(FPPowerup.STRONG_REVIVALS);
                        case "Potion - Cheaper Stocks": return FPSaveManager.GetItemDescription(FPPowerup.CHEAPER_STOCKS);
                        case "Potion - Healing Strike": return FPSaveManager.GetItemDescription(FPPowerup.REGENERATION);
                        case "Potion - Attack Up": return FPSaveManager.GetItemDescription(FPPowerup.ATTACK_UP);
                        case "Potion - Strong Shields": return FPSaveManager.GetItemDescription(FPPowerup.STRONG_SHIELDS);
                        case "Potion - Accelerator": return FPSaveManager.GetItemDescription(FPPowerup.SPEED_UP);
                        case "Potion - Super Feather": return FPSaveManager.GetItemDescription(FPPowerup.JUMP_UP);

                        // Brave Stones.
                        case "Element Burst": return FPSaveManager.GetItemDescription(FPPowerup.ELEMENT_BURST);
                        case "Max Life Up": return FPSaveManager.GetItemDescription(FPPowerup.MAX_LIFE_UP);
                        case "Crystals to Petals": return FPSaveManager.GetItemDescription(FPPowerup.MORE_PETALS);
                        case "Powerup Start": return FPSaveManager.GetItemDescription(FPPowerup.POWERUP_START);
                        case "Shadow Guard": return FPSaveManager.GetItemDescription(FPPowerup.SHADOW_GUARD);
                        case "Payback Ring": return FPSaveManager.GetItemDescription(FPPowerup.PAYBACK_RING);
                        case "Wood Charm": return FPSaveManager.GetItemDescription(FPPowerup.WOOD_CHARM);
                        case "Earth Charm": return FPSaveManager.GetItemDescription(FPPowerup.EARTH_CHARM);
                        case "Water Charm": return FPSaveManager.GetItemDescription(FPPowerup.WATER_CHARM);
                        case "Fire Charm": return FPSaveManager.GetItemDescription(FPPowerup.FIRE_CHARM);
                        case "Metal Charm": return FPSaveManager.GetItemDescription(FPPowerup.METAL_CHARM);
                        case "No Stocks": return FPSaveManager.GetItemDescription(FPPowerup.STOCK_DRAIN);
                        case "Expensive Stocks": return FPSaveManager.GetItemDescription(FPPowerup.PRICY_STOCKS);
                        case "Double Damage": return FPSaveManager.GetItemDescription(FPPowerup.DOUBLE_DAMAGE);
                        case "No Revivals": return FPSaveManager.GetItemDescription(FPPowerup.NO_REVIVALS);
                        case "No Guarding": return FPSaveManager.GetItemDescription(FPPowerup.NO_GUARDING);
                        case "No Petals": return FPSaveManager.GetItemDescription(FPPowerup.NO_PETALS);
                        case "Time Limit": return FPSaveManager.GetItemDescription(FPPowerup.TIME_LIMIT);
                        case "Items To Bombs": return FPSaveManager.GetItemDescription(FPPowerup.ITEMS_TO_BOMBS);
                        case "Life Oscillation": return FPSaveManager.GetItemDescription(FPPowerup.BIPOLAR_LIFE);
                        case "One Hit KO": return FPSaveManager.GetItemDescription(FPPowerup.ONE_HIT_KO);
                        case "Petal Armor": return FPSaveManager.GetItemDescription(FPPowerup.PETAL_ARMOR);
                        case "Rainbow Charm": return FPSaveManager.GetItemDescription(FPPowerup.RAINBOW_CHARM);

                        // Chapters.
                        case "Sky Pirate Panic": return "Allows access to Avian Museum and Airship Sigwada.";
                        case "Enter the Battlesphere": return "Allows access to Phoenix Highway, Zao Land and The Battlesphere.";
                        case "Mystery of the Frozen North": return "Allows access to Tiger Falls, Robot Graveyard, Shade Armory and Snowfields.";
                        case "Globe Opera": return "Allows access to Globe Opera 1, Globe Opera 2, Auditorium, Palace Courtyard and Tidal Gate.";
                        case "Robot Wars! Snake VS Tarsier": return "Allows access to Zulon Jungle and Nalao Lake.";
                        case "Echoes of the Dragon War": return "Allows access to Ancestral Forge, Magma Starscape and Diamond Point.";
                        case "Justice in the Sky Paradise": return "Allows access to Sky Bridge and Lightning Tower.";
                        case "Bakunawa": return "Allows access to Gravity Bubble, Bakunawa Chase, Bakunawa Rush, Refinery Room, Clockwork Arboretum, Inversion Dynamo, Lunar Cannon, Merga and Weapon's Core.";
                        case "Progressive Chapter": return "Unlocks the next chapter's set of stages.";

                        // Stages
                        case "Dragon Valley":
                        case "Shenlin Park":
                        case "Tiger Falls":
                        case "Robot Graveyard":
                        case "Shade Armory":
                        case "Snowfields":
                        case "Avian Museum":
                        case "Airship Sigwada":
                        case "Phoenix Highway":
                        case "Zao Land":
                        case "The Battlesphere":
                        case "Globe Opera 1":
                        case "Globe Opera 2":
                        case "Auditorium":
                        case "Palace Courtyard":
                        case "Tidal Gate":
                        case "Sky Bridge":
                        case "Lightning Tower":
                        case "Zulon Jungle":
                        case "Nalao Lake":
                        case "Ancestral Forge":
                        case "Magma Starscape":
                        case "Diamond Point":
                        case "Gravity Bubble":
                        case "Bakunawa Chase":
                        case "Bakunawa Rush":
                        case "Refinery Room":
                        case "Clockwork Arboretum":
                        case "Inversion Dynamo":
                        case "Lunar Cannon":
                        case "Merga": return $"Allows access to {location.ItemName}.";

                        // Chest Tracers.
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
                        case "Chest Tracer - Lunar Cannon": return $"Reveals the locations of chests in {location.ItemName.Replace("Chest Tracer - ", "")}.";
                        case "Chest Tracer": return $"Reveals the locations of chests across Avalice.";

                        // Traps.
                        case "Mirror Trap": return "The entire stage is flipped from right to left for 30 seconds.";
                        case "Swap Trap": return "Swaps the active player character for the remainder of the active stage.";
                        case "Pie Trap": return "Spawns one of Acrabelle's pies on the player's position.";
                        case "Spring Trap": return "Spawns a spring in front of the player to send them backwards.";
                        case "PowerPoint Trap": return "Reduces the game's framerate to 15 frames per second for 30 seconds.";
                        case "Zoom Trap": return "Zooms the camera in for 30 seconds.";
                        case "Aaa Trap": return "Causes Aaa to scream for a while.";
                        case "Spike Ball Trap": return "Throws eight Macer spike balls at the player.";
                        case "Pixellation Trap": return "Heavily pixellates the viewport for 30 seconds.";
                        case "Rail Trap": return "Makes every solid surface in the stage into a grind rail.";
                        case "Spam Trap": return "Places a message box on the screen that moves around and changes to distract the player.";
                        case "Syntax Jumpscare Trap": return "Suddenly spawns a giant Syntax on the screen.";
                    }
                }

                // Return the default description.
                return defaultDescription;
            }
        }
    }
}
