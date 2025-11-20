using Archipelago.MultiClient.Net.Helpers;

namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class SpamTrap : FPBaseObject
    {
        // The various spam messages that can be picked for display.
        private static readonly string?[] headers =
        [
            "NOTICE",
            "Beauty Contest",
            "CONGRATULATIONS",
            "Advertisment",
            null,
            null,
            "Adventure Awaits",
            "Buzz Cola",
            "Advertisment",
            null,
            null,
            "A MYURRDERRRR?!",
            null,
            "CONGRATULATIONS",
            "Thief Alert!",
            null,
            "Did You Know?",
            null,
            "Message from Ghandi",
            "Did You Know?",
            "Shrine of Chance",
            "The Ocean",
            null,
            "Need Reception?",
            "AURORA BOREALIS",
            "Dear [PLAYER NAME]",
            "ALERT"
        ];
        private static readonly string[] messages =
        [
            "We've been trying\r\nto reach you\r\nregarding your\r\nbike's extended\r\nwarranty.",
            "You have won second\r\nprize in a\r\nbeauty contest\r\n\r\nCollect $10",
            "YOU'RE THE 50,000TH\r\nVISITOR TO ZAO LAND!\r\n\r\nCLICK HERE TO CLAIM\r\nYOUR PRIZE!",
            "Half price entry to\r\nForest Frontiers.\r\n\r\nAvaliable while\r\nstocks last.", // Reference to OpenRCT2.
            "You won't get tired\r\nof my voice will you?\r\nYou won't get tired\r\nof my voice will you?\r\nYou won't get tired\r\nof my voice will you?\r\nYou won't get tired\r\nof my voice will you?", // Reference to FNaF World.
            "AAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA\r\nAAAAAAAAAAAAAAAAAAAA",
            "Explore the Keep\r\ntoday.\r\n\r\nThis message\r\nsponsored by\r\nThe Keymaster.", // Reference to Keymaster's Keep.
            "For humans!\r\n\r\nIsn't posionous to\r\nanybody!\r\n(that we know of...)", // Reference to The Simpsons: Hit and Run.
            "75% off your next\r\npurchase at JojaMart.", // Reference to Stardew Valley.
            "Receiving this\r\nSpam Trap...\r\n\r\nIt fills you with\r\ndetermination.", // Reference to Undertale
            "You feel an evil\r\npresence watching\r\nyou...", // Reference to Terraria.
            "ON MY OWL EXPRESS?!", // Reference to A Hat in Time.
            "eastmost peninsula\r\nis the secret", // Reference to The Legend of Zelda.
            "You've won your\r\nvery own mansion.\r\n\r\nClick here for\r\ndetails!", // Reference to Luigi's Mansion.
            "The word\r\n[PLAYER NAME],\r\nthey stole it too!", // Reference to Kingdom Hearts 2 apparently, think the original word is Photo?
            "You want fun?\r\n[PLAYER NAME]\r\nwill show you fun...",
            "There's a Mew\r\nunder the truck.", // Reference to Pokémon Red.
            "i showed you my\r\ncacodemon plz\r\nrespond", // Reference to Doom.
            "Our words are backed\r\nby nuclear weapons!", // Reference to Civilization.
            "Metal Harbor is\r\nactually beatable\r\nwithout the\r\nLight Shoes!", // Reference to Sonic Adventure 2: Battle.
            "You offer to the\r\nshrine, but gain\r\nnothing.", // Reference to Risk of Rain 2.
            "Now with 75%\r\nmore Leviathan!", // Reference to Subnautica
            "This advert\r\ndedicated to those\r\nwho perished on\r\nthe climb...", // Reference to Celeste
            "Climb to the top\r\nof Hawk Peak!", // Reference to A Short Hike.
            "At this time of year?\r\nAt this time of day?\r\nIn this part of the\r\nmutliworld?\r\n\r\nLocalised entirely\r\nwithin your slot data?!", // You know what this is a reference to...
            "Please come to the\r\ncastle. I've baked\r\na cake for you.\r\nYours truly--\r\nPrincess Toadstool", // Reference to Super Mario 64.
            "[PLAYER NAME]\r\nhas died in an\r\naccident on\r\nSteeplechase 1!", // Also a reference to OpenRCT2.
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

        private new void Start()
        {
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
            var messageIndex = Plugin.rng.Next(messages.Length);

            // Select a colour for the background.
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colours[Plugin.rng.Next(colours.Length)];

            // If the target message has a header, then just update its text.
            // If not, then hide the header element and shift the body element up by 8 pixels.
            if (headers[messageIndex] != null)
                gameObject.transform.GetChild(1).GetComponent<TextMesh>().text = headers[messageIndex];
            else
            {
                gameObject.transform.GetChild(1).gameObject.SetActive(false);
                gameObject.transform.GetChild(2).transform.localPosition = new(128.5f, -80f, 0f);
            }

            // Update the body element's text.
            gameObject.transform.GetChild(2).GetComponent<TextMesh>().text = messages[messageIndex];

            // Swap out [PLAYER NAME] in the header with our name.
            gameObject.transform.GetChild(1).GetComponent<TextMesh>().text = gameObject.transform.GetChild(1).GetComponent<TextMesh>().text.Replace("[PLAYER NAME]", Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot));

            // Swap out [PLAYER NAME] in the body with a random name from the multiworld.
            List<string> playerNames = [];
            foreach (PlayerInfo? player in Plugin.session.Players.AllPlayers)
                playerNames.Add(player.Name);
            gameObject.transform.GetChild(2).GetComponent<TextMesh>().text = gameObject.transform.GetChild(2).GetComponent<TextMesh>().text.Replace("[PLAYER NAME]", playerNames[Plugin.rng.Next(playerNames.Count)]);
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
    }
}
