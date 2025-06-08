namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPCameraPatcher
    {
        /// <summary>
        /// Handles flipping the screen (and keeping the UI correct) when a Mirror Trap is active.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPCamera), "LateUpdate")]
        static void MirrorTrapScreenFlip(ref Camera ___uiCam)
        {
            // Only flip if a player is present.
            if (FPPlayerPatcher.player == null)
                return;

            // Check if the Mirror Trap timer is going.
            if (Plugin.MirrorTrapTimer > 0f)
            {
                // Find the Pixel Art Target renderer.
                GameObject pixelArtTarget = GameObject.Find("Pixel Art Target");

                // If we've found it, then check if it has a positive X scale, if so, invert it.
                if (pixelArtTarget != null)
                    if (pixelArtTarget.transform.localScale.x > 0)
                        pixelArtTarget.transform.localScale = new Vector3(pixelArtTarget.transform.localScale.x * -1f, pixelArtTarget.transform.localScale.y, pixelArtTarget.transform.localScale.z);

                // Get the UI camera's current projection matrix.
                Matrix4x4 projectionMatrix = ___uiCam.projectionMatrix;

                // If the first value is positive, then invert it to mirror the UI.
                if (projectionMatrix.m00 > 0)
                    ___uiCam.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
            }

            // Check if the Mirror Trap timer has gone below 0.
            else if (Plugin.MirrorTrapTimer <= 0 && Plugin.MirrorTrapTimer > -1)
            {
                // Set the trap timer to 0 so this check doesn't refire.
                Plugin.MirrorTrapTimer = -1;

                // Find the Pixel Art Target renderer.
                GameObject pixelArtTarget = GameObject.Find("Pixel Art Target");

                // If we've found it, then check if it has a negative X scale, if so, invert it.
                if (pixelArtTarget != null)
                    if (pixelArtTarget.transform.localScale.x < 0)
                        pixelArtTarget.transform.localScale = new Vector3(pixelArtTarget.transform.localScale.x * -1f, pixelArtTarget.transform.localScale.y, pixelArtTarget.transform.localScale.z);

                // Get the UI camera's current projection matrix.
                Matrix4x4 projectionMatrix = ___uiCam.projectionMatrix;

                // If the first value is negative, then invert it to mirror the UI back to normal.
                if (projectionMatrix.m00 < 0)
                    ___uiCam.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
            }
        }
    }
}
