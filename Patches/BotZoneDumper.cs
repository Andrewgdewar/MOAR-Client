using System.Reflection;
using EFT.Game.Spawning;
using HarmonyLib;
using SPT.Reflection.Patching;
using MOAR.Helpers; 

namespace MOAR.Patches
{
    /// <summary>
    /// Logs all BotZones on map load. Useful for debugging spawn zones.
    /// </summary>
    public class BotZoneDumper : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(typeof(LocationScene), "Awake");

        [PatchPostfix]
        public static void Postfix(LocationScene __instance)
        {
            if (__instance?.BotZones == null || __instance.BotZones.Length == 0)
                return;

            if (!Settings.debug.Value)
                return;

            foreach (var botZone in __instance.BotZones)
            {
                Plugin.LogSource.LogInfo($"[BotZoneDumper] BotZone name: {botZone.NameZone}, ID: {botZone.Id}");
            }
        }
    }
}
