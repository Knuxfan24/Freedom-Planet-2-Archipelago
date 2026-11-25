using Archipelago.MultiClient.Net.Helpers;
using BepInEx.Bootstrap;

namespace Freedom_Planet_2_Archipelago
{
    internal class SocketEvents
    {
        /// <summary>
        /// Joke messages for if a DeathLink comes in without a reason attached to it.
        /// </summary>
        private static readonly string[] GenericDeathLinkReasons =
        [
            "$ had an oopsie.",
            "$ didn't find their Photochad.", // Reference to Balatro.
            "$ tripped.",
            "$ died. Sadge.",
            "$'s princess is in another castle.", // Reference to Super Mario Brothers.
            "$ got bodied.",
            "$ hit the ground too hard.", // Reference to Minecraft.
            "$ should have played on Drizzle.", // Reference to Risk of Rain.
            "$ didn't make it to 6AM.", // Reference to Five Nights at Freddy's.
            "$ had a Station Brakes Failure.", // Reference to RollerCoaster Tycoon.
            "$'s heart belongs to darkness.", // Reference to Kingdom Hearts.
            "$ had vapor for brains.", // Reference to Metroid Prime.
            "$ passed out at 2AM.", // Reference to Stardew Valley.
            "$ didn't follow the damn train.", // Reference to Grand Theft Auto: San Andreas.
            "Cranky was right about $.", // Reference to Donkey Kong Country.
            "$ forgot to rip and tear.", // Reference to Doom.
            "$ missed 1 box.", // Reference to Crash Bandicoot.
            "$ had a bad time.", // Reference to Undertale.
            "$ forgot the real superpower of teamwork.", // Reference to Sonic Heroes.
            "$ is a horrible goose.", // Reference to Untilted Goose Game.
            "$ got splatted.", // Reference to Splatoon.
            "$ misread their tracker client.",
            "$ forgot the basics of CQC.", // Reference to Metal Gear Solid 3.
            "The future did not start with $.", // Reference to Portal 2.
            "$ let it overcharge.", // Reference to Half-Life.
            "$ did not pass Go.", // Reference to Monopoly.
            "The \"miracle\" never happen for $.", // Reference to Ace Attorney.
            "$ perished on the climb.", // Reference to Celeste.
            "$ was not The Imposter.", // Reference to Among Us.
            "$ got Crossworlded.", // Reference to Sonic Racing CrossWorlds.
            "$ fired the laser.", // Reference to both Freedom Planet 2 itself and to System Shock.
            "$ removed their OS Chip.", // Reference to NieR Automata.
        ];

