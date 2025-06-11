using System.Linq;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuCreditsPatcher
    {
        /// <summary>
        /// Moves some things around at the end of the credits.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCredits), "Start")]
        static void MoveThings()
        {
            // Grab all the objects in the credits scene before the game disables some.
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>().Where(x => x.activeInHierarchy).ToArray();

            // Disable all the Time Capsules, as they don't mean anything for the credits in AP.
            foreach (var obj in allObjects.Where(x => x.name.StartsWith("CapsuleSlot")))
                obj.gameObject.SetActive(false);

            // Move the various Thanks for Playing elements to be better centered without the Time Capsules taking up space.
            MoveObject(allObjects.FirstOrDefault(x => x.name == "CharacterArt"), new(0, 24, 0));
            MoveObject(allObjects.FirstOrDefault(x => x.name == "ThankYou"), new(0, -48, 0));
            MoveObject(allObjects.FirstOrDefault(x => x.name == "Label"), new(-216, 128, -3));
            MoveObject(allObjects.FirstOrDefault(x => x.name == "TotalLabel"), new(-312, 160, -4));
            MoveObject(allObjects.FirstOrDefault(x => x.name == "TotalTime"), new(-144, 136, -4));
            MoveObject(allObjects.FirstOrDefault(x => x.name == "Label (1)"), new(216, 128, -3));
            MoveObject(allObjects.FirstOrDefault(x => x.name == "TotalLabel (1)"), new(124, 160, -4));
            MoveObject(allObjects.FirstOrDefault(x => x.name == "TotalTime (1)"), new(288, 136, -4));

            static void MoveObject(GameObject obj, Vector3 position)
            {
                if (obj != null)
                    obj.transform.localPosition = position;
            }
        }

        /// <summary>
        /// Disconnects from the server upon exiting the credits.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCredits), "State_Wait")]
        static void DisconnectOnExit() => Plugin.session.Socket.Disconnect();
    }
}
