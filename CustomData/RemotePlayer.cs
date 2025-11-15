namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class RemotePlayer : FPBaseObject
    {
        // FPBaseObject stuff.
        private FPObjectState state;

        /// <summary>
        /// The animation this remote player should be using.
        /// </summary>
        public string currentAni = "Idle";

        /// <summary>
        /// Whether or not the remote player has had its sprites and animator setup.
        /// </summary>
        public bool hasVisualCharacter = false;

        private void Update()
        {
            if (hasVisualCharacter)
                GetComponent<Animator>().Play(currentAni);

            // Flip the sprite renderer depending on the facing direction.
            if (direction == FPDirection.FACING_LEFT) GetComponent<SpriteRenderer>().flipX = true;
            if (direction == FPDirection.FACING_RIGHT) GetComponent<SpriteRenderer>().flipX = false;
        }
    }
}
