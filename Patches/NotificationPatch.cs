using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using HarmonyLib;
using MOAR.Helpers;
using SPT.Reflection.Patching;

namespace MOAR.Patches
{
    public class NotificationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(
                typeof(MetricsEventsClass),
                nameof(MetricsEventsClass.SetLocationLoaded)
            );
        }

        [PatchPrefix]
        static bool Prefix()
        {
            if (!Settings.ShowPresetOnRaidStart.Value)
                return false;

            Methods.DisplayMessage(
                "Preset set to " + Routers.GetAnnouncePresetName(),
                EFT.Communications.ENotificationIconType.EntryPoint
            );

            return true;
        }
    }

    public class NotificationPatch2 : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPrefix]
        static bool Prefix()
        {
            if (!Settings.ShowPresetOnRaidStart.Value)
                return false;

            List<string> messages =
            [
                ", good luck!",
                ", may the bots ever be in your favour.",
                ", you're probably screwed.",
                ", may your raids be bug-free.",
                ", enjoy the dumpster fire.",
                ", hope you brought snacks.",
                ", good luck, seriously.",
                ", prepare to be crushed.",
                ", you’re about to get wrecked.",
                ", enjoy the show.",
                ", good luck, you'll need it.",
                ", enjoy the carnage.",
                ", try not to rage-quit.",
                ", don’t say I didn’t warn you.",
                ", best of luck surviving that.",
                ", it’s going to be a long day for you.",
                ", be water my friend.",
                ", let the feelings of dread pass over you.",
                ", black a leg!",
                ", it's about to get ugly. Enjoy.",
            ];

            Random randNum = new Random();

            int aRandomPos = randNum.Next(messages.Count);

            string currName = messages[aRandomPos];

            Methods.DisplayMessage(
                "Current preset is " + Routers.GetAnnouncePresetName() + currName,
                EFT.Communications.ENotificationIconType.EntryPoint
            );

            return true;
        }
    }
}
