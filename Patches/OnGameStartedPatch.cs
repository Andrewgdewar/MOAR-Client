using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using MOAR.Components;
using SPT.Reflection.Patching;

namespace MOAR.Patches
{
    /// <summary>
    /// Ensures that BotZoneRenderer is added to the GameWorld on raid start.
    /// </summary>
    public class OnGameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // Simplified target method lookup (method is parameterless)
            return AccessTools.Method(typeof(GameWorld), "OnGameStarted");
        }

        [PatchPrefix]
        private static void PatchPrefix(GameWorld __instance)
        {
            // Ensure the BotZoneRenderer is only added if it doesn't already exist
            if (__instance.GetComponent<BotZoneRenderer>() == null)
            {
                __instance.gameObject.AddComponent<BotZoneRenderer>();
            }
        }
    }
}
