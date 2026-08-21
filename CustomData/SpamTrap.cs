using Archipelago.MultiClient.Net.Helpers;
using System.Text.RegularExpressions;

namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class SpamTrap : FPBaseObject
    {
        /// <summary>
        /// Valid placeholder types for replacing parts of a string in the header or message body.
        /// </summary>
        public enum PlaceholderTypes
        {
            RandomName, // Picks any name from the server.
            RandomNameNoServer, // Excludes the word "Server".
            RandomNameNotOurs, // Excludes our own name.
            RandomNameNotOursOrServer, // Excludes both "Server" and our own name.
            OurName, // Shows our own name.
            RandomString // Picks a random string from a provided set.
        }

        public class SpamTrapMessage(string? header, string message)
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

            /// <summary>
            /// Strings that can be used to fill in the RandomString placeholder type.
            /// </summary>
            public List<string> PlaceholderStrings = [];

            // Initialiser that includes placeholders.
            public SpamTrapMessage(string? header, string message, List<PlaceholderTypes> placeholders) : this(header, message) => Placeholders = placeholders;
            public SpamTrapMessage(string? header, string message, List<PlaceholderTypes> placeholders, List<string> placeholderStrings) : this(header, message)
            {
                Placeholders = placeholders;
                PlaceholderStrings = placeholderStrings;
            }
        }

        // The various messages that can be picked for display.
        public static readonly SpamTrapMessage[] messages =
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
            new(null, "Receiving this\r\nSpam Trap...\r\n\r\nIt fills you with\r\ndetermination."), // Reference to Undertale.
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
            new("The Ocean", "Now with 75%\r\nmore Leviathan!"), // Reference to Subnautica.
            new(null, "This advert\r\ndedicated to those\r\nwho perished on\r\nthe climb..."), // Reference to Celeste
            new("Need Reception?", "Climb to the top\r\nof Hawk Peak!"), // Reference to A Short Hike.
            new("AURORA BOREALIS", "At this time of year?\r\nAt this time of day?\r\nIn this part of the\r\nmutliworld?\r\n\r\nLocalised entirely\r\nwithin your slot data?!"), // Reference to that Simpsons meme.
            new("Dear {$}", "Please come to the\r\ncastle. I've baked\r\na cake for you.\r\nYours truly--\r\nPrincess Toadstool", [PlaceholderTypes.OurName]), // Reference to Super Mario 64.
            new("ALERT", "{$}\r\nhas died in an\r\naccident on\r\nSteeplechase 1!", [PlaceholderTypes.RandomName]), // Reference to OpenRCT2.
            new("ACCESS DENIED", "Adam has yet to\r\nauthorise usage\r\nof this Spam Trap."), // Reference to Metroid: Other M.
            new("FACT", "The square root\r\nof rope is string."), // Reference to Portal 2.
            new("ACT QUICKLY!", "Roaming TESTIFICATE\r\nlooking for local\r\nadventurer to\r\ntrade with in\r\nyour area!"), // Reference to Minecraft.
            new(null, "What is a man?\r\nA miserable little\r\npile of secrets!"), // Reference to Castlevania: Symphony of the Night.
            new(null, "Yer' treasure\r\nchest's looking a\r\nbit light boy!"), // Reference to Spongebob Squarepants: Battle for Bikini Bottom.
            new("Zoe", "I'm sorry,\r\n{$},\r\nbut you seem to be\r\nplaying a hacked\r\nversion of this\r\ngame.", [PlaceholderTypes.OurName]), // Reference to Spyro 3.
            new(null, "IT'S JUST\r\nA BIG NOSE BUSH"), // Reference to Rayman 2.
            new(null, "Local boy discovers\r\nfriends are power.\r\n\r\nSword responds\r\nwith confusion."), // Reference to Kingdom Hearts.
            new(null, "DeathLink received\r\nfrom {$}?", [PlaceholderTypes.RandomNameNotOursOrServer]),
            new("Exciting Tournament!", "Not just a race...\r\nBut a special race,\r\nto see who's the\r\nfastest!"), // Reference to Sonic Riders.
            new("KTOX TV Report", "Dangerous Games\r\ndelayed due to\r\nDigger related\r\nincidents."), // Reference to Megaman Legends.
            new(null, "Blue haired CEO\r\nforces castle\r\nvisitors to play\r\ncard games.\r\n\r\nExperts still\r\nconfused."), // Reference to Kingdom Hearts: Chain of Memories with a slight Birth by Sleep reference too.
            new(null, "\"Barrier continues\r\nto hold\" reports\r\nfrustrated conductor."), // Reference to The Legend of Zelda: Wind Waker.
            new("Wheel of Fortune", "NOPE!"), // Reference to Balatro.
            new("COMING SOON!", "Something\r\nNew\r\n'n'\r\nTasty"), // Reference to Oddworld: Abe's Oddysee.
            new("Hey all!", "{$}\r\nhere!", [PlaceholderTypes.RandomNameNotOurs]), // Reference to Scott the Woz.
            new(null, "The train headed\r\nfor the Mystic Ruins\r\nwill be departing\r\nsoon."), // Reference to Sonic Adventure.
            new("FOOOOOOOOOOOOOOL!!", "You blew it!\r\nYou've totally\r\nscrewed yourself!\r\nNobody enters my\r\nhome and leaves in\r\none piece!"), // Reference to A Hat in Time.
            new("BREAKING NEWS", "Regional Chuckola\r\nreserves low!\r\n\r\nTwo brothers are on\r\nthe scene to\r\ninvestigate!"), // Reference to Mario & Luigi: Superstar Saga.
            new("TRAGEDY", "Local resident dies\r\ndue to mysterious\r\npool ladder related\r\nincident.\r\n{$} denies\r\ninvolvement.", [PlaceholderTypes.RandomNameNoServer]), // Reference to The Sims.
            new(null, "The only thing\r\nthey fear is\r\n{$}.", [PlaceholderTypes.RandomName]), // Reference to Doom.
            new(null, "According to all\r\nknown laws of\r\naviation, there is\r\nno way a Milla\r\nshould be able\r\nto fly."), // Reference to the Bee Movie copy pasta.
            new("ArchipelaVPN", "For all your\r\nprivacy* needs.\r\n\r\n\r\n*privacy not\r\nguaranteed"), // Reference to VPNs being a common YouTube sponsorship.
            new("{$}cast", "Welcome.\r\nWelcome to City 17.\r\nYou have chosen, or\r\nbeen chosen, to\r\nrelocate to one of\r\nour finest remaining\r\nurban centers.", [PlaceholderTypes.RandomNameNotOurs]), // Reference to Half-Life 2.
            new(null, "Thank you\r\n{$}!\r\n\r\nBut our princess is\r\nin another\r\nmultiworld!", [PlaceholderTypes.OurName]),
            new("CATS:", "How are you\r\ngentlemen !!\r\n\r\nAll your base\r\nare belong to us."), // Reference to Zero Wing.
            new(null, "Boy gets beaten in\r\nfoot race by one\r\nsecond despite\r\nspeedrunner\r\ntechniques."), // Reference to The Legend of Zelda: Ocarina of Time.
            new("{$}", "I would just love it\r\nif there was a\r\nVending Machine \r\nright here!", [PlaceholderTypes.RandomNameNotOursOrServer]), // Reference to Tomadachi Life.
            new("CONTROVERSY!", "Local \"More Gun\"\r\nadvocate caught\r\nappreciating \"A\r\nLittle Less Gun\""), // Reference to Team Fortress 2.
            new(null, "Supposed \"Greatest\r\nPlan\" turned out\r\nto be not so great.\r\n\r\nPilot unavailable\r\nfor comment."), // Reference to The Henry Stickmin Collection.
            new(null, "Random block of Tofu\r\ncalls it quits,\r\ncites concern over\r\namount of Buzzsaws\r\nand Salt.\r\nMeat lovers apathetic\r\nat this announcement."), // Reference to Super Meat Boy.
            new("SHOCKING DISCOVERY", "Floor Ice Cream\r\nallegedly gives\r\nhealth.\r\n\r\nExperts still deny\r\nthe angel's claim."), // Reference to Kid Icarus Uprising.
            new(null, "{$}\r\nwins by doing\r\nabsolutely nothing.", [PlaceholderTypes.RandomNameNotOursOrServer]), // Reference to the Luigi Wins meme.
            new(null, "Bazelgeuse reported\r\nin the area. Local\r\nHunters traumatised\r\nby horns."), // Reference to Monster Hunter World.
            new(null, "Local millionaire\r\nsurprisingly\r\napologises after\r\ntaunting waiting\r\npatrons for over\r\nfifty minutes."), // Reference to Wario World.
            new(null, "Squids continue to\r\nargue over mundane\r\nchoices.\r\n\r\nNewly arriving\r\nOctopi left confused."), // Reference to Splatoon.
            new(null, "Greenland closes\r\nall access.\r\n\r\nLocal residents\r\nfeel oddly smug."), // Reference to Plague Inc.
            new("Remote Purchasing?", "Local gangster\r\nallegedly purchases\r\ndesert property\r\ndespite paying for\r\ncountryside motel\r\nroom."), // Reference to Grand Theft Auto: San Andreas.
            new(null, "\"They just weren't\r\nprotected at all\",\r\nclaims Literature\r\nClub president upon\r\ndeleting critical\r\nCHR files."), // Reference to Doki Doki Literature Club.
            new(null, "Local plumber\r\nboycotts motorcycles\r\nover concerns of\r\nunfair advantage."), // Reference to Mario Kart Wii.
            new(null, "SECRET POWERS OF\r\nHUMBLE BUG NET AND\r\nFISHING ROD\r\nREVEALED!?"), // Reference to The Legend of Zelda: A Link to the Past and Twilight Princess.
            new(null, "Hedgehog shows up\r\nlate after being\r\nlost in maze."), // Reference to Super Smash Brothers Brawl.
            new("Tragic...", "Local Skyloft\r\nresident allegedly\r\nspoke to Goron at\r\nwrong time."), // Reference to The Legend of Zelda: Skyward Sword.
            new("WARNING!", "OK  [CANCEL]  DELETE"), // Reference to Sonic Adventure 2.
            new("FACT", "To make a\r\nphotocopier, simply\r\nphotocopy a mirror."), // Reference to Portal 2.
            new(null, "Local Bandicoot\r\nreportedly still\r\nwaiting on new\r\nLaptop Battery."), // Reference to Crash Bandicoot 2.
            new("Neo Cortex", "The crystals,\r\n{$}.", [PlaceholderTypes.OurName]), // Reference to Crash Bandicoot 2.
            new("LET'S GO GAMBLING!", "[XXX] Aw dang it!\r\n[XXX] Aw dang it!\r\n[XXX] Aw dang it!\r\n[XXX] Aw dang it!\r\n[XXX] Aw dang it!\r\n[XXX] Aw dang it!"), // Reference to that Flipnote Studio animation.
            new("DEAR PESKY PLUMBERS", "The Koopalings and I\r\nhave taken over the\r\nmultiworld.\r\n{$} is now\r\na permanent guest\r\nat one of my 7\r\nBK'd games!", [PlaceholderTypes.RandomNameNotOursOrServer]), // Reference to Hotel Mario.
            new(null, "{$}\r\ndoesn't need to hear\r\nall this, they're\r\na highly trained\r\nprofessional.", [PlaceholderTypes.RandomNameNoServer]), // Reference to Half-Life.
            new(null, "Monster Truck\r\nsightings increase.\r\n\r\nSouls reported\r\nstolen."), // Reference to Sonic Racing CrossWorlds.
            new(null, "Planet remains\r\nshattered.\r\n\r\nLocal scholar, when\r\nasked for comment,\r\njust talked about\r\nsandwiches..."), // Reference to Sonic Unleashed.
            new(null, "{$}\r\nreportedly didn't\r\nget the Wordle.", [PlaceholderTypes.RandomNameNotOursOrServer]), // Reference to Wordle.
            new(null, "{$}\r\nhit the ground\r\ntoo hard.", [PlaceholderTypes.RandomNameNotOursOrServer]), // Reference to Minecraft.
            new(null, "Reading this\r\nSpam Trap\r\ncrashes Paper Mario."), // Reference to old "Doing [x] crashes Paper Mario" videos.
            new("Intelligence Core", "Peanut butter\r\nbutterflies. It is\r\nshaped like a fish.\r\n\r\n\r\nCUP"), // Reference to the Portal Google Translated mod.
            new("Pot: $2,400", "{$}\r\nhas...\r\n{$}\r\n\r\n{$} wins\r\nthe hand.", [PlaceholderTypes.OurName, PlaceholderTypes.RandomString, PlaceholderTypes.RandomNameNotOurs], ["Ace High.", "A Pair of Twos.", "Two Pair.", "Three of a Kind.", "Four of a Kind.", "A Flush!", "A Straight!", "A Full House!", "A Straight Flush!", "A ROYAL FLUSH!"]), // Reference to Poker Night at the Inventory.
            new(null, "Mudokon workers\r\ncontinue to go\r\nmissing.\r\n\r\nReports of Bird\r\nPortals dismissed as\r\n\"crap\"."), // Reference to Oddworld.
            new(null, "Grandma allegedly\r\ndiscovers quantum\r\ntechnology while\r\nbaking cookies."), // Reference to Cookie Clicker.
            new(null, "{$}\r\nmakes yet another\r\ncum joke. Gets\r\nSuper Quiplash.", [PlaceholderTypes.RandomNameNotOursOrServer]), // Reference to Quiplash.
            new(null, "Uh-oh! How unfortunate!\r\nUh-oh! How unfortunate!\r\nI'm gonna do a sneaky\r\nthing, and add a new\r\ntext box to your\r\nscreen!"), // Reference to Ultimate Custom Night.
            new("Jokes on them.", "{$}\r\nforgot about\r\nThe Psychic.\r\n\r\nOnly played\r\nHigh Card.", [PlaceholderTypes.RandomNameNoServer]), // Reference to Balatro.
            new(null, "Real Hardware sales\r\ndrop following\r\nretirement of\r\nHedgehog-based\r\nquality assurance."), // Reference to redhotsonic.
            new(null, "{$}\r\nwas not The\r\nImposter.", [PlaceholderTypes.RandomNameNoServer]), // Reference to Among Us.
            new(null, "Worm drowns\r\nin rope related\r\naccident."), // Reference to Worms.
            new(null, "Local manager\r\nfinds secret to\r\nsafe consumption of\r\nfull bottle of Pain\r\nPills.\r\n\r\nClick for more info."), // Reference to Left 4 Dead.
            new(null, "Hedgehog stands\r\nupside down on\r\nshuttle loop.\r\n\r\nSpectators baffled."), // Reference to Sonic '06.
            new(null, "Reports of mining\r\nlaser sabotage\r\nrefuted, declared\r\ntragic accident."), // Reference to Freedom Planet 2 itself.
            new(null, "Local officer\r\nreportedly almost\r\na sandwich."), // Reference to Resident Evil.
            new(null, "There was a\r\nhole here.\r\n\r\nIt's gone now."), // Reference to Silent Hill 2.
            new("BECOME AS GODS", "THIS CANNOT CONTINUE"), // Reference to NieR Automata.
            new(null, "Nanomachine market\r\nsurges following\r\ninternet memes."), // Reference to Metal Gear (specifically Armstrong's line in Rising).
            new(null, "Time Travel achieved\r\nwithout set of\r\nstones.\r\n\r\nHedgehog left\r\nconfused."), // Reference to Sonic Generations (specifically a line in the 2024 rewrite).
            new(null, "Finny Fun turns\r\nout to be not\r\nso fun after all."), // Reference to Kingdom Hearts 2.
            new(null, "Glory to Arstotzka"), // Reference to Papers, Please.
            new("valve plz", "ricochet 2\r\nwhen?"), // Reference to Ricochet.
            new(null, "{$}\r\nforgot to install\r\nCounter-Strike:\r\nSource.", [PlaceholderTypes.RandomNameNoServer]), // Reference to how Counter-Strike: Source used to be so important to Garry's Mod.
            new(null, "YOU'RE WINNER !"), // Reference to Big Rigs.
            new(null, "An archipelago.gg\r\naccount is required\r\nto play this title."), // Reference to the bethesda.net requirement in the 25th anniversary Doom rereleases.
            new(null, "Hear the words of\r\nO-Lir, last Sentinel\r\nof the Fortress\r\nTemple. May they\r\nserve you well."), // Reference to Metroid Prime 2: Echoes.
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
        private List<string> placeholderStrings = null;
        private int placeholderIndex = 0;

        // Debug specific message for testing the placeholders.
        public bool debugTrap;
        public int debugIndex;
        private readonly SpamTrapMessage DebugMessage = new("***DEBUG for {$}***",
                                                            "Random Name: {$}\r\nNot Server: {$}\r\nNot Us: {$}\r\nNeither: {$}\r\nUs: {$}\r\nCustom: {$}",
                                                            [PlaceholderTypes.OurName, PlaceholderTypes.RandomName, PlaceholderTypes.RandomNameNoServer, PlaceholderTypes.RandomNameNotOurs, PlaceholderTypes.RandomNameNotOursOrServer, PlaceholderTypes.OurName, PlaceholderTypes.RandomString],
                                                            ["String 1", "String 2", "String 3", "Potato"]);

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

            // Handle overriding the text if this is a Debug Spam Trap.
            if (debugTrap)
            {
                if (debugIndex == messages.Length)
                {
                    header = DebugMessage.Header;
                    message = DebugMessage.Message;
                    placeholders = DebugMessage.Placeholders;
                    placeholderStrings = DebugMessage.PlaceholderStrings;
                }
                else
                {
                    header = messages[debugIndex].Header;
                    message = messages[debugIndex].Message;
                    placeholders = messages[debugIndex].Placeholders;
                    placeholderStrings = messages[debugIndex].PlaceholderStrings;
                }
            }

            // Get the names of the players in this multiworld.
            List<string> playerNames = [];
            foreach (PlayerInfo? player in Plugin.session.Players.AllPlayers)
                playerNames.Add(player.Name);

            // Swap out any placeholders the header and message may have.
            if (header != null) header = ReplacePlaceholders(header, placeholders, placeholderStrings);
            message = ReplacePlaceholders(message, placeholders, placeholderStrings);

            // Select a colour for the background if this isn't a Debug Spam Trap.
            if (!debugTrap)
                gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colours[Plugin.rng.Next(colours.Length)];

            // Handle setting the header and body positions. We redo stuff here for the sake of the Debug Spam Trap.
            if (header != null)
            {
                gameObject.transform.GetChild(1).gameObject.SetActive(true);
                gameObject.transform.GetChild(1).GetComponent<TextMesh>().text = header;
                gameObject.transform.GetChild(2).transform.localPosition = new(128.5f, -88f, 0f);
            }
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
            // Check if this is a Debug Spam Trap.
            if (debugTrap)
            {
                // Cycle through messages when the Numpad is used, printing the current index (the last one is the default Debug message) and rerunning the start function to update everything.
                if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    if (debugIndex == 0)
                        debugIndex = messages.Length;
                    else
                        debugIndex--;

                    Plugin.consoleLog.LogDebug($"Debug Spam Trap Message Index: {debugIndex}");

                    Start();
                }
                if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    if (debugIndex == messages.Length)
                        debugIndex = 0;
                    else
                        debugIndex++;

                    Plugin.consoleLog.LogDebug($"Debug Spam Trap Message Index: {debugIndex}");

                    Start();
                }

                // Don't do the timer stuff if we're a Debug Spam Trap.
                return;
            }

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

        private string ReplacePlaceholders(string text, List<PlaceholderTypes> placeholders, List<string> placeholderStrings = null)
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


                        case PlaceholderTypes.RandomString:
                            split[splitIndex] = placeholderStrings[Plugin.rng.Next(placeholderStrings.Count)];
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
