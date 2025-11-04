using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class RemotePlayer : FPBaseObject
    {
        // FPBaseObject stuff.
        private static int classID = -1;
        private FPObjectState state;
        private bool isValidatedInObjectList;

        /// <summary>
        /// The slot of the player tied to this remote player.
        /// </summary>
        public int associatedPlayerSlot = -1;

        /// <summary>
        /// The animation this remote player should be using.
        /// </summary>
        private string currentAni = "Idle";

        /// <summary>
        /// Whether or not the remote player is a vanilla character and thus should use the approriate character sprites.
        /// </summary>
        private bool hasCharacter = false;

        private new void Start()
        {
            state = State_Default;

            // Start the FPBaseObject setup.
            base.Start();
            classID = FPStage.RegisterObjectType(this, GetType(), 0);
            objectID = classID;

            // Force this script to always be active.
            activationMode = FPActivationMode.ALWAYS_ACTIVE;

            // Grab this player's data entry.
            JObject initialEntry = Plugin.session.DataStorage[$"FP2_PlayerSlot{associatedPlayerSlot}"].To<JObject>();

            // Set our position to this player's.
            position.x = (float)initialEntry["PositionX"];
            position.y = (float)initialEntry["PositionY"];

            // Determine the player prefab index.
            int prefabIndex = -1;
            switch ((string)initialEntry["Character"])
            {
                case "Lilac": prefabIndex = 0; break;
                case "Carol": prefabIndex = 3; break;
                case "Milla": prefabIndex = 2; break;
                case "Neera": prefabIndex = 1; break;
            }

            // Check if we found a prefab index.
            if (prefabIndex != -1)
            {
                // Set the hasCharacter flag to true.
                hasCharacter = true;

                // Get the sprite and animator from the player prefab.
                GetComponent<SpriteRenderer>().sprite = Plugin.playerPrefabs[prefabIndex].GetComponent<SpriteRenderer>().sprite;
                gameObject.AddComponent<Animator>().runtimeAnimatorController = Plugin.playerPrefabs[prefabIndex].GetComponent<Animator>().runtimeAnimatorController;
            }

            // Set up the handler to listen to this player's data storage entry.
            Plugin.session.DataStorage[$"FP2_PlayerSlot{associatedPlayerSlot}"].OnValueChanged += RemotePlayer_OnValueChanged;
        }

        /// <summary>
        /// Updates various elements on this object when the data storage entry for this remote player changes.
        /// </summary>
        private void RemotePlayer_OnValueChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
        {
            // Update the position, animation and facing direction for this player.
            position.x = (float)newValue["PositionX"];
            position.y = (float)newValue["PositionY"];
            currentAni = (string)newValue["Animation"];
            direction = (FPDirection)(int)newValue["Facing"];

            // Determine if we need to hide the object for this player or not (due to them not being in this stage).
            if ((string)newValue["Scene"] != SceneManager.GetActiveScene().name) gameObject.SetActive(false);
            else gameObject.SetActive(true);
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
            if (hasCharacter)
                GetComponent<Animator>().Play(currentAni);

            // Flip the sprite renderer depending on the facing direction.
            if (direction == FPDirection.FACING_LEFT) GetComponent<SpriteRenderer>().flipX = true; 
            if (direction == FPDirection.FACING_RIGHT) GetComponent<SpriteRenderer>().flipX = false; 
        }
    }
}
