using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Freedom_Planet_2_Archipelago.Patchers;
using System.Linq;

namespace Freedom_Planet_2_Archipelago
{
    internal class SocketEvents
    {
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
            string notifyMessage = $"DeathLink received from {deathLink.Source}";

            // Present the cause and source of the DeathLink.
            if (deathLink.Cause != null)
                notifyMessage = $"{deathLink.Cause}";

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
            switch (packet)
            {
                case ChatPrintJsonPacket printJSON:

                    // Create a new dialog queue entry based on the packet's data.
                    DialogQueue dialogMessage = new()
                    {
                        active = true,
                        text = printJSON.Message,
                        lengthOffset = Math.Max(2f, printJSON.Message.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Length / 2),
                        name = Plugin.session.Players.GetPlayerName(printJSON.Slot),
                        portrait = Plugin.apChatIcon
                    };

                    // If a sprite exists for this player, then replace the generic AP icon with it.
                    if (Plugin.apChatIcons.ContainsKey(dialogMessage.name))
                        dialogMessage.portrait = Plugin.apChatIcons[dialogMessage.name];

                    // Loop through each entry in the Aaa Trap's queue.
                    for (int queueIndex = 0; queueIndex < Plugin.AaaTrap.GetComponent<PlayerDialog>().queue.Length; queueIndex++)
                    {
                        // If this entry isn't populated already, then add a random line to it, mark it as active, then stop looping.
                        if (Plugin.AaaTrap.GetComponent<PlayerDialog>().queue[queueIndex].name == null)
                        {
                            Plugin.AaaTrap.GetComponent<PlayerDialog>().queue[queueIndex] = dialogMessage;
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

                    // Whether or not we've been in a position to receive the linked trap.
                    bool received = false;

                    // Selects a trap at random from a set.
                    void RandomTrap(Dictionary<string, bool> traps)
                    {
                        KeyValuePair<string, bool> trap = traps.ElementAt(Plugin.rng.Next(traps.Count));
                        AddTrap(bouncedPacket, trap.Key, trap.Value);
                    }

                    // Handle the traps and what they should link to.
                    switch (bouncedPacket.Data["trap_name"].ToObject<string>())
                    {
                        // Brave Stones.
                        case "Double Damage": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "Double Damage", true); break;
                        case "Expensive Stocks": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "Expensive Stocks", true); break;
                        case "Items To Bombs": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "Items To Bombs", true); break;
                        case "Life Oscillation": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "Life Oscillation", true); break;
                        case "No Guarding": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "No Guarding", true); break;
                        case "No Petals": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "No Petals", true); break;
                        case "No Revivals": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "No Revivals", true); break;
                        case "No Stocks": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "No Stocks", true); break;
                        case "One Hit KO": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "One Hit KO", true); break;
                        case "Time Limit": if ((long)Plugin.slotData["trap_stones"] != 0) AddTrap(bouncedPacket, "Time Limit", true); break;

                        // FP2 Traps.
                        case "Swap Trap": AddTrap(bouncedPacket, "Swap Trap", true); break;
                        case "Mirror Trap": AddTrap(bouncedPacket, "Mirror Trap"); break;
                        case "Pie Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Spring Trap": AddTrap(bouncedPacket, "Spring Trap", true); break;
                        case "PowerPoint Trap": AddTrap(bouncedPacket, "PowerPoint Trap"); break;
                        case "Zoom Trap": AddTrap(bouncedPacket, "Zoom Trap"); break;
                        case "Aaa Trap": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "Spike Ball Trap": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "Pixellation Trap": AddTrap(bouncedPacket, "Pixellation Trap"); break;
                        case "Rail Trap": AddTrap(bouncedPacket, "Rail Trap"); break;

                        // None FP2 Based Traps, sourced from https://docs.google.com/spreadsheets/d/1yoNilAzT5pSU9c2hYK7f2wHAe9GiWDiHFZz8eMe1oeQ/edit?gid=811965759#gid=811965759.
                        case "144p Trap": AddTrap(bouncedPacket, "Pixellation Trap", true); break;
                        case "Army Trap": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "Banana Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Banana Peel Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Bee Trap": AddTrap(bouncedPacket, "Zoom Trap"); break;
                        case "Blue Balls Curse": AddTrap(bouncedPacket, "One Hit KO", true); break;
                        case "Bomb": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "Bonk Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Bubble Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Buyon Trap": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "Camera Rotate Trap": AddTrap(bouncedPacket, "Mirror Trap"); break;
                        case "Chaos Control Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Confound Trap": AddTrap(bouncedPacket, "Pixellation Trap"); break;
                        case "Confuse Trap": AddTrap(bouncedPacket, "Pixellation Trap"); break;
                        case "Confusion Trap": AddTrap(bouncedPacket, "Pixellation Trap"); break;
                        case "Controller Drift Trap": AddTrap(bouncedPacket, "Rail Trap"); break;
                        case "Cutscene Trap": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "Damage Trap": AddTrap(bouncedPacket, "Life Oscillation", true); break;
                        case "Deisometric Trap":
                            RandomTrap(new()
                            {
                                { "Zoom Trap", false },
                                { "Mirror Trap", false }
                            });
                            break;
                        case "Disable A Trap": AddTrap(bouncedPacket, "No Guarding", true); break;
                        case "Disable B Trap": AddTrap(bouncedPacket, "No Guarding", true); break;
                        case "Disable C Up Trap": AddTrap(bouncedPacket, "No Guarding", true); break;
                        case "Disable Z Trap": AddTrap(bouncedPacket, "No Guarding", true); break;
                        case "Dry Trap": AddTrap(bouncedPacket, "No Stocks", true); break;
                        case "Eject Ability": AddTrap(bouncedPacket, "Swap Trap", true); break;
                        case "Electrocution Trap": AddTrap(bouncedPacket, "Life Oscillation", true); break;
                        case "Exposition Trap": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "Fake Transition": AddTrap(bouncedPacket, "Zoom Trap"); break;
                        case "Fast Trap": AddTrap(bouncedPacket, "Rail Trap"); break;
                        case "Fear Trap": AddTrap(bouncedPacket, "Rail Trap"); break;
                        case "Fire Trap": AddTrap(bouncedPacket, "Life Oscillation", true); break;
                        case "Flip Trap": AddTrap(bouncedPacket, "Mirror Trap"); break;
                        case "Freeze Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Frog Trap": AddTrap(bouncedPacket, "Swap Trap", true); break;
                        case "Frozen Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Fuzzy Trap": AddTrap(bouncedPacket, "Pixellation Trap"); break; // Doesn't have a source or effect listed on the sheet? Thinking of Yoshi's Island?
                        case "Gadget Shuffle Trap": AddTrap(bouncedPacket, "Swap Trap", true); break;
                        case "Get Out Trap": AddTrap(bouncedPacket, "Time Limit", true); break;
                        case "Ghost": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "Ghost Chat": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "Hiccup Trap": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "Honey Trap": AddTrap(bouncedPacket, "Rail Trap"); break;
                        case "Ice Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Ice Floor Trap": AddTrap(bouncedPacket, "Rail Trap"); break;
                        case "Icy Hot Pants Trap": AddTrap(bouncedPacket, "Spring Trap", true); break;
                        case "Input Sequence Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Instant Death Trap": AddTrap(bouncedPacket, "One Hit KO", true); break;
                        case "Laughter Trap": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "Literature Trap": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "Meteor Trap": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "My Turn! Trap": AddTrap(bouncedPacket, "Mirror Trap"); break;
                        case "No Vac Trap": AddTrap(bouncedPacket, "No Guarding", true); break;
                        case "Nut Trap": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "OmoTrap": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "Paralyze Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Phone Trap": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "Poison Mushroom": AddTrap(bouncedPacket, "Zoom Trap"); break;
                        case "Poison Trap": AddTrap(bouncedPacket, "Life Oscillation", true); break;
                        case "Police Trap": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "Posession Trap": AddTrap(bouncedPacket, "Life Oscillation", true); break;
                        case "Push Trap": AddTrap(bouncedPacket, "Rail Trap"); break;
                        case "Reversal Trap": AddTrap(bouncedPacket, "Mirror Trap"); break;
                        case "Reverse Trap": AddTrap(bouncedPacket, "Mirror Trap"); break;
                        case "Screen Flip Trap": AddTrap(bouncedPacket, "Mirror Trap"); break;
                        case "Slip Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Slow Trap": AddTrap(bouncedPacket, "PowerPoint Trap"); break;
                        case "Slowness Trap": AddTrap(bouncedPacket, "PowerPoint Trap"); break;
                        case "Spooky Time": AddTrap(bouncedPacket, "Zoom Trap"); break;
                        case "Squash Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "Stun Trap": AddTrap(bouncedPacket, "Pie Trap", true); break;
                        case "SvC Effect":
                            RandomTrap(new()
                            {
                                { "Zoom Trap", false },
                                { "Mirror Trap", false },
                                { "Powerpoint Trap", false },
                                { "Pie Trap", true},
                                { "Swap Trap", true},
                                { "Spring Trap", true},
                                { "Aaa Trap", true},
                            });
                            break;
                        case "Thwimp Trap": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "Timer Trap": AddTrap(bouncedPacket, "Time Limit", true); break;
                        case "Time Warp Trap": AddTrap(bouncedPacket, "Time Limit", true); break;
                        case "Tiny Trap": AddTrap(bouncedPacket, "Zoom Trap"); break;
                        case "Tip Trap": AddTrap(bouncedPacket, "Aaa Trap"); break;
                        case "TNT Barrel Trap": AddTrap(bouncedPacket, "Spike Ball Trap", true); break;
                        case "W I D E Trap": AddTrap(bouncedPacket, "Zoom Trap"); break;

                        default: Plugin.consoleLog.LogDebug($"No trap matchup found for {bouncedPacket.Data["source"]}'s {bouncedPacket.Data["trap_name"]}!"); break;
                    }

                    if (received)
                        Plugin.sentMessageQueue.Add($"{bouncedPacket.Data["source"]} linked a {bouncedPacket.Data["trap_name"]}!");

                    void AddTrap(BouncedPacket bouncedPacket, string trapName, bool requiresPlayer = false)
                    {
                        // If this trap requires a player, then check for one and don't add it if they don't exist.
                        if (requiresPlayer && (FPPlayerPatcher.player == null || !FPStage.objectsRegistered))
                            return;

                        Plugin.TrapLinks.Add(new ArchipelagoItem(trapName, bouncedPacket.Data["source"].ToObject<string>()));
                        received = true;
                    }

                    break;
            }
        }
    }
}
