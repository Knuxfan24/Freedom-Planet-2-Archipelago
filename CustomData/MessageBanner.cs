namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class MessageBanner : FPBaseObject
    {
        /// <summary>
        /// The state that the banner is currently in.
        /// </summary>
        public FPObjectState state;

        /// <summary>
        /// Whether the banner's object has actually been set up.
        /// </summary>
        private bool isValidatedInObjectList;

        /// <summary>
        /// The text that will get typed into the text mesh.
        /// </summary>
        public string text = "Placeholder";

        /// <summary>
        /// The text mesh part of the banner.
        /// </summary>
        private TextMesh textMesh;

        /// <summary>
        /// The character that is currently being typed.
        /// </summary>
        private int characterIndex;

        /// <summary>
        /// A timer used to pause between typing and erasing.
        /// </summary>
        private float genericTimer;

        private void Start()
        {
            // Set the banner to its idle state.
            state = State_Idle;

            // Start the FPBaseObject setup.
            base.Start();

            // Get the text mesh and blank out its string.
            textMesh = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMesh>();
            textMesh.text = string.Empty;
        }

        private void Update()
        {
            // Validate this object in the stage list if it hasn't already been.
            if (!isValidatedInObjectList && FPStage.objectsRegistered)
                isValidatedInObjectList = FPStage.ValidateStageListPos(this);

            // Invoke our state if it isn't null and the banner object is registered.
            if (FPStage.objectsRegistered && state != null)
                state();

            // Change the location of the label depending on the scene.
            switch (SceneManager.GetActiveScene().name)
            {
                case "ClassicMenu": position.y = -13; break; // Top of the screen so it doesn't clip the shop menu.
                case "ArenaMenu": position.y = -348; break; // Bottom of the screen so it doesn't clip the challenge description.

                default:
                    position.y = -60; // Best position to fit in with the gameplay HUD.

                    // If the Time Limit Brave Stone is active and hasn't elapsed, then shift the label down to -86 so it doesn't overlap the timer.
                    if (FPPlayerPatcher.player != null)
                        if (FPPlayerPatcher.player.powerups.Contains(FPPowerup.TIME_LIMIT))
                            if (FPSaveManager.GetStageParTime(FPStage.currentStage.stageID) - (FPStage.currentStage.minutes * 6000 + FPStage.currentStage.seconds * 100 + FPStage.currentStage.milliSeconds) > 0)
                                position.y = -86;
                    break; 
            }

        }

        public void State_Idle()
        {
            // Reset the generic timer and character index.
            genericTimer = 0;
            characterIndex = 0;

            // Force the scale to 0.
            gameObject.transform.localScale = Vector3.zero;

            // Force empty the text.
            text = string.Empty;
        }

        public void State_Expand()
        {
            // Expand the scale's x axis based on the delta time if it hasn't reached 1 yet (clamping the value to prevent an ugly snap back).
            if (gameObject.transform.localScale.x < 1f)
                gameObject.transform.localScale = new(Mathf.Clamp(gameObject.transform.localScale.x + (Time.deltaTime * 4), 0, 1), 1f, 1f);

            // Swap the state and force the scale to an even 1.
            else
            {
                state = State_Typing;
                gameObject.transform.localScale = Vector3.one;
            }
        }

        private void State_Typing()
        {
            // Add the current character from the text value to the text mesh.
            // TODO: This is affected by framerate, but I'm not sure I care.
            textMesh.text += text[characterIndex];

            // If we still have characters left, then increment the index.
            // If not, then swap to the waiting state.
            if (characterIndex < text.Length - 1)
                characterIndex++;
            else
                state = State_Waiting;
        }

        private void State_Waiting()
        {
            // Increment our generic timer by the delta time.
            genericTimer += Time.deltaTime;

            // Swap to the erasing state if the timer reaches 2.
            if (genericTimer >= 2f)
                state = State_Erasing;
        }

        private void State_Erasing()
        {
            // If the text mesh has characters in it, then remove the most recent.
            // Else, swap to the shrink state.
            if (textMesh.text.Length != 0)
                textMesh.text = textMesh.text.Remove(textMesh.text.Length - 1);
            else
                state = State_Shrink;
        }

        private void State_Shrink()
        {
            // Reset the generic timer and character index.
            // We duplicate this here to fix an edge case where the idle state seems to be bypassed, leading to an out of range exception on the character index.
            genericTimer = 0;
            characterIndex = 0;

            // Reduce the scale's x axis based on the delta time if it hasn't reached 1 yet (clamping the value to prevent an ugly snap back).
            // Else, return to the idle state.
            if (gameObject.transform.localScale.x > 0f)
                gameObject.transform.localScale = new(Mathf.Clamp(gameObject.transform.localScale.x - (Time.deltaTime * 4), 0, 1), 1f, 1f);
            else
                state = State_Idle;
        }
    }
}
