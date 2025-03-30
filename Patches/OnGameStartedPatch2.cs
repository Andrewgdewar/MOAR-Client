using System.Reflection;
using EFT;
using HarmonyLib;
using MOAR.Components;
using SPT.Reflection.Patching;

namespace MOAR.Patches
{
    /// <summary>
    /// Ensures that BotZoneRenderer is added to the BotZone when its Awake method is called.
    /// Used to visualize or debug spawn zones dynamically during runtime.
    /// </summary>
    public class OnGameStartedPatch2 : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(typeof(BotZone), "Awake");

        [PatchPostfix]
        private static void Postfix(BotZone __instance)
        {
            if (__instance != null && __instance.GetComponent<BotZoneRenderer>() == null)
            {
                __instance.gameObject.AddComponent<BotZoneRenderer>();
            }
        }
    }
}
