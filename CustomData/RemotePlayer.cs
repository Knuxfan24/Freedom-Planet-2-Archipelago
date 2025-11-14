namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class RemotePlayer : FPBaseObject
    {
        // FPBaseObject stuff.
        private static int classID = -1;
        private FPObjectState state;
        private bool isValidatedInObjectList;

        /// <summary>
        /// The animation this remote player should be using.
        /// </summary>
        public string currentAni = "Idle";

        /// <summary>
        /// Whether or not the remote player has had its sprites and animator setup.
        /// </summary>
        public bool hasVisualCharacter = false;

        private new void Start()
        {
            state = State_Default;

            // Start the FPBaseObject setup.
            base.Start();
            classID = FPStage.RegisterObjectType(this, GetType(), 0);
            objectID = classID;

            // Force this script to always be active.
            activationMode = FPActivationMode.ALWAYS_ACTIVE;
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
            // Play the animation that is currently set, assuming the remote player is a vanilla character.
            // TODO: This causes problems for some animations (such as ones that are supposed to transition into another one without calling SetPlayerAnimation.
            // TODO: Carol looks a bit odd if the remote player is on her bike, as we only ever pull her on foot animator.
            if (hasVisualCharacter)
                GetComponent<Animator>().Play(currentAni);

            // Flip the sprite renderer depending on the facing direction.
            if (direction == FPDirection.FACING_LEFT) GetComponent<SpriteRenderer>().flipX = true; 
            if (direction == FPDirection.FACING_RIGHT) GetComponent<SpriteRenderer>().flipX = false; 
        }
    }
}
