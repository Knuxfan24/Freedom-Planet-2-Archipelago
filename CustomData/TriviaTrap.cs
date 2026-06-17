// TODO: The prefab for this has a timer in it, but considering the movement keys are the answer keys, I'm not sure the extra stress of the timer is needed?
// TODO: These don't work in Bakunawa Chase.
// TODO: Wrong answers don't successfully kill the BFF2000.
namespace Freedom_Planet_2_Archipelago.CustomData
{
    public class TriviaTrap : FPBaseObject
    {
        // Small internal class to store questions.
        public class DKCQuestion
        {
            public string Question = "";
            public string Difficulty = "EASY";
            public string Author;
            public List<string> Answers = [];
        }

        // FPBaseObject stuff.
        private static int classID = -1;
        private FPObjectState state;
        private bool isValidatedInObjectList;

        // Question UI values.
        private TextMesh questionUI;
        private string question = "Question";
        private int questionCharacterIndex;

        // References to the game objects for the answers.
        private GameObject answer1UI;
        private GameObject answer2UI;
        private GameObject answer3UI;
        private GameObject answer4UI;

        // The actual answers.
        private string[] answers = ["Correct Answer", "Wrong Answer 1", "Wrong Answer 2"/*, "Wrong Answer 3"*/];

        // The index of the correct answer.
        private int answerIndex = 0;

        // The timer used to show the answers.
        private float genericTimer = 0.25f;

        // A reference to the stage HUD.
        private FPHudMaster HUD;

        // The traps that can replace this if we don't have any trivia database files.
        private readonly string[] replacementTraps =
        [
            "Swap Trap",
            "Mirror Trap",
            "Pie Trap",
            "Spring Trap",
            "PowerPoint Trap",
            "Zoom Trap",
            "Aaa Trap",
            "Spike Ball Trap",
            "Pixellation Trap",
            "Rail Trap",
            "Spam Trap",
            "Syntax Jumpscare Trap"
        ];

        private new void Start()
        {
            // If we don't have any trivia, then swap this trap out for another one.
            if (Plugin.TriviaGames.Count == 0)
            {
                // Create and handle a new trap.
                ArchipelagoItem replacementTrap = new()
                {
                    ItemName = replacementTraps[Plugin.rng.Next(replacementTraps.Length)],
                    Source = "Trivia Trap"
                };
                Helpers.HandleItem(new(replacementTrap, 1));

                // Tell the player what trap activated and why.
                Plugin.sentMessageQueue.Add($"No Trivia Found. Have a {replacementTrap.ItemName} instead!");

                // Disable the trivia trap flag.
                Plugin.TriviaTrap = false;

                // Destroy this trivia trap.
                GameObject.Destroy(this.gameObject);

                // Don't do anything else.
                return;
            }

            // Get the HUD and hide it.
            HUD = UnityEngine.Object.FindObjectOfType<FPHudMaster>();
            HUD?.state = 2;

            // Set up a string to store the difficulty of the selected question for checking against the config option.
            string questionDifficulty = string.Empty;

            void SelectQuestion()
            {
                // TODO: Allow some games to be blacklisted maybe?
                int gameIndex = Plugin.rng.Next(Plugin.TriviaGames.Count);
                List<DKCQuestion> questions = Plugin.TriviaGames.ElementAt(gameIndex).Value;

                // Pick a random question to use.
                int questionIndex = Plugin.rng.Next(questions.Count);

                // Set the trivia trap's question and answers to this question's values.
                question = questions[questionIndex].Question;
                answers = [.. questions[questionIndex].Answers];
                questionDifficulty = questions[questionIndex].Difficulty;
            }

            // Select a question, replacing it if the difficulty is invalid.
            SelectQuestion();
            while (Plugin.configTriviaDifficulty.Value == 1 && questionDifficulty == "HARD")
                SelectQuestion();
            while (Plugin.configTriviaDifficulty.Value == 0 && (questionDifficulty == "HARD" || questionDifficulty == "MEDIUM"))
                SelectQuestion();

            // Word wrap the question string.
            question = FPStage.WrapText(question, 50);

            // Swap to the typing state.
            state = State_TypeQuestion;

            // Start the FPBaseObject setup.
            base.Start();
            classID = FPStage.RegisterObjectType(this, GetType(), 0);
            objectID = classID;

            // Force this script to always be active.
            activationMode = FPActivationMode.ALWAYS_ACTIVE;

            // Play the menu select sound.
            FPAudio.PlaySfx(FPAudio.SFX_SELECT);

            // Get the Question Text Mesh and wipe it.
            questionUI = gameObject.transform.GetChild(0).GetComponent<TextMesh>();
            questionUI.text = string.Empty;

            // Get the correct answer.
            string correctQuestion = answers[0];

            // Shuffle the answers.
            answers.Shuffle();

            // Find the index of the correct answer.
            answerIndex = Array.FindIndex(answers, x => x.Equals(correctQuestion));
            //Plugin.consoleLog.LogDebug($"Answer index is {answerIndex}.");

            // Get the answer game objects.
            answer1UI = gameObject.transform.GetChild(1).gameObject;
            answer2UI = gameObject.transform.GetChild(2).gameObject;
            answer3UI = gameObject.transform.GetChild(3).gameObject;
            answer4UI = gameObject.transform.GetChild(4).gameObject;

            // Set the answer text.
            answer4UI.transform.GetChild(1).GetComponent<TextMesh>().text = answers[0];
            answer3UI.transform.GetChild(1).GetComponent<TextMesh>().text = answers[1];
            answer2UI.transform.GetChild(1).GetComponent<TextMesh>().text = answers[2];
            // Not all questions have a fourth answer, so check for that first.
            if (answers.Length > 3)
                answer1UI.transform.GetChild(1).GetComponent<TextMesh>().text = answers[3];
        }

