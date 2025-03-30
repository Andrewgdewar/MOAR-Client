using System.Linq;
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
                return true;

            // Don't double-announce in FIKA since host will handle the message
            if (Settings.IsFika)
                return true;

            string flair = Plugin.GetFlairMessage();

            var selected = Settings.PresetList.FirstOrDefault(p => p.Name == Settings.currentPreset.Value);
            string label = selected?.Label ?? Settings.currentPreset.Value ?? "Unknown";

            Methods.DisplayMessage($"Current preset is {label}{flair}", ENotificationIconType.EntryPoint);

            return true;
        }
    }
}