        /// <summary>
        /// What TrapLink items should be received as. All the trap names were sourced from https://docs.google.com/spreadsheets/d/1yoNilAzT5pSU9c2hYK7f2wHAe9GiWDiHFZz8eMe1oeQ/edit?gid=811965759#gid=811965759.
        /// TODO: Fully go through the sheet and add things for newer traps/rethink older ones. Also update the FP2 column on the sheet.
        /// </summary>
        private static readonly Dictionary<string, string[]> TrapLinkMappings = new()
        {
            { "144p Trap", ["Pixellation Trap"] },
            { "Aaa Trap", ["Aaa Trap"] },
            { "Animal Trap", ["Swap Trap"] },
            { "Animal Bonus Trap", [] },
            { "Army Trap", ["Spike Ball Trap"] },
            { "Attraction Breakdown Trap", [] },
            { "Attraction Voucher Trap", [] },
            { "Bald Trap", [] },
            { "Banana Peel Trap", ["Pie Trap"] },
            { "Banana Trap", ["Pie Trap"] },
            { "Banner Trap", ["Double Damage"] },
            { "Bathroom Trap", [] },
            { "Bee Trap", ["Syntax Jumpscare Trap", "Zoom Trap"] },
            { "Blango Artillery", ["Pie Trap"] },
            { "Blastoad", ["Spring Trap"] },
            { "Blue Balls Curse", ["One Hit KO"] },
            { "Bomb", ["Spike Ball Trap", "Items To Bombs"] },
            { "Bomb Trap", ["Spike Ball Trap", "Items To Bombs"] },
            { "Bonk Trap", ["Pie Trap"] },
            { "Breakout Trap", [] },
            { "Bubble Trap", ["Pie Trap"] },
            { "Bullet Time Trap", ["PowerPoint Trap"] },
            { "Burger Hat Trap", ["Syntax Jumpscare Trap"] },
            { "Burn Trap", [] },
            { "Buyon Trap", ["Spike Ball Trap"] },
            { "Camera Trap", ["Mirror Trap", "Zoom Trap"] },
            { "Camera Rotate Trap", ["Mirror Trap"] },
            { "Chameleos Spit", ["Double Damage"] },
            { "Chaos Trap", [] },
            { "Chaos Control Trap", ["Pie Trap"] },
            { "Chart Modifier Trap", [] },
            { "Chaser Trap", [] },
            { "Clear Image Trap", [] },
            { "Coda Trap", [] },
            { "Confound Trap", ["Pixellation Trap"] },
            { "Confuse Trap", ["Pixellation Trap"] },
            { "Confusion Trap", ["Pixellation Trap"] },
            { "Conga Artillery", ["No Petals"] },
            { "Control Ball Trap", [] },
            { "Controller Drift Trap", ["Rail Trap"] },
            { "CRT Trap", ["Pixellation Trap"] },
            { "Crystal Trap", ["Pixellation Trap"] },
            { "Curse Trap", [] },
            { "Cursed Trap", [] },
            { "Cursed Ball Trap", [] },
            { "Cutscene Trap", ["Aaa Trap"] },
            { "Dad Trap", [] },
            { "Damage Trap", ["Life Oscillation"] },
            { "Debug Level Trap", [] },
            { "Dead Ringer Trap", [] },
            { "Deisometric Trap", ["Zoom Trap", "Mirror Trap"] },
            { "Depletion Trap", [] },
            { "Depression Trap", [] },
            { "Disable A Trap", ["No Guarding"] },
            { "Disable B Trap", ["No Guarding"] },
            { "Disable C Up Trap", ["No Guarding"] },
            { "Disable Tag Trap", ["Swap Trap"] },
            { "Disable Z Trap", ["No Guarding"] },
            { "Disarm Trap", [] },
            { "Double Damage", ["Double Damage"] },
            { "Dry Trap", ["No Stocks"] },
            { "E. Gadd Ramblings", ["Aaa Trap"] },
            { "Earth Trap", [] },
            { "Egg Trap", [] },
            { "Eject Ability", ["Swap Trap"] },
            { "Electrocution Trap", ["Life Oscillation"] },
            { "Employee Hiring Trap", [] },
            { "Employee Tiredness Trap", [] },
            { "Employee Training Trap", [] },
            { "Empty Item Box Trap", ["No Guarding"] },
            { "Enchantment Trap", [] },
            { "Enemy Ball Trap", [] },
            { "Energy Drain Trap", [] },
            { "Expensive Stocks", ["Expensive Stocks"] },
            { "Explosion Trap", ["Items To Bombs"] },
            { "Exposition Trap", ["Aaa Trap"] },
            { "Extreme Chaos Mode", [] },
            { "Fake Transition", ["Zoom Trap"] },
            { "Farcaster", [] },
            { "Fast Trap", ["Mach Speed Trap"] },
            { "Fat CJ Trap", [] },
            { "Fear Trap", ["Syntax Jumpscare Trap"] },
            { "Fire Trap", ["Life Oscillation"] },
            { "Fish Eye Trap", ["Zoom Trap", "Mirror Trap", "Pixellation Trap"] },
            { "Fishing Trap", [] },
            { "Fishin' Boo Trap", [] },
            { "First Person Trap", [] },
            { "Flint Trap", [] },
            { "Flip Horizontal Trap", ["Mirror Trap"] },
            { "Flip Trap", ["Mirror Trap"] },
            { "Flip Vertical Trap", ["Mirror Trap"] },
            { "Flood Trap", ["Flood Trap"] },
            { "Food Poisoning Trap", ["Life Oscillation"] },
            { "Fracture Trap", ["Pixellation Trap"] },
            { "Frame Slime Trap", [] },
            { "Freeze Trap", ["Pie Trap"] },
            { "Frog Trap", ["Swap Trap"] },
            { "Frost Trap", [] },
            { "Frozen Trap", ["Pie Trap"] },
            { "Furry Convention Trap", ["Swap Trap"] },
            { "Fuzzy Trap", ["Pixellation Trap"] },
            { "Gadget Shuffle Trap", ["Swap Trap"] },
            { "Gas Trap", ["Mirror Trap"] },
            { "Get Out Trap", ["Time Limit"] },
            { "Ghost", ["Spike Ball Trap"] },
            { "Ghost Chat", ["Aaa Trap"] },
            { "Glass Trap", [] },
            { "Gloom Trap", [] },
            { "Gooey Bag", [] },
            { "Gravity Trap", [] },
            { "Grayscale Trap", [] },
            { "Guest Bathroom Trap", [] },
            { "Guest Happiness Trap", [] },
            { "Guest Hunger Trap", [] },
            { "Guest Kill Trap", [] },
            { "Guest Money Trap", [] },
            { "Guest Spawn Trap", [] },
            { "Guest Thirst Trap", [] },
            { "Guest Tiredness Trap", [] },
            { "Guest Vandal Trap", [] },
            { "Guest Vomiting Trap", [] },
            { "Gustcrab", [] },
            { "Help Trap", ["Aaa Trap"] },
            { "Hey! Trap", ["Aaa Trap", "Spam Trap"] },
            { "Hiccup Trap", ["Aaa Trap"] },
            { "Hidden Trap", [] },
            { "HiSpeed Trap", ["Mach Speed Trap"] },
            { "Home Trap", [] },
            { "Honey Trap", ["Rail Trap"] },
            { "Ice Floor Trap", ["Rail Trap"] },
            { "Ice Trap", ["Pie Trap"] },
            { "Icon Trap", [] },
            { "Icy Hot Pants Trap", ["Spring Trap"] },
            { "Impatience Trap", ["Time Limit"] },
            { "Input Sequence Trap", [] },
            { "Instant Crystal Trap", [] },
            { "Instant Death Trap", ["One Hit KO"] },
            { "Invert Colors Trap", [] },
            { "Inverted Mouse Trap", ["Mirror Trap"] },
            { "Invisiball Trap", [] },
            { "Invisible Trap", [] },
            { "Invisibility Trap", [] },
            { "Iron Boots Trap", [] },
            { "Isometric Trap", [] },
            { "Items to Bombs", ["Items to Bombs"] },
            { "Jump Trap", [] },
            { "Jumping Jacks Trap", [] },
            { "Lane Trap", [] },
            { "Laser Trap", [] },
            { "Laughter Trap", ["Aaa Trap"] },
            { "Leaping Trap", [] },
            { "Light Up Path Trap", [] },
            { "Literature Trap", ["Aaa Trap"] },
            { "Mana Drain Trap", [] },
            { "Market Crash Trap", [] },
            { "Math Quiz Trap", [] },
            { "Meteor Trap", ["Spike Ball Trap"] },
            { "Metronome Trap", [] },
            { "Minimap Trap", [] },
            { "Mirror Trap", ["Mirror Trap"] },
            { "Modified Bloons Trap", [] },
            { "Monkey Mash Trap", [] },
            { "Monkey Trap", [] },
            { "My Turn! Trap", ["Mirror Trap"] },
            { "Mystery Trap", [] },
            { "Nightmare Trap", [] },
            { "Ninja Trap", [] },
            { "No Guarding", ["No Guarding"] },
            { "No Hat Trap", [] },
            { "No Petals", ["No Petals"] },
            { "No Revivals", ["No Revivals"] },
            { "No Return Trap", [] },
            { "No Stocks", ["No Stocks"] },
            { "No Vac Trap", ["No Guarding"] },
            { "NoSpeed Trap", [] },
            { "Number Sequence Trap", [] },
            { "Nut Trap", ["Spike Ball Trap"] },
            { "OmoTrap", ["Aaa Trap", "Spam Trap"] },
            { "One Hit KO", ["One Hit KO"] },
            { "One Hit Trap", [] },
            { "Paper Trap", [] },
            { "Paralyze Trap", ["Pie Trap"] },
            { "Paralysis Trap", ["Pie Trap"] },
            { "Paratoad", ["Pie Trap"] },
            { "Pea Soup Trap", [] },
            { "Person Trap", [] },
            { "Phone Trap", ["Aaa Trap"] },
            { "Pie Trap", ["Pie Trap"] },
            { "Pikmin Trap", [] },
            { "Pinball Trap", [] },
            { "Pincercrab", [] },
            { "Pixelate Trap", ["Pixellation Trap"] },
            { "Pixellation Trap", ["Pixellation Trap"] },
            { "Player Money Trap", [] },
            { "Poison Mushroom", ["Zoom Trap"] },
            { "Poison Trap", ["Life Oscillation"] },
            { "Poisontoad", ["Life Oscillation"] },
            { "Pokemon Count Trap", [] },
            { "Pokemon Trivia Trap", ["Trivia Trap"] },
            { "Police Trap", ["Spike Ball Trap"] },
            { "PONG Challenge", [] },
            { "Pong Trap", [] },
            { "Posession Trap", ["Life Oscillation"] },
            { "PowerPoint Trap", ["PowerPoint Trap"] },
            { "PSP Trap", [] },
            { "Puppet Spider", [] },
            { "Push Trap", ["Rail Trap"] },
            { "Radiation Trap", [] },
            { "Rail Trap", ["Rail Trap"] },
            { "Ranch Trap", [] },
            { "Random Status Trap", [] },
            { "Research Trap", [] },
            { "Resistance Trap", [] },
            { "Reversal Trap", ["Mirror Trap"] },
            { "Reverse Controls Trap", ["Mirror Trap"] },
            { "Reverse Trap", ["Mirror Trap"] },
            { "Riptide Trap", [] },
            { "Rockfall Trap", ["Spike Ball Trap"] },
            { "Sandstorm Trap", ["Pixellation Trap"] },
            { "Saturation Trap", [] },
            { "Screen Flip Trap", ["Mirror Trap"] },
            { "SFX Trap", [] },
            { "Shake Trap", [] },
            { "Shrink Trap", [] },
            { "Shuffle Trap", [] },
            { "Sinking Trap", [] },
            { "Sleep Trap", ["Pie Trap"] },
            { "Sleeptoad", ["Pie Trap"] },
            { "Slip Trap", ["Pie Trap"] },
            { "Slow Trap", ["PowerPoint Trap"] },
            { "Slowness Trap", ["PowerPoint Trap"] },
            { "Smile Trap", [] },
            { "Snake Trap", [] },
            { "Spam Trap", ["Spam Trap"] },
            { "Speed Up Trap", ["Mach Speed Trap"] },
            { "Spider Web Trap", [] },
            { "Spike Ball Trap", ["Spike Ball Trap"] },
            { "Spin Trap", [] },
            { "Space Trap", [] },
            { "Spooky Time", ["Zoom Trap"] },
            { "Spotlight Trap", ["Zoom Trap"] },
            { "Spring Trap", ["Spring Trap"] },
            { "Squash Trap", ["Pie Trap"] },
            { "Sticky Floor Trap", [] },
            { "Sticky Hands Trap", [] },
            { "Stinky Trap", [] },
            { "Stun Trap", ["Pie Trap"] },
            { "Stutter Trap", [] },
            { "Sudden Trap", [] },
            { "Summon Trap", [] },
            { "SvC Effect", [] },
            { "Swap Trap", [] },
            { "Syntax Jumpscare Trap", ["Syntax Jumpscare Trap"] },
            { "Tar Trap", ["Pie Trap"] },
            { "Tarr Trap", [] },
            { "Taunt Trap", [] },
            { "Teleport Trap", [] },
            { "Text Trap", ["Aaa Trap", "Spam Trap"] },
            { "Thwimp Trap", ["Spike Ball Trap"] },
            { "Time Limit", ["Time Limit"] },
            { "Time Warp Trap", ["Time Limit"] },
            { "Timer Trap", ["Time Limit"] },
            { "Tiny Trap", ["Zoom Trap"] },
            { "Tip Trap", [] },
            { "TNT Barrel Trap", ["Spike Ball Trap"] },
            { "TNT Trap", ["Items To Bombs"] },
            { "Tool Swap Trap", [] },
            { "Toxin Trap", [] },
            { "Transmute Trap", [] },
            { "Trivia Trap", ["Spam Trap"] },
            { "Tutorial Trap", ["Aaa Trap", "Spam Trap"] },
            { "Underwater Trap", [] },
            { "Undo Trap", [] },
            { "UNO Challenge", [] },
            { "Upscroll Trap", [] },
            { "Vintage Trap", [] },
            { "Wailnard", [] },
            { "Weather Cloudy Trap", [] },
            { "Weather Rainy Trap", [] },
            { "Weather Stormy Trap", [] },
            { "Weather Sunny Trap", [] },
            { "Well Done Trap", [] },
            { "W I D E Trap", ["Zoom Trap"] },
            { "Whirlpool Trap", [] },
            { "Whoops! Trap", [] },
            { "Yap Trap", ["Aaa Trap"] },
            { "Zoom In Trap", ["Zoom Trap"] },
            { "Zoom Out Trap", ["Zoom Trap"] },
            { "Zoom Trap", ["Zoom Trap"] },
        };