        private void Update()
        {
            // Validate this object in the stage list if it hasn't already been.
            if (!isValidatedInObjectList && FPStage.objectsRegistered)
                isValidatedInObjectList = FPStage.ValidateStageListPos(this);

            // Invoke the current state if it isn't null.
            state?.Invoke();
        }

        private void State_TypeQuestion()
        {
            // Add the current character from the text value to the text mesh.
            // TODO: This is affected by framerate, but I'm not sure I care.
            questionUI.text += question[questionCharacterIndex];

            // If we still have characters left, then increment the index.
            // If not, then start revealing the answers.
            if (questionCharacterIndex < question.Length - 1)
                questionCharacterIndex++;
            else
                state = State_RevealAnswer4;
        }

        private void State_Waiting()
        {
            // Decrement our timer by the game's delta time.
            genericTimer -= Time.deltaTime;

            // Check if we've reached 0 on our timer.
            if (genericTimer <= 0)
            {
                genericTimer = 0.25f;

                if (!answer3UI.activeSelf) { state = State_RevealAnswer3; return; }
                if (!answer2UI.activeSelf) { state = State_RevealAnswer2; return; }
                if (!answer1UI.activeSelf) { state = State_RevealAnswer1; return; }
            }
        }

        private void State_WaitForAnswer()
        {
            if (FPPlayerPatcher.player.input.upPress)
            {
                if (answers.Length > 3)
                {
                    if (answerIndex == 3) HandleRightAnswer();
                    else HandleWrongAnswer();
                }
            }

            if (FPPlayerPatcher.player.input.downPress)
            {
                if (answerIndex == 2) HandleRightAnswer();
                else HandleWrongAnswer();
            }

            if (FPPlayerPatcher.player.input.leftPress)
            {
                // Flip the answer index for this answer if a Mirror Trap is active.
                int directionIndex = 1;
                if (Plugin.MirrorTrapTimer > 0)
                    directionIndex = 0;

                if (answerIndex == directionIndex) HandleRightAnswer();
                else HandleWrongAnswer();
            }

            if (FPPlayerPatcher.player.input.rightPress)
            {
                // Flip the answer index for this answer if a Mirror Trap is active.
                int directionIndex = 0;
                if (Plugin.MirrorTrapTimer > 0)
                    directionIndex = 1;

                if (answerIndex == directionIndex) HandleRightAnswer();
                else HandleWrongAnswer();
            }
        }

        private void HandleWrongAnswer()
        {
            // Set the flag on the player to tell it we're dying because of the trivia trap.
            FPPlayerPatcher.dyingFromTriviaTrap = true;

            // Play the "Bzzt! Wrong!" voice line.
            FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("FP2_Zao_S10_06_aptrim"));

            // Force run the player's crush action to kill them.
            FPPlayerPatcher.player.Action_Crush();

            // Remove the trivia trap flag.
            Plugin.TriviaTrap = false;

            // Destroy this trivia trap.
            GameObject.Destroy(this.gameObject);
        }

        private void HandleRightAnswer()
        {
            // Spawn 7 life petals, copied from the original code for breaking a life petal box.
            for (float num = 22.5f; num <= 157.5f; num += 135f / (float)(7 - 1))
            {
                ItemPetal itemPetal = (ItemPetal)FPStage.CreateStageObject(ItemPetal.classID, FPPlayerPatcher.player.position.x, FPPlayerPatcher.player.position.y);
                itemPetal.gameObject.layer = FPPlayerPatcher.player.gameObject.layer;
                itemPetal.state = itemPetal.State_Released;
                itemPetal.velocity.x = Mathf.Cos((FPPlayerPatcher.player.transform.eulerAngles.z + num) * ((float)Math.PI / 180f)) * 4f;
                itemPetal.velocity.y = Mathf.Sin((FPPlayerPatcher.player.transform.eulerAngles.z + num) * ((float)Math.PI / 180f)) * 4f;
            }

            // Play the +5 sound.
            FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("GachaponWin"));

            // Remove the trivia trap flag.
            Plugin.TriviaTrap = false;

            // Bring back the HUD.
            HUD?.state = 1;

            // Destroy this trivia trap.
            GameObject.Destroy(this.gameObject);
        }

        // Various copied methods to reveal answers. Might try and crush this down to one.
        // Answer 2 is different due to it checking if Answer 1 exists or not.
        private void State_RevealAnswer4()
        {
            answer4UI.SetActive(true);
            FPAudio.PlaySfx(FPAudio.SFX_MOVE);
            state = State_Waiting;
        }
        private void State_RevealAnswer3()
        {
            answer3UI.SetActive(true);
            FPAudio.PlaySfx(FPAudio.SFX_MOVE);
            state = State_Waiting;
        }
        private void State_RevealAnswer2()
        {
            answer2UI.SetActive(true);
            FPAudio.PlaySfx(FPAudio.SFX_MOVE);

            if (answers.Length > 3) state = State_Waiting;
            else state = State_WaitForAnswer;
        }
        private void State_RevealAnswer1()
        {
            answer1UI.SetActive(true);
            FPAudio.PlaySfx(FPAudio.SFX_MOVE);
            state = State_WaitForAnswer;
        }
    }
}
