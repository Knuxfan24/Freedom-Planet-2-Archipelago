using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using FP2Lib.Player;
using Freedom_Planet_2_Archipelago.Patchers;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class MenuConnection : MonoBehaviour
    {
        /// <summary>
        /// The state that the menu is currently in.
        /// </summary>
        private FPObjectState state;

        /// <summary>
        /// The index of the menu object that is selected.
        /// 0: The server address.
        /// 1: The slot name.
        /// 2: The password.
        /// 3: The character.
        /// 4: The connect button.
        /// </summary>
        private static int menuIndex;

        /// <summary>
        /// The index of the selected character.
        /// </summary>
        public static int characterIndex;

        /// <summary>
        /// The cursor object.
        /// </summary>
        private static GameObject cursor;

        /// <summary>
        /// The server address text box.
        /// </summary>
        private static GameObject serverTextBox;

        /// <summary>
        /// The actual value of the server address.
        /// </summary>
        private static string serverAddress = "archipelago.gg:";

        /// <summary>
        /// The slot name text box.
        /// </summary>
        private static GameObject slotTextBox;

        /// <summary>
        /// The actual value of the slot name.
        /// </summary>
        private static string slotName = "Freedom Planet 2";

        /// <summary>
        /// The password text box.
        /// </summary>
        private static GameObject passwordTextBox;

        /// <summary>
        /// The actual value of the password.
        /// </summary>
        private static string password = "";

        /// <summary>
        /// The character selector.
        /// </summary>
        private static GameObject characterSelector;

        /// <summary>
        /// The object for the connect button when it isn't highlighted.
        /// </summary>
        private static GameObject connectButtonOff;

        /// <summary>
        /// The object for the connect button when it is highlighted.
        /// </summary>
        private static GameObject connectButtonOn;

        /// <summary>
        /// The set of characters, filled in with the base game characters initially.
        /// </summary>
        public static readonly Dictionary<string, int> characters = new() { { "Lilac", 0 }, { "Carol", 1 }, { "Milla", 3 }, { "Neera", 4 } };

        private void Start()
        {
            // Set the menu to its main state.
            state = State_Main;

            // Reset menu index.
            menuIndex = 0;

            // Get the cursor object.
            cursor = transform.GetChild(1).gameObject;

            // Get the text box objects.
            serverTextBox = transform.GetChild(2).GetChild(1).gameObject;
            slotTextBox = transform.GetChild(3).GetChild(1).gameObject;
            passwordTextBox = transform.GetChild(4).GetChild(1).gameObject;

            // Get the character selector.
            characterSelector = transform.GetChild(6).gameObject;

            // Get the two connect button objects.
            connectButtonOff = transform.GetChild(5).GetChild(0).gameObject;
            connectButtonOn = transform.GetChild(5).GetChild(1).gameObject;

            // Add any custom characters and the Random Each Stage option to the character set.
            if (characters.Count == 4)
            {
                // Loop through each character defined in FP2Lib and add them to the dictionary if they have a loaded asset bundle (can't access registered, so this'll do).
                foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
                    if (chara.dataBundle != null)
                        characters.Add(chara.Name, chara.id);

                // Add the Random Each Stage option.
                characters.Add("Random Each Stage", int.MaxValue);
            }

            // Set the option values to the ones stored in the config.
            if (Plugin.configServerAddress != null) serverAddress = Plugin.configServerAddress.Value;
            if (Plugin.configSlotName != null) slotName = Plugin.configSlotName.Value;
            if (Plugin.configPassword != null) password = Plugin.configPassword.Value;
            if (Plugin.configCharacter != null) characterIndex = Plugin.configCharacter.Value;

            // Sanity check the character index (in case a character mod gets removed).
            if (characterIndex >= characters.Count) characterIndex = 0;

            // Replace the placeholder values in the prefab textboxes.
            serverTextBox.GetComponent<TextMesh>().text = serverAddress;
            slotTextBox.GetComponent<TextMesh>().text = slotName;
            passwordTextBox.GetComponent<TextMesh>().text = password;
            characterSelector.GetComponent<TextMesh>().text = $"Character: < {characters.ElementAt(characterIndex).Key} >";
        }

        private void Update()
        {
            // Update the menu input if we're not somehow paused.
            if (FPStage.state != FPStageState.STATE_PAUSED)
                FPStage.UpdateMenuInput();

            // Invoke our state if it isn't null and the menu object is registered.
            if (FPStage.objectsRegistered && state != null)
                state();
        }

        class CacheJSON
        {
            public Dictionary<string, long>? item_name_to_id { get; set; }
        }

        private void State_Main()
        {
            // Check for the F9 key or Select button to trigger the template sprite definition creator.
            if (Input.GetKeyDown(KeyCode.F9) || Input.GetKeyDown("joystick 1 button 8"))
            {
                // Get the Archipelago cache directory.
                string cacheDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Archipelago\Cache\datapackage";

                // Check that the cache directory actually exists (unlike the AP text client itself...)
                if (Directory.Exists(cacheDirectory))
                {
                    // Loop through each directory in the cache.
                    foreach (string? directory in Directory.GetDirectories(cacheDirectory))
                    {
                        // Get the name of this game's cache directory.
                        string gameName = Path.GetFileName(directory);

                        // Skip Freedom Planet 2.
                        if (gameName == "Freedom Planet 2")
                            continue;

                        // Log that we're creating the template file.
                        Plugin.consoleLog.LogInfo($"Creating template items.json file for {gameName}");

                        // Create the directory for this game in the Template Sprite Definitions directory.
                        Directory.CreateDirectory($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Template Sprite Definitions\{gameName}");

                        // Get the newest JSON.
                        FileInfo newestJson = new DirectoryInfo(directory).GetFiles("*.json").OrderByDescending(f => f.LastWriteTime).First();

                        // Deserialise the JSON to grab the item_name_to_id block from it.
                        CacheJSON json = JsonConvert.DeserializeObject<CacheJSON>(File.ReadAllText(newestJson.FullName));

                        // Set up a list of item descriptors.
                        List<ItemDescriptor> itemDescriptors = [];

                        // Loop through each item in the JSON and create an item definition with its name filled in in the item names array and sprite name.
                        if (json.item_name_to_id != null)
                        {
                            foreach (KeyValuePair<string, long> item in json.item_name_to_id)
                            {
                                itemDescriptors.Add(new()
                                {
                                    ItemNames = [item.Key],
                                    SpriteName = item.Key
                                });
                            }
                        }

                        // Write our generated items.json file to our directory.
                        File.WriteAllText($@"{Paths.GameRootPath}\mod_overrides\Archipelago\Template Sprite Definitions\{gameName}\items.json", JsonConvert.SerializeObject(itemDescriptors, Formatting.Indented));
                    }
                }
            }

            // Move the cursor up or down.
            if (FPStage.menuInput.up)
            {
                if (menuIndex != 0)
                    menuIndex--;
                else
                    menuIndex = 4;

                SetCursorPosition();
            }
            if (FPStage.menuInput.down)
            {
                if (menuIndex != 4)
                    menuIndex++;
                else
                    menuIndex = 0;

                SetCursorPosition();
            }

            // Change the character selection.
            if (FPStage.menuInput.left && menuIndex == 3)
            {
                if (characterIndex == 0)
                    characterIndex = characters.Count - 1;
                else
                    characterIndex--;

                FPAudio.PlayMenuSfx(FPAudio.SFX_MOVE);
                characterSelector.GetComponent<TextMesh>().text = $"Character: < {characters.ElementAt(characterIndex).Key} >";
            }
            if (FPStage.menuInput.right && menuIndex == 3)
            {
                if (characterIndex == characters.Count - 1)
                    characterIndex = 0;
                else
                    characterIndex++;

                FPAudio.PlayMenuSfx(FPAudio.SFX_MOVE);
                characterSelector.GetComponent<TextMesh>().text = $"Character: < {characters.ElementAt(characterIndex).Key} >";
            }

            // Handle selecting an option.
            if (FPStage.menuInput.confirm)
            {
                switch (menuIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                        cursor.gameObject.SetActive(false);
                        FPAudio.PlayMenuSfx(FPAudio.SFX_SELECT);
                        state = State_Typing;
                        break;

                    case 4:
                        FPAudio.PlayMenuSfx(FPAudio.SFX_SELECT);
                        state = State_WaitingToConnect;
                        break;
                }
            }
        }

        private void State_WaitingToConnect()
        {
            // Log the connection attempt.
            Plugin.consoleLog.LogInfo($"Attempting to connect to {serverAddress} as {slotName} with password {password}");

            // Create the session and try to connect.
            Plugin.session = ArchipelagoSessionFactory.CreateSession(serverAddress);
            LoginResult connectionResult = Plugin.session.TryConnectAndLogin("Freedom Planet 2", slotName, Archipelago.MultiClient.Net.Enums.ItemsHandlingFlags.AllItems, null, null, null, password, true);

            // Check if the connection failed.
            if (!connectionResult.Successful)
            {
                // Get the failure data.
                LoginFailure connectionFailure = (LoginFailure)connectionResult;

                // Append the errors reported to a message and write it to the console.
                string errorMessage = $"Failed to Connect to {serverAddress} as {slotName} with password {password}:";
                foreach (string error in connectionFailure.Errors)
                    errorMessage += $"\n\t{error}";
                foreach (ConnectionRefusedError error in connectionFailure.ErrorCodes)
                    errorMessage += $"\n\t{error}";
                Plugin.consoleLog.LogError(errorMessage);

                // Play the invalid sound as an indicator.
                FPAudio.PlayMenuSfx(FPAudio.SFX_INVALID);

                // Return to the main state without running the rest of this one.
                state = State_Main;
                return;
            }

            // Get the success data.
            LoginSuccessful connectionSuccess = (LoginSuccessful)connectionResult;

            // Get the slot data.
            Plugin.slotData = connectionSuccess.SlotData;

            // Overwrite the link values in the slot data if we need to.
            if (Plugin.configDeathLinkOverride.Value != -1) Plugin.slotData["death_link"] = Plugin.configDeathLinkOverride.Value;
            if (Plugin.configRingLinkOverride.Value != -1) Plugin.slotData["ring_link"] = Plugin.configRingLinkOverride.Value;
            if (Plugin.configTrapLinkOverride.Value != -1) Plugin.slotData["trap_link"] = Plugin.configTrapLinkOverride.Value;
            
            // Print all the slot data to the log, as a debug log.
            foreach (var key in Plugin.slotData)
                Plugin.consoleLog.LogDebug($"{key.Key}: {key.Value} (Type: {key.Value.GetType()})");

            // Create the socket handler for receiving items.
            Plugin.session.Items.ItemReceived += SocketEvents.Socket_ReceiveItem;
            
            // Fetch all the locations.
            var locations = Plugin.session.Locations.AllLocations;
            Plugin.session.Locations.ScoutLocationsAsync(items =>
                {
                    Plugin.items = items;
                },
                false, locations.ToArray());

            // Create the DeathLink service and the socket handler for receiving them.
            Plugin.DeathLink = Plugin.session.CreateDeathLinkService();
            Plugin.DeathLink.OnDeathLinkReceived += SocketEvents.Socket_ReceiveDeathLink;

            // Enable DeathLink if its enabled in the slot data.
            if ((long)Plugin.slotData["death_link"] != 0)
                Plugin.DeathLink.EnableDeathLink();

            // Add the RingLink tag if its enabled in our slot data.
            if ((long)Plugin.slotData["ring_link"] != 0)
                Plugin.session.ConnectionInfo.UpdateConnectionOptions([.. Plugin.session.ConnectionInfo.Tags, .. new string[1] { "RingLink" }]);

            // Add the TrapLink tag if its enabled in our slot data.
            if ((long)Plugin.slotData["trap_link"] != 0)
                Plugin.session.ConnectionInfo.UpdateConnectionOptions([.. Plugin.session.ConnectionInfo.Tags, .. new string[1] { "TrapLink" }]);

            // Set up the socket events.
            Plugin.session.Socket.PacketReceived += SocketEvents.Socket_LinkPackets;

            #region Archipelago Save Setup/Loading
            // Check if we don't already have a save for this seed.
            if (!File.Exists($@"{FPSaveManagerPatcher.GetSavesPath()}\{Plugin.session.RoomState.Seed}_Save.json"))
            {
                // Set up a random number generator.
                System.Random rng = new();

                // Roll a random number to use for our slot.
                int saveSlot = rng.Next();

                // If this slot number already exists, reroll until it doesn't.
                while (File.Exists($@"{Paths.GameRootPath}\Archipelago Saves\{saveSlot}.json"))
                    saveSlot = rng.Next();

                // Create a save with this slot.
                Plugin.save = new(saveSlot);
            }

            // If we do already have a save, then just load it.
            else
                Plugin.save = JsonConvert.DeserializeObject<ArchipelagoSave>(File.ReadAllText($@"{Paths.GameRootPath}\Archipelago Saves\{Plugin.session.RoomState.Seed}_Save.json"));

            if ((long)Plugin.slotData["chest_tracer_items"] == 0)
                Plugin.save.ChestTracers = Enumerable.Repeat(true, 24).ToArray();

            // Unlock the "Empty" item for both the Brave Stones and Potions so the menu doesn't freak out and break.
            Plugin.save.BraveStones[0] = true;
            Plugin.save.Potions[0] = true;

            // If the chapter option is set to open, then unlock the Weapon's Core panel, as it doesn't have a stage item in this state.
            if ((long)Plugin.slotData["chapters"] == 2)
                Plugin.save.StageUnlocks[31] = true;
            #endregion

            #region Freedom Planet 2 Save Setup/Loading
            // Set up a new FP2 save so things in the shop aren't marked as purchased erroneously.
            FPSaveManager.NewGame(0);

            // Load the existing file.
            if (File.Exists($@"{Paths.GameRootPath}\Archipelago Saves\{Plugin.save.SaveSlot}.json"))
                FPSaveManager.LoadFromFile(Plugin.save.SaveSlot);

            // Set the character based on our selected value.
            FPSaveManager.character = (FPCharacterID)characters.ElementAt(characterIndex).Value;

            // If our character ID is set to the integer limit, then set our Using Random Character flag.
            if ((int)FPSaveManager.character == int.MaxValue)
                Plugin.usingRandomCharacter = true;
            else
                Plugin.usingRandomCharacter = false;

            // Reveal all the map tiles.
            for (int tileIndex = 0; tileIndex < FPSaveManager.mapTileReveal.Length; tileIndex++)
                FPSaveManager.mapTileReveal[tileIndex] = true;

            // Save our two files.
            Helpers.Save();
            #endregion

            #region Getting Items upon Connection
            // Loop through each item that have been received from the multiworld and add it to the item queue
            foreach (ItemInfo item in Plugin.session.Items.AllItemsReceived)
            {
                SocketEvents.SetUpQueue(item);
                Plugin.session.Items.DequeueItem();
            }

            // Send each value through the HandleItem function.
            Helpers.HandleStartItems();

            // Clear out the item queue.
            Plugin.itemQueue = [];

            // Set the item queue's timer so it can handle items received while connected.
            Plugin.itemQueueTimer = 0f;
            #endregion

            // Find the menu's screen transition object.
            FPScreenTransition transition = GameObject.Find("Screen Transition").GetComponent<FPScreenTransition>();

            // Set the transition's type to wipe.
            transition.transitionType = FPTransitionTypes.WIPE;

            // Set the speed of the transition.
            transition.transitionSpeed = 48f;

            // Load the Classic Menu.
            transition.sceneToLoad = "ClassicMenu";

            // Set the transition to pure black.
            transition.SetTransitionColor(0f, 0f, 0f);

            // Start the transition.
            transition.BeginTransition();

            // Stop the music.
            FPAudio.StopMusic();

            // Play the menu wipe sound.
            FPAudio.PlayMenuSfx(3);

            // Update the values in the config file.
            Plugin.configServerAddress.Value = serverAddress;
            Plugin.configSlotName.Value = slotName;
            Plugin.configPassword.Value = password;
            Plugin.configCharacter.Value = characterIndex;

            // Set the current character in FP2Lib, as we bypass where it would normally be set, which breaks everything.
            PlayerHandler.currentCharacter = PlayerHandler.GetPlayableCharaByFPCharacterId(FPSaveManager.character);

            // Swap to the nothing state.
            state = State_Nothing;
        }

        private void State_Nothing() { }

        private void State_Typing()
        {
            // Pass the right values to the AddCharacter and SetTextboxText functions depending on the menu index.
            switch (menuIndex)
            {
                case 0:
                    AddCharacter(serverAddress);
                    SetTextboxText(serverTextBox.GetComponent<TextMesh>(), serverAddress);
                    break;

                case 1:
                    AddCharacter(slotName);
                    SetTextboxText(slotTextBox.GetComponent<TextMesh>(), slotName);
                    break;

                case 2:
                    AddCharacter(password);
                    SetTextboxText(passwordTextBox.GetComponent<TextMesh>(), password);
                    break;
            }

            static void SetTextboxText(TextMesh textMesh, string text)
            {
                // If the string is longer than 28 characters, then visually truncate it.
                // If not, then just set the textbox's value to the string as is.
                if (text.Length > 28)
                    textMesh.text = text.Substring(0, 25) + "...";
                else
                    textMesh.text = text;
            }

            void AddCharacter(string value)
            {
                // Loop through each character in Unity's input string.
                foreach (char character in Input.inputString)
                {
                    // Check if the character is a return.
                    if (character == '\r')
                    {
                        // Swap back to the main state.
                        state = State_Main;

                        // Play the menu selection sound.
                        FPAudio.PlayMenuSfx(FPAudio.SFX_SELECT);

                        // Reactivate the cursor object.
                        cursor.gameObject.SetActive(true);

                        // If the string is longer than 28 characters, then print the full one to the console.
                        if (value.Length > 28)
                        {
                            switch (menuIndex)
                            {
                                case 0: Plugin.consoleLog.LogInfo($"Full server address is: {value}"); break;
                                case 1: Plugin.consoleLog.LogInfo($"Full slot name is: {value}"); break;
                                case 2: Plugin.consoleLog.LogInfo($"Full password is: {value}"); break;
                            }
                        }

                        // Stop here.
                        return;
                    }

                    // Check if this character is a backspace.
                    if (character == '\b')
                    {
                        // Check if the string has any characters in it.
                        if (value.Length > 0)
                        {
                            // Remove the last character.
                            value = value.Substring(0, value.Length - 1);

                            // Play the dialog sound as an indicator.
                            FPAudio.PlayMenuSfx(FPAudio.SFX_DIALOG);
                        }
                        else
                        {
                            // Play the the invalid sound if we have no characters to remove.
                            FPAudio.PlayMenuSfx(FPAudio.SFX_INVALID);
                        }
                    }

                    // Assume this is a normal character.
                    else
                    {
                        // Add this character to our string.
                        value += character;

                        // Play the score tally sound as an indicator.
                        FPAudio.PlayMenuSfx(FPAudio.SFX_TALLY);
                    }
                }

                // Update the right value depending on the menu index.
                switch (menuIndex)
                {
                    case 0: serverAddress = value; break;
                    case 1: slotName = value; break;
                    case 2: password = value; break;
                }
            }
        }

        private void SetCursorPosition()
        {
            // Play the cursor move sound.
            FPAudio.PlayMenuSfx(FPAudio.SFX_MOVE);

            // Move the cursor depending on its current index.
            switch(menuIndex)
            {
                case 0: cursor.transform.localPosition = new(64, -26); break;
                case 1: cursor.transform.localPosition = new(64, -54); break;
                case 2: cursor.transform.localPosition = new(64, -82); break;
                case 3: cursor.transform.localPosition = new(64, -110); break;
                case 4: cursor.transform.localPosition = new(-128, -26); break;
            }

            // Enable or disable the connection button sprite renderers depending on if its selected or not.
            if (menuIndex != 4)
            {
                connectButtonOff.GetComponent<SpriteRenderer>().enabled = true;
                connectButtonOn.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                connectButtonOff.GetComponent<SpriteRenderer>().enabled = false;
                connectButtonOn.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