        /// <summary>
        /// TrapLink things that require the player object to exist and a stage to be fully loaded before they can activate.
        /// </summary>
        public static readonly string[] TrapLinkRequiresPlayer =
        [
            "Double Damage",
            "Expensive Stocks",
            "Items To Bombs",
            "Life Oscillation",
            "No Guarding",
            "No Petals",
            "No Revivals",
            "No Stocks",
            "One Hit KO",
            "Time Limit",
            "Invisibility Cloak",
            "Madstone",
            "Explosive Finale",
            "Idol of Greed",
            "Gravity Boots",
            "Swap Trap",
            "Pie Trap",
            "Spring Trap",
            "Spike Ball Trap",
            "Trivia Trap",
            "Mach Speed Trap",
            "Wordle Trap",
            "Flood Trap"
        ];

        /// <summary>
        /// TrapLink things that come from the Potion Seller mod.
        /// </summary>
        public static readonly string[] PotionSellerTrapLink =
        [
            "Invisibility Cloak",
            "Madstone",
            "Explosive Finale",
            "Idol of Greed",
            "Gravity Boots"
        ];

        /// <summary>
        /// Event handler for when we receive an item from the multiworld.
        /// </summary>
        public static void Socket_ReceiveItem(Archipelago.MultiClient.Net.Helpers.ReceivedItemsHelper helper)
        {
            // Get the item the multiworld sent us and handle adding it to our queue.
            SetUpQueue(helper.PeekItem());

            // Dequeue this item.
            helper.DequeueItem();
        }

