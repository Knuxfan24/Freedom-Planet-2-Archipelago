using Archipelago.MultiClient.Net.Helpers;
using System.Text.RegularExpressions;

namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class SpamTrap : FPBaseObject
    {
        /// <summary>
        /// Valid placeholder types for replacing parts of a string in the header or message body.
        /// </summary>
        private enum PlaceholderTypes
        {
            RandomName, // Picks any name from the server.
            RandomNameNoServer, // Excludes the word "Server".
            RandomNameNotOurs, // Excludes our own name.
            RandomNameNotOursOrServer, // Excludes both "Server" and our own name.
            OurName // Shows our own name.
        }

        private class SpamTrapMessage(string? header, string message)
        {
            /// <summary>
            /// Text shown in the header. If this is null, then the header is removed entirely and the message body shifted up.
            /// </summary>
            public string? Header = header;

            /// <summary>
            /// Text shown in the message body.
            /// </summary>
            public string Message { get; set; } = message;

            /// <summary>
            /// Placeholder types for replacing text in the header or body.
            /// </summary>
            public List<PlaceholderTypes> Placeholders = [];

            // Initialiser that includes placeholders.
            public SpamTrapMessage(string? header, string message, List<PlaceholderTypes> placeholders) : this(header, message) => Placeholders = placeholders;
        }

        // The various messages that can be picked for display.
        private static readonly SpamTrapMessage[] messages =
        [
            new("NOTICE", "We've been trying\r\nto reach you\r\nregarding your\r\nbike's extended\r\nwarranty."),
            new("Beauty Contest", "You have won second\r\nprize in a\r\nbeauty contest\r\n\r\nCollect $10"),
            new("CONGRATULATIONS", "YOU'RE THE 50,000TH\r\nVISITOR TO ZAO LAND!\r\n\r\nCLICK HERE TO CLAIM\r\nYOUR PRIZE!"),
            new("Advertisment", "Half price entry to\r\nForest Frontiers.\r\n\r\nAvaliable while\r\nstocks last."), // Reference to OpenRCT2.
            new(null, "You won't get tired\r\nof my voice will you?\r\nYou won't get tired\r\nof my voice will you?\r\nYou won't get tired\r\nof my voice will you?\r\nYou won't get tired\r\nof my voice will you?"), // Reference to FNaF World.
            new(null, "AAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA"),
            new("Adventure Awaits", "Explore the Keep\r\ntoday.\r\n\r\nThis message\r\nsponsored by\r\nThe Keymaster."), // Reference to Keymaster's Keep.
            new("Buzz Cola", "For humans!\r\n\r\nIsn't posionous to\r\nanybody!\r\n(that we know of...)"), // Reference to The Simpsons: Hit and Run.
            new("Advertisment", "75% off your next\r\npurchase at JojaMart."), // Reference to Stardew Valley.
            new(null, "Receiving this\r\nSpam Trap...\r\n\r\nIt fills you with\r\ndetermination."), // Reference to Undertale
            new(null, "You feel an evil\r\npresence watching\r\nyou..."), // Reference to Terraria.
            new("A MYURRDERRRR?!", "ON MY OWL EXPRESS?!"), // Reference to A Hat in Time.
            new(null, "eastmost peninsula\r\nis the secret"), // Reference to The Legend of Zelda.
            new("CONGRATULATIONS", "You've won your\r\nvery own mansion.\r\n\r\nClick here for\r\ndetails!"), // Reference to Luigi's Mansion.
            new("Thief Alert!", "The word\r\n{$},\r\nthey stole it too!", [PlaceholderTypes.RandomName]), // Reference to Kingdom Hearts 2.
            new(null, "You want fun?\r\n{$}\r\nwill show you fun...", [PlaceholderTypes.RandomNameNotOursOrServer]),
            new("Did You Know?", "There's a Mew\r\nunder the truck."), // Reference to Pokémon Red.
            new(null, "i showed you my\r\ncacodemon plz\r\nrespond"), // Reference to Doom.
            new("Message from Ghandi", "Our words are backed\r\nby nuclear weapons!"), // Reference to Civilization.
            new("Did You Know?", "Metal Harbor is\r\nactually beatable\r\nwithout the\r\nLight Shoes!"), // Reference to Sonic Adventure 2: Battle.
            new("Shrine of Chance", "You offer to the\r\nshrine, but gain\r\nnothing."), // Reference to Risk of Rain 2.
            new("The Ocean", "Now with 75%\r\nmore Leviathan!"), // Reference to Subnautica
            new(null, "This advert\r\ndedicated to those\r\nwho perished on\r\nthe climb..."), // Reference to Celeste
            new("Need Reception?", "Climb to the top\r\nof Hawk Peak!"), // Reference to A Short Hike.
            new("AURORA BOREALIS", "At this time of year?\r\nAt this time of day?\r\nIn this part of the\r\nmutliworld?\r\n\r\nLocalised entirely\r\nwithin your slot data?!"), // Reference to that Simpsons meme.
            new("Dear {$}", "Please come to the\r\ncastle. I've baked\r\na cake for you.\r\nYours truly--\r\nPrincess Toadstool", [PlaceholderTypes.OurName]), // Reference to Super Mario 64.
            new("ALERT", "{$}\r\nhas died in an\r\naccident on\r\nSteeplechase 1!", [PlaceholderTypes.RandomName]), // Also a reference to OpenRCT2.
            new("ACCESS DENIED", "Adam has yet to\r\nauthorise usage\r\nof this Spam Trap."), // Reference to Metroid: Other M, which isn't in AP (or even has a Randomiser) but oh well.
            new("FACT", "The square root\r\nof rope is string."), // Reference to Portal 2
            new("ACT QUICKLY!", "Local TESTIFICATE\r\nlooking for local\r\nadventurer to\r\ntrade with in\r\nyour area!"), // Reference to Minecraft.
            new(null, "What is a man?\r\nA miserable little\r\npile of secrets!"), // Reference to Castlevania: Symphony of the Night
            new(null, "Yer' treasure\r\nchest's looking a\r\nbit light boy!"), // Reference to Spongebob Squarepants: Battle for Bikini Bottom.
            new("Zoe", "I'm sorry,\r\n{$},\r\nbut you seem to be\r\nplaying a hacked\r\nversion of this\r\ngame.", [PlaceholderTypes.OurName]), // Reference to Spyro 3.
            new(null, "IT'S JUST\r\nA BIG NOSE BUSH"), // Reference to Rayman 2.
            new(null, "Local boy discovers\r\nfriends are power.\r\n\r\nSword responds\r\nwith confusion."), // Reference to Kingdom Hearts.
            new(null, "DeathLink received\r\nfrom {$}?", [PlaceholderTypes.RandomNameNotOursOrServer]),
            new("Exciting Tournament!", "Not just a race...\r\nBut a special race,\r\nto see who's the\r\nfastest!"), // Reference to Sonic Riders.
            new(null, "KTOX TV reports\r\nDangerous Games\r\ndelayed due to\r\nDigger related\r\nincidents."), // Reference to Megaman Legends.
            new(null, "Blue haired CEO\r\nforces castle\r\nvisitors to play\r\ncard games.\r\n\r\nExperts still\r\nconfused."), // Reference to Kingdom Hearts: Chain of Memories with a slight Birth by Sleep reference too.
            new(null, "\"Barrier continues\r\nto hold\" reports\r\nfrustrated conductor."), // Reference to The Legend of Zelda: Wind Waker.
            new("Wheel of Fortune", "NOPE!"), // Reference to Balatro.
        ];

        // The valid colours to tint the background.
        private static readonly UnityEngine.Color[] colours =
        [
            UnityEngine.Color.black,
            UnityEngine.Color.white, // Acts as blue due to the background already being blue.
            UnityEngine.Color.cyan,
            UnityEngine.Color.green,
            UnityEngine.Color.magenta,
            UnityEngine.Color.red,
            UnityEngine.Color.yellow
        ];

        // FPBaseObject stuff.
        private static int classID = -1;
        private FPObjectState state;
        private bool isValidatedInObjectList;

        // A timer that counts down to destroy this spam trap.
        private float genericTimer = 5;

        // The values for the text in the actual spam trap.
        private string? header;
        private string message = "";
        private List<PlaceholderTypes> placeholders = [];
        private int placeholderIndex = 0;

        // Debug specific message for testing the placeholders.
        private readonly SpamTrapMessage DebugMessage = new("***DEBUG for {$}***",
                                                            "Random Name: {$}\r\nNot Server: {$}\r\nNot Us: {$}\r\nNeither: {$}\r\nUs: {$}",
                                                            [PlaceholderTypes.OurName, PlaceholderTypes.RandomName, PlaceholderTypes.RandomNameNoServer, PlaceholderTypes.RandomNameNotOurs, PlaceholderTypes.RandomNameNotOursOrServer, PlaceholderTypes.OurName]);

        private new void Start()
        {
            // Reset the placeholder index.
            placeholderIndex = 0;

            state = State_Default;

            // Start the FPBaseObject setup.
            base.Start();
            classID = FPStage.RegisterObjectType(this, GetType(), 0);
            objectID = classID;

            // Force this script to always be active.
            activationMode = FPActivationMode.ALWAYS_ACTIVE;

            // Randomly set the timer to a value between 3 and 5.
            genericTimer = Plugin.rng.Next(3, 6);

            // Select the message to display.
            int messageIndex = Plugin.rng.Next(messages.Length);

            // Load our header and message.
            header = messages[messageIndex].Header;
            message = messages[messageIndex].Message;
            placeholders = messages[messageIndex].Placeholders;

            // DEBUG: Replace the stuff with our debug message. Only use this if adding new placeholder types.
            //header = DebugMessage.Header;
            //message = DebugMessage.Message;
            //placeholders = DebugMessage.Placeholders;

            // Get the names of the players in this multiworld.
            List<string> playerNames = [];
            foreach (PlayerInfo? player in Plugin.session.Players.AllPlayers)
                playerNames.Add(player.Name);

            // Swap out any placeholders the header and message may have.
            if (header != null) header = ReplacePlaceholders(header, placeholders);
            message = ReplacePlaceholders(message, placeholders);

            // Select a colour for the background.
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colours[Plugin.rng.Next(colours.Length)];

            // If the target message has a header, then just update its text.
            // If not, then hide the header element and shift the body element up by 8 pixels.
            if (header != null)
                gameObject.transform.GetChild(1).GetComponent<TextMesh>().text = header;
            else
            {
                gameObject.transform.GetChild(1).gameObject.SetActive(false);
                gameObject.transform.GetChild(2).transform.localPosition = new(128.5f, -80f, 0f);
            }

            // Update the body element's text.
            gameObject.transform.GetChild(2).GetComponent<TextMesh>().text = message;
        }

        private void Update()
        {
            // Validate this object in the stage list if it hasn't already been.
            if (!isValidatedInObjectList && FPStage.objectsRegistered)
                isValidatedInObjectList = FPStage.ValidateStageListPos(this);

            // Invoke the current state if it isn't null.
            state?.Invoke();
        }

        private void State_Default()
        {
            // Decrement our timer by the game's delta time.
            genericTimer -= Time.deltaTime;

            // Check if we've reached 0 on our timer.
            if (genericTimer <= 0)
            {
                // Kill this spam trap's object.
                Destroy(this.gameObject);

                // Decrement the spam trap count.
                Plugin.SpamTrapCount--;

                // If the spam trap count is still above 0, then spawn a new one.
                if (Plugin.SpamTrapCount > 0)
                    Helpers.SpawnSpamTrap();
            }
        }

        private string ReplacePlaceholders(string text, List<PlaceholderTypes> placeholders)
        {
            // If we only have at most two players (likely our own name and the server), then force replace RandomNameNotOursOrServer with RandomName.
            if (Plugin.session.Players.AllPlayers.Count() <= 2)
                for (int placeholderIndex = 0; placeholderIndex < placeholders.Count; placeholderIndex++)
                    if (placeholders[placeholderIndex] == PlaceholderTypes.RandomNameNotOursOrServer)
                        placeholders[placeholderIndex] = PlaceholderTypes.RandomName;

            // Split the string on the {$} indicators.
            string[] split = Regex.Split(text, "({\\$})");

            // Loop through each split.
            for (int splitIndex = 0; splitIndex < split.Length; splitIndex++)
            {
                // Check that this split is a placeholder one.
                if (split[splitIndex] == "{$}")
                {
                    // Check that we haven't got more placeholders than we actually called for.
                    if (placeholderIndex >= placeholders.Count)
                    {
                        Plugin.consoleLog.LogError($"Spam Trap value '{text}' had more placeholders than defined!");
                        break;
                    }

                    // Determine what to do based on our current placeholder's type.
                    switch (placeholders[placeholderIndex])
                    {
                        // Pick a random name from the player list.
                        case PlaceholderTypes.RandomName:
                            split[splitIndex] = Plugin.session.Players.AllPlayers.ToArray()[Plugin.rng.Next(Plugin.session.Players.AllPlayers.ToArray().Length)].Name;
                            break;

                        // Force our split to "Server", then select from the player list until we pick something else.
                        case PlaceholderTypes.RandomNameNoServer:
                            split[splitIndex] = "Server";

                            while (split[splitIndex] == "Server")
                                split[splitIndex] = Plugin.session.Players.AllPlayers.ToArray()[Plugin.rng.Next(Plugin.session.Players.AllPlayers.ToArray().Length)].Name;
                            break;

                        // Force our split to our slot name, then select from the player list until we pick something valid.
                        case PlaceholderTypes.OurName:
                        case PlaceholderTypes.RandomNameNotOurs:
                        case PlaceholderTypes.RandomNameNotOursOrServer:
                            split[splitIndex] = Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot);

                            if (placeholders[placeholderIndex] is PlaceholderTypes.RandomNameNotOurs)
                                while (split[splitIndex] == Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot))
                                    split[splitIndex] = Plugin.session.Players.AllPlayers.ToArray()[Plugin.rng.Next(Plugin.session.Players.AllPlayers.ToArray().Length)].Name;

                            if (placeholders[placeholderIndex] is PlaceholderTypes.RandomNameNotOursOrServer)
                                while (split[splitIndex] == Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot) || split[splitIndex] == "Server")
                                    split[splitIndex] = Plugin.session.Players.AllPlayers.ToArray()[Plugin.rng.Next(Plugin.session.Players.AllPlayers.ToArray().Length)].Name;
                            break;

                        // Log an error if we haven't handled this placeholder type.
                        default: Plugin.consoleLog.LogError($"Placeholder type {placeholders[placeholderIndex]} not handled!"); break;
                    }

                    // Increment our placeholder index.
                    placeholderIndex++;
                }
            }

            // Return our edited string.
            return String.Join("", split);
        }
    }
}
