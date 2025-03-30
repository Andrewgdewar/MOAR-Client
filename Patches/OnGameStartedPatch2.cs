using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using MOAR.Components;
using SPT.Reflection.Patching;

namespace MOAR.Patches
{
    /// <summary>
    /// Ensures that BotZoneRenderer is added to the BotZone when its Awake method is called.
    /// </summary>
    public class OnGameStartedPatch2 : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // Patch the Awake method of BotZone to ensure BotZoneRenderer is added when BotZone is initialized.
            return AccessTools.Method(typeof(BotZone), "Awake");
        }

        [PatchPrefix]
        private static void PatchPrefix(BotZone __instance)
        {
            // Ensure the BotZoneRenderer is only added if it doesn't already exist.
            if (__instance.GetComponent<BotZoneRenderer>() == null)
            {
                __instance.gameObject.AddComponent<BotZoneRenderer>();
            }
        }
    }
}
