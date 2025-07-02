using Archipelago.MultiClient.Net.Models;
using FP2Lib.Player;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

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
        /// Sets the shop prices to the value chosen in the player YAML.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuShop), "Start")]
        static void SetShopPrices(ref bool ___payWithCrystals, ref int[] ___itemCosts)
        {
            // If this is the Vinyl shop, then set the values in the item costs array to that of vinyl_shop_price.
            if (___payWithCrystals)
                for (int costIndex = 0; costIndex < ___itemCosts.Length; costIndex++)
                    ___itemCosts[costIndex] = (int)(long)Plugin.slotData["vinyl_shop_price"];

            // If this is the Vinyl shop, then set the values in the item costs array to that of milla_shop_price.
            else
                for (int costIndex = 0; costIndex < ___itemCosts.Length; costIndex++)
                    ___itemCosts[costIndex] = (int)(long)Plugin.slotData["milla_shop_price"];
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
                GatherSpritesAndLocations(30, "Milla");
            else
                GatherSpritesAndLocations(60, "Vinyl");

            void GatherSpritesAndLocations(int itemCount, string shop)
            {
                // Loop through and get the location indices for this shop.
                for (int i = 1; i <= itemCount; i++)
                    locationIDs.Add(Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", $"{shop} Shop Item {i}"));

                // Scout the locations for this shop.
                Plugin.session.Locations.ScoutLocationsAsync(HandleScoutInfo, [.. locationIDs]);

                // Wait for the scout to finish before continuing.
                while (_ScoutedLocationInfo.Count < itemCount)
                    System.Threading.Thread.Sleep(1);

                // Get the sprites for the items in this shop.
                List<Sprite> sprites = [];
                for (int i = 0; i < itemCount; i++)
                    sprites.Add(Helpers.GetItemSprite(_ScoutedLocationInfo.ElementAt(i).Value, true));
                Sprites = [.. sprites];

                // If our shop information setting is set to full, then also send hints for the items in this shop.
                if ((long)Plugin.slotData["shop_information"] == 0)
                {
                    // Reset the location ID lost.
                    locationIDs = [];

                    // Calculate how many items are valid hints.
                    int hintableItems = FPSaveManager.TotalStarCards();
                    if (shop == "Vinyl")
                        hintableItems *= 2;

                    // Loop through and get the location indices for this shop's hints.
                    for (int i = 1; i <= hintableItems; i++)
                        locationIDs.Add(Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", $"{shop} Shop Item {i}"));

                    // Scout for the hints for these locations.
                    // TODO: If an item is purchased before being hinted for, then it makes the hint each time despite the annouce setting. MultiClient bug?
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
                for (int i = 0; i < ___powerups.Length; i++)
                    if (___powerups[i].digitValue > 1 && (i + ___detailListOffset) < 30)
                        ___powerups[i].GetComponent<SpriteRenderer>().sprite = Sprites[i + ___detailListOffset];
            }
            else
            {
                for (int i = 0; i < ___vinyls.Length; i++)
                    if (___vinyls[i].digitValue != 0 && (i + ___detailListOffset) < 60)
                        ___vinyls[i].GetComponent<SpriteRenderer>().sprite = Sprites[i + ___detailListOffset];
            }

            // Only replace the item name and description if it's unlocked.
            if (___detailName[0].GetComponent<TextMesh>().text != "? ? ? ? ?")
            {
                // Don't try and replace the item name and description if it would end up out of bounds (likely because of FP2Lib adding Vinyls from other mods).
                if ((___payWithCrystals || selectedItem >= 30) && (!___payWithCrystals || selectedItem >= 60))
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
                if (location.ItemGame == "Manual_FreedomPlanet2_Knuxfan24")
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
                    }
                }

                // Return the default description.
                return defaultDescription;
            }
        }
    }
}
