using System.Reflection;
using EFT;
using HarmonyLib;
using MOAR.Components;
using SPT.Reflection.Patching;
using UnityEngine;

namespace MOAR.Patches
{
    /// <summary>
    /// Ensures that BotZoneRenderer is added to the GameWorld on raid start.
    /// </summary>
    public class OnGameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));

        [PatchPrefix]
        private static void Prefix(GameWorld __instance)
        {
            if (__instance == null)
                return;

            if (!__instance.TryGetComponent<BotZoneRenderer>(out _))
            {
                __instance.gameObject.AddComponent<BotZoneRenderer>();
                Plugin.LogSource.LogDebug("[OnGameStartedPatch] BotZoneRenderer added to GameWorld.");
            }
        }
    }
}