        /// <summary>
        /// Sets up the queue of items to be received.
        /// </summary>
        /// <param name="item">The item we're handling.</param>
        public static void SetUpQueue(ItemInfo item)
        {
            // Set up a queued item entry for this item.
            ArchipelagoItem queuedItem = new(item.ItemName, item.Player.Name);

            // Set up a boolean to check if we've got an item of the same type and source in the queue already.
            bool foundExistingInQueue = false;

            // Loop through each queued item.
            foreach (ArchipelagoItem itemInQueue in Plugin.itemQueue.Keys)
            {
                // Check if this item's name and source matches this dictionary entry.
                if (itemInQueue.ItemName == queuedItem.ItemName && itemInQueue.Source == queuedItem.Source)
                {
                    // Increment this dictionary entry's count.
                    Plugin.itemQueue[itemInQueue]++;

                    // Set our flag.
                    foundExistingInQueue = true;

                    // Stop the rest of the foreach loop, as its a pointless waste of time now.
                    break;
                }
            }

            // If we haven't found a version of this item in the queue already, then add one.
            if (!foundExistingInQueue)
                Plugin.itemQueue.Add(queuedItem, 1);
        }

        /// <summary>
        /// Event handler for when we receive a DeathLink from the multiworld.
        /// </summary>
        public static void Socket_ReceiveDeathLink(DeathLink deathLink)
        {
            // Set up the message showing our DeathLink source.
            string notifyMessage = string.Empty;

            // Present the cause and source of the DeathLink, assuming we actually have one.
            if (deathLink.Cause != null)
                if (deathLink.Cause != "")
                    notifyMessage = $"{deathLink.Cause}";

            // If we still don't have a notify message set, then pull a generic one.
            if (notifyMessage == string.Empty)
                notifyMessage = GenericDeathLinkReasons[Plugin.rng.Next(GenericDeathLinkReasons.Length)].Replace("$", deathLink.Source);

            // Set the player's DeathLink flags accordingly.
            FPPlayerPatcher.canSendDeathLink = false;
            FPPlayerPatcher.hasBufferedDeathLink = true;

            // Add the message to our sent queue so it'll take priority for the message label.
            Plugin.sentMessageQueue.Add(notifyMessage);
        }

