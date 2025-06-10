using System.Reflection.Emit;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPSaveManagerPatcher
    {
        // Set the save path to Archipelago Saves.
        public static string GetSavesPath() => $@"{Paths.GameRootPath}\Archipelago Saves";

        // Force the JSONs to save with indenting.
        static string FancifyJson(UnityEngine.Object obj) => JsonUtility.ToJson(obj, true);

        /// <summary>
        /// Overwrite the slot number when the game tries to check if a save exists.
        /// </summary>
        /// <param name="f">The slot to use instead.</param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSaveManager), "FileExists")]
        static void HijackSlotNumberForExistCheck(ref int f) => f = Plugin.save.SaveSlot;

        /// <summary>
        /// Overwrite the slot number when the game tries to save.
        /// </summary>
        /// <param name="f">The slot to use instead.</param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSaveManager), "SaveToFile")]
        static void HijackSlotNumberForSaving(ref int f) => f = Plugin.save.SaveSlot;

        /// <summary>
        /// Overwrite the slot number when the game tries to load.
        /// </summary>
        /// <param name="f">The slot to use instead.</param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile")]
        static void HijackSlotNumberForLoading(ref int f) => f = Plugin.save.SaveSlot;

        /// <summary>
        /// Replaces the Save Manager's Total Star Cards calculation with the amount from the multiworld.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), nameof(FPSaveManager.TotalStarCards))]
        static void HijackStarCardCount(ref int __result) => __result = Plugin.save.StarCardCount;

        /// <summary>
        /// Replaces the Save Manager's Total Logs calculation with the time capsule amount from the multiworld.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), nameof(FPSaveManager.TotalLogs))]
        static void HijackTimeCapsuleCount(ref int __result) => __result = Plugin.save.TimeCapsuleCount;

        /// <summary>
        /// Stops the Save Manager from unequipping items that haven't been acquired in the shop.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSaveManager), nameof(FPSaveManager.SanitizeItemSets))]
        static bool StopItemSetSanitisation() => false;

        /// <summary>
        /// Forces the game to format the JSON files it saves, rather than shoving them all on a single line.
        /// Taken from FP2Lib's own Save Manager patcher.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "SaveToFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchJsonStyle(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new(instructions);

            for (var i = 1; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Call && (codes[i - 1].opcode == OpCodes.Ldloc_0 || codes[i - 1].opcode == OpCodes.Ldloc_1) && codes[i - 2].opcode == OpCodes.Stfld)
                    codes[i] = Transpilers.EmitDelegate(FancifyJson);

            return codes;
        }

        /// <summary>
        /// Forces the game to use our save path.
        /// Taken from FP2Lib's own Save Manager patcher.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "SaveToFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveWrite(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new(instructions);

            for (var i = 1; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);

            return codes;
        }

        /// <summary>
        /// Forces the game to load from our save path.
        /// Taken from FP2Lib's own Save Manager patcher.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveLoad(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new(instructions);

            for (var i = 1; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);

            return codes;
        }

        /// <summary>
        /// Forces the game to delete from our save path. In theory this should never happen, but just in case.
        /// Taken from FP2Lib's own Save Manager patcher.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "DeleteFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveDelete(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new(instructions);

            for (var i = 1; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);

            return codes;
        }

        /// <summary>
        /// Forces the game to get info on files from our save path.
        /// Taken from FP2Lib's own Save Manager patcher.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuFile), "GetFileInfo", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchFileInfo(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new(instructions);

            for (var i = 1; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);

            return codes;
        }
    }
}
