namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class FloodTrapMovement : MonoBehaviour
    {
        // The target this water should move to.
        public float yTarget;

        // Which state this water is in.
        private int state = 0;

        // The timer for when this water should lower.
        private float genericTimer = 0;

        void Update()
        {
            // Only do stuff if the game isn't paused.
            if (FPStage.state == FPStageState.STATE_PAUSED)
                return;

            // Determine what to do based on the state.
            switch (state)
            {
                // Raise the water up to the Y Target, swap changing to state 1 once it reaches that point.
                case 0:
                    if (transform.position.y < yTarget)
                    {
                        transform.position = new(transform.position.x, transform.position.y + 8 * FPStage.deltaTime, transform.position.z);
                        return;
                    }
                    state = 1;
                    break;

                // Increment the timer until it hits 60, then change to state 2.
                case 1:
                    genericTimer += Time.deltaTime;

                    if (genericTimer >= 60)
                        state = 2;
                    break;

                // Lower the water until its off the bottom of the screen then destroy it.
                case 2:
                    yTarget = FPCamera.stageCamera.bottom;

                    if (transform.position.y > yTarget)
                    {
                        transform.position = new(transform.position.x, transform.position.y - 8 * FPStage.deltaTime, transform.position.z);
                        return;
                    }

                    GameObject.Destroy(this.gameObject);
                    break;
            }
        }
    }
}