        /// <summary>
        /// Event handler for when we receive a packet from the Multiworld.
        /// </summary>
        public static void Socket_LinkPackets(ArchipelagoPacketBase packet)
        {
            try
            {
                switch (packet)
                {
                    // Unused placeholder to print the packet's type.
                    //default:
                    //    Plugin.consoleLog.LogDebug(packet.PacketType);
                    //    break;

                    // Used by chat messages and server messages.
                    case PrintJsonPacket printJSON:
                        // Don't do any of this if we aren't piping any of the chat through.
                        if (Plugin.configChat.Value == 0)
                            break;

                        // Set up values for the actual message and (if needed) the player name of its origin.
                        string message = "";
                        string player = "";
                        float lengthOffset = 2;

                        // Try and parse this Print JSON packet as a chat message.
                        try
                        {
                            message = SM64Message(((ChatPrintJsonPacket?)printJSON).Message);
                            player = Plugin.session.Players.GetPlayerName(((ChatPrintJsonPacket?)printJSON).Slot);
                            lengthOffset = Math.Max(2f, message.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Length / 2);

                        }

                        // If we failed to parse this packet as a chat message, then handle its raw text data.
                        catch
                        {
                            // Don't do this if we're not piping all the messages through.
                            if (Plugin.configChat.Value != 2)
                                break;

                            // Loop through each part of the message.
                            foreach (JsonMessagePart? jsonData in printJSON.Data)
                            {
                                // Set up a string to hold the text to add to the text box.
                                string textPart = "";

                                // Determine how to handle this message part based on type.
                                switch (jsonData.Type)
                                {
                                    case Archipelago.MultiClient.Net.Enums.JsonMessagePartType.PlayerId:
                                        {
                                            // Read the info of the specified player and set our text value to their name.
                                            var playerInfo = Plugin.session.Players.GetPlayerInfo(int.Parse(jsonData.Text));
                                            textPart = SM64Message(playerInfo.Name);

                                            // If this player is us, then make the text purple, if not, make it yellow.
                                            if (playerInfo.Name == Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot))
                                                textPart = $"<c=purple>{textPart}</c>";
                                            else
                                                textPart = $"<c=yellow>{textPart}</c>";

                                            break;
                                        }

                                    case Archipelago.MultiClient.Net.Enums.JsonMessagePartType.ItemId:
                                        {
                                            // Read the info of the specified player and get the name of the item index the message references.
                                            PlayerInfo playerInfo = Plugin.session.Players.GetPlayerInfo(jsonData.Player.Value);
                                            textPart = SM64Message(Plugin.session.Items.GetItemName(long.Parse(jsonData.Text), playerInfo.Game));

                                            // Set the colour of the text based on the item's flag.
                                            if ((jsonData.Flags & Archipelago.MultiClient.Net.Enums.ItemFlags.Advancement) != 0) textPart = $"<c=purple>{textPart}</c>";
                                            else if ((jsonData.Flags & Archipelago.MultiClient.Net.Enums.ItemFlags.NeverExclude) != 0) textPart = $"<c=blue>{textPart}</c>";
                                            else if ((jsonData.Flags & Archipelago.MultiClient.Net.Enums.ItemFlags.Trap) != 0) textPart = $"<c=orange>{textPart}</c>";
                                            else textPart = $"<c=cyan>{textPart}</c>";

                                            break;
                                        }

                                    case Archipelago.MultiClient.Net.Enums.JsonMessagePartType.LocationId:
                                        {
                                            // Read the info of the specified player and get the name of the location index the message references while also making it green.
                                            PlayerInfo playerInfo = Plugin.session.Players.GetPlayerInfo(jsonData.Player.Value);
                                            textPart = $"<c=green>{SM64Message(Plugin.session.Locations.GetLocationNameFromId(long.Parse(jsonData.Text), playerInfo.Game))}</c>";
                                            break;
                                        }

                                    // For anything else, just add the text value to the message as is.
                                    default:
                                        textPart = SM64Message(jsonData.Text); break;
                                }

                                // Add our text to the message.
                                message += textPart;

                                // Set a lower length offset so these messages don't stay up for as long.
                                lengthOffset = Math.Max(2f, message.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Length / 4);
                            }
                        }

                        // Create a new dialog queue entry based on the data read from the packet.
                        DialogQueue dialogMessage = new()
                        {
                            active = true,
                            text = message,
                            lengthOffset = lengthOffset,
                            name = player,
                            portrait = Plugin.apChatIcon
                        };

                        // If a sprite exists for this player, then replace the generic AP icon with it.
                        if (Plugin.apChatIcons.ContainsKey(dialogMessage.name))
                            dialogMessage.portrait = Plugin.apChatIcons[dialogMessage.name];

                        // Do the SM64 joke to the player's name.
                        dialogMessage.name = SM64Message(player);

                        // Loop through each entry in the text display's queue.
                        for (int queueIndex = 0; queueIndex < Plugin.TextDisplay.GetComponent<PlayerDialog>().queue.Length; queueIndex++)
                        {
                            // If this entry isn't populated already, then add our message to it, mark it as active, then stop looping.
                            if (Plugin.TextDisplay.GetComponent<PlayerDialog>().queue[queueIndex].name == null || (Plugin.TextDisplay.GetComponent<PlayerDialog>().queue[queueIndex].name == string.Empty && Plugin.TextDisplay.GetComponent<PlayerDialog>().queue[queueIndex].text == string.Empty))
                            {
                                Plugin.TextDisplay.GetComponent<PlayerDialog>().queue[queueIndex] = dialogMessage;
                                break;
                            }
                        }
                        break;

                    case BouncedPacket bouncedPacket when bouncedPacket.Tags.Contains("RingLink"):
                        // Ignore the packet if we're the one who sent it or RingLink is disabled.
                        if (bouncedPacket.Data["source"].ToObject<int>() == Plugin.session.ConnectionInfo.Slot || ((long)Plugin.slotData["ring_link"] == 0))
                            return;

                        // Get the value from the RingLink.
                        int ringLinkValue = bouncedPacket.Data["amount"].ToObject<int>();

                        // Check if the player exists.
                        if (FPPlayerPatcher.player != null)
                        {
                            // Disable our send flag so we don't send the RingLink value back.
                            FPSaveManagerPatcher.DisableRingLinkSend = true;

                            // If the RingLink value is positive, then give us that amount of crystals.
                            if (ringLinkValue > 0)
                            {
                                for (int ringIndex = 0; ringIndex < ringLinkValue; ringIndex++)
                                    FPSaveManager.AddCrystal(FPPlayerPatcher.player);
                            }

                            // If the RingLink value is negative, then remove that amount.
                            if (ringLinkValue < 0)
                            {
                                FPPlayerPatcher.player.crystals -= ringLinkValue;
                                FPPlayerPatcher.player.totalCrystals += ringLinkValue;

                                // Clamp the values to 0 and the extra life cost respectively.
                                if (FPPlayerPatcher.player.totalCrystals < 0)
                                    FPPlayerPatcher.player.totalCrystals = 0;
                                if (FPPlayerPatcher.player.crystals > FPPlayerPatcher.player.extraLifeCost)
                                    FPPlayerPatcher.player.crystals = FPPlayerPatcher.player.extraLifeCost;

                                // Check if the player has a shield.
                                if (FPPlayerPatcher.player.shieldHealth > 0)
                                {
                                    // Play the approriate sound effect for the shield.
                                    if (FPPlayerPatcher.player.shieldHealth > 1)
                                        FPAudio.PlaySfx(FPPlayerPatcher.player.sfxShieldHit);
                                    else
                                        FPAudio.PlaySfx(FPPlayerPatcher.player.sfxShieldBreak);

                                    // Reduce the player's shield health.
                                    FPPlayerPatcher.player.shieldHealth -= 1;

                                    // Create the shield hit flash.
                                    ShieldHit shieldHit2 = (ShieldHit)FPStage.CreateStageObject(ShieldHit.classID, FPPlayerPatcher.player.position.x, FPPlayerPatcher.player.position.y);
                                    shieldHit2.SetParentObject(FPPlayerPatcher.player);
                                    shieldHit2.remainingDuration = 60f - Mathf.Min((float)FPPlayerPatcher.player.shieldHealth * 3f, 30f);
                                }

                                // Check if the player has health to lose (a RingLink should never kill).
                                else if (FPPlayerPatcher.player.health > 0)
                                {
                                    // Play the damage sound effect.
                                    FPAudio.PlaySfx(FPPlayerPatcher.player.sfxHurt);

                                    // Either remove a health petal, or floor the health down to 0.
                                    if (FPPlayerPatcher.player.health > 1f)
                                        FPPlayerPatcher.player.health -= 1f;
                                    else
                                        FPPlayerPatcher.player.health = 0;
                                }
                            }

                            // Reenable our send flag.
                            FPSaveManagerPatcher.DisableRingLinkSend = false;
                        }

                        // If the player doesn't exist, then apply the RingLink straight to the save.
                        else
                        {
                            FPSaveManager.totalCrystals += ringLinkValue;

                            // Clamp the value to 0.
                            if (FPSaveManager.totalCrystals < 0)
                                FPSaveManager.totalCrystals = 0;
                        }

                        break;

                    case BouncedPacket bouncedPacket when bouncedPacket.Tags.Contains("TrapLink"):
                        // Ignore the packet if we're the one who sent it or TrapLink is disabled.
                        if (bouncedPacket.Data["source"].ToObject<string>() == Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot) || ((long)Plugin.slotData["trap_link"] == 0))
                            return;

                        // Get the name of the trap from the packet.
                        string trapName = bouncedPacket.Data["trap_name"].ToObject<string>();

                        // Check if we have an entry for this trap name.
                        if (TrapLinkMappings.ContainsKey(trapName))
                        {
                            // Check that we have any mappings for this trap name.
                            if (TrapLinkMappings[trapName].Length > 0)
                            {
                                // Pick a trap from the mappings array.
                                string targetTrap = TrapLinkMappings[trapName][Plugin.rng.Next(TrapLinkMappings[trapName].Length)];

                                // If our chosen trap requires the player, then abort.
                                if (TrapLinkRequiresPlayer.Contains(targetTrap) && (FPPlayerPatcher.player == null || !FPStage.objectsRegistered))
                                    return;

                                // If our chosen trap links to a Potion Seller mod item but the mod is missing, then also abort.
                                if (PotionSellerTrapLink.Contains(targetTrap) && !Chainloader.PluginInfos.ContainsKey("com.eps.plugin.fp2.potion-seller"))
                                    return;

                                // Add this trap item and a message to the queue.
                                Plugin.TrapLinks.Add(new ArchipelagoItem(targetTrap, bouncedPacket.Data["source"].ToObject<string>()));
                                Plugin.sentMessageQueue.Add($"{bouncedPacket.Data["source"]} linked a {trapName}!");
                                break;
                            }
                        }

                        // If there wasn't an entry, or was one with no mappings, then put a notice in the console.
                        Plugin.consoleLog.LogWarning($"No TrapLink mappings found for '{trapName}'.");
                        break;

                    case BouncedPacket bouncedPacket when bouncedPacket.Tags.Contains("SharedDamage"):
                        // Check that the damage source isn't us and that we actually have DamageLink enabled.
                        if (bouncedPacket.Data["source"].ToObject<string>() == Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot) || ((long)Plugin.slotData["damage_link"] == 0))
                            return;

