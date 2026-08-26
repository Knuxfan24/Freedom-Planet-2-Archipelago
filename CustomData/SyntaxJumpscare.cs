namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class SyntaxJumpscare : FPBaseObject
    {
        private FPObjectState state;

        // Reference to the prefab's animator.
        private Animator animator;

        // Index so we can play the right sounds, as we can't just add an animation event.
        private int soundIndex = 0;

        private new void Start()
        {
            state = State_Default;

            // Get the animator.
            animator = this.gameObject.GetComponent<Animator>();

            // Start the FPBaseObject setup.
            base.Start();

            // Force this script to always be active.
            activationMode = FPActivationMode.ALWAYS_ACTIVE;

            // Play the appearance sound.
            FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("SyntaxAppear"));
        }

        private void Update()
        {
            // Invoke the current state if it isn't null.
            state?.Invoke();
        }

        private void State_Default()
        {
            // Play a sound depending on the position in the animation and the soundIndex value.
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.173f && soundIndex == 0)
            {
                FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("Sight1"));
                soundIndex = 1;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f && soundIndex == 1)
            {
                FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("SyntaxAppear"));
                soundIndex = 2;
            }

            // If we've reached the end of the animation, then delete this object.
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                Destroy(this.gameObject);
        }
    }
}
