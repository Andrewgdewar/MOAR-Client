using System.Reflection;
using EFT.Game.Spawning;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace MOAR.Patches
{
    /// <summary>
    /// Logs all BotZones on map load. Debug/dev usage.
    /// </summary>
    public class BotZoneDumper : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(typeof(LocationScene), "Awake");

        [PatchPostfix]
        public static void Postfix(LocationScene __instance)
        {
            if (__instance?.BotZones == null) return;

            foreach (var botZone in __instance.BotZones)
            {
                Plugin.LogSource.LogInfo($"BotZone name: {botZone.NameZone}, ID: {botZone.Id}");
            }
        }
    }
}