                        // Check that the player exists.
                        if (FPPlayerPatcher.player != null)
                        {
                            // Take the damage_points value, divide it by 10 (lining up with Megaman X, where 10 points is 1 blip on the health bar) and add it to the player's healthDamage value.
                            FPPlayerPatcher.player.healthDamage += bouncedPacket.Data["damage_points"].ToObject<float>() / 10;

                            // Force run the Hurt action on the player to apply the damage (or let them guard through it if they can pull that off).
                            FPPlayerPatcher.player.Action_Hurt();
                        }

                        break;
                }
            }
            catch (Exception e)
            {
                Plugin.consoleLog.LogError($"Socket event errored with the exception!\r\n\t{e}");
            }
        }
    
        private static string SM64Message(string input)
        {
            // Only bother doing this if the Mario 64 Text option is enabled.
            if (Plugin.configSM64Text.Value)
            {
                // Set a default last character.
                char lastCharacter = 'X';

                // Create a StringBuilder out of the input string.
                System.Text.StringBuilder strBuilder = new(input);

                // Loop through each character in the StringBuilder.
                for (int characterIndex = 0; characterIndex < strBuilder.Length; characterIndex++)
                {
                    // Check if this character is one that's missing from the Mario 64 font.
                    if (Char.ToUpper(strBuilder[characterIndex]) is '!' or '#' or '%' or '&' or '*' or '+' or ',' or '-' or '.' or '?' or 'J' or 'Q' or 'V' or 'X' or 'Z')
                    {
                        // Type the previously stored character, converting its case if needed.
                        if (Char.IsUpper(strBuilder[characterIndex])) strBuilder[characterIndex] = Char.ToUpper(lastCharacter);
                        else strBuilder[characterIndex] = Char.ToLower(lastCharacter);
                    }
                    else
                    {
                        // Update the last character if this isn't a space.
                        if (strBuilder[characterIndex] != ' ')
                            lastCharacter = strBuilder[characterIndex];
                    }
                }

                // Return the finalised string.
                return strBuilder.ToString();
            }

            // Return the input if we're not doing the edit.
            return input;
        }
    }
}
