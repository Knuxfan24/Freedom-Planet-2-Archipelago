// Sprite sourced from: https://commons.wikimedia.org/wiki/File:DVD_logo.svg
namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class DVDScreensaver : MonoBehaviour
    {
        private bool right;
        private bool up;
        private float speed;
        private SpriteRenderer renderer;
        private float genericTimer;

        void Start()
        {
            // Get the sprite renderer for the object.
            renderer = GetComponent<SpriteRenderer>();

            // Determine the direction to start moving in and the speed.
            if (Plugin.rng.Next(0, 2) == 0) right = true;
            if (Plugin.rng.Next(0, 2) == 0) up = true;
            speed = Plugin.rng.Next(2, 5);

            // Set the timer to 60 seconds.
            genericTimer = 60;

            // Pick a colour for the sprite.
            SetNewColour();
        }

        // Update is called once per frame
        void Update()
        {
            // Move the DVD Screensaver on the X axis.
            if (right)
                transform.position = new Vector3(transform.position.x + (speed * FPStage.deltaTime), transform.position.y);
            else
                transform.position = new Vector3(transform.position.x - (speed * FPStage.deltaTime), transform.position.y);

            // Move the DVD Screensaver on the Y axis.
            if (up)
                transform.position = new Vector3(transform.position.x, transform.position.y + (speed * FPStage.deltaTime));
            else
                transform.position = new Vector3(transform.position.x, transform.position.y - (speed * FPStage.deltaTime));

            // Bounce and pick a new colour when at the edges.
            if (transform.position.x >= 576) { right = false; SetNewColour(); }
            if (transform.position.x <= 64) { right = true; SetNewColour(); }
            if (transform.position.y >= -28) { up = false; SetNewColour(); }
            if (transform.position.y <= -332) { up = true; SetNewColour(); }

            // Run the timer down and delete the object when it hits 0.
            genericTimer -= Time.deltaTime;
            if (genericTimer <= 0)
                GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// Generates a colour for the sprite.
        /// </summary>
        void SetNewColour() => renderer.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
    }
}
