using Freedom_Planet_2_Archipelago.Patchers;

namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class ChestTracer : FPBaseObject
    {
        // FPBaseObject stuff.
        private static int classID = -1;
        private FPObjectState state;
        private bool isValidatedInObjectList;

        // The position this chest tracer should point to.
        public Vector2 targetPosition;

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
            // If the player doesn't exist for whatever reason, then don't do anything.
            if (FPPlayerPatcher.player == null)
                return;

            // Check if the player is close enough to the chest for the arrow's snap.
            if (FPPlayerPatcher.player.position.x <= targetPosition.x + 160 && FPPlayerPatcher.player.position.x >= targetPosition.x - 160 && FPPlayerPatcher.player.position.y <= targetPosition.y + 160 && FPPlayerPatcher.player.position.y >= targetPosition.y - 160)
            {
                // Set this tracer's position to the target's, with the y value increased by 128 to place it above the chest.
                position = new(targetPosition.x, targetPosition.y + 128);

                // Rotate this tracer so the arrow points down.
                transform.rotation = Quaternion.Euler(0, 0, -90);

                // Stop here.
                return;
            }

            // Set this tracer's position to the player's.
            position = FPPlayerPatcher.player.position;

            // Caculate and set the rotation of this tracer.
            transform.right = targetPosition - position;
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        }
    }
}
