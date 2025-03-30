using System.Reflection;
using EFT;
using EFT.Communications;
using HarmonyLib;
using MOAR.Helpers;
using SPT.Reflection.Patching;

namespace MOAR.Patches
{
    /// <summary>
    /// Displays the current preset with a random flair message on raid start.
    /// </summary>
    public class NotificationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(typeof(GameWorld), "OnGameStarted");

        [PatchPrefix]
        private static bool Prefix()
        {
            if (!Settings.ShowPresetOnRaidStart.Value)
                return false;

            string flair = Plugin.GetFlairMessage();
            string preset = Routers.GetAnnouncePresetName();
            Methods.DisplayMessage($"Current preset is {preset}{flair}", ENotificationIconType.EntryPoint);

            return true;
        }
    }
}
