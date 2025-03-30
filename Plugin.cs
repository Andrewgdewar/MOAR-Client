using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.Communications;
using HarmonyLib;
using MOAR.Helpers;
using MOAR.Patches;

namespace MOAR
{
    [BepInPlugin("MOAR.settings", "MOAR", "3.0.1")]
    [BepInDependency("com.fika.core", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;
        private static readonly Random _rng = new Random();

        private void Awake()
        {
            LogSource = Logger;
            new Harmony("com.moar.patches").PatchAll();
        }

        private void Start()
        {
            Settings.Init(Config);
            Routers.Init(Config);
            EnablePatches();
        }

        private void EnablePatches()
        {
            new SniperPatch().Enable();
            new AddEnemyPatch().Enable();
            new NotificationPatch().Enable();

            if (Settings.enablePointOverlay.Value)
                new OnGameStartedPatch().Enable();
        }

        private void Update()
        {
            if (TryPress(Settings.DeleteBotSpawn.Value))
                AnnounceResult(Routers.DeleteBotSpawn(), "Deleted 1 bot spawn point");

            if (TryPress(Settings.AddBotSpawn.Value))
                AnnounceResult(Routers.AddBotSpawn(), "Added 1 bot spawn point");

            if (TryPress(Settings.AddSniperSpawn.Value))
                AnnounceResult(Routers.AddSniperSpawn(), "Added 1 sniper spawn point");

            if (TryPress(Settings.AddPlayerSpawn.Value))
                AnnounceResult(Routers.AddPlayerSpawn(), "Added 1 player spawn point");

            if (Settings.AnnounceKey.Value.BetterIsDown())
                Methods.DisplayMessage($"Current preset is {Routers.GetAnnouncePresetName()}", ENotificationIconType.EntryPoint);
        }

        private static bool TryPress(KeyboardShortcut shortcut) =>
            shortcut.BetterIsDown() && Singleton<GameWorld>.Instance?.MainPlayer != null;

        private static void AnnounceResult(string result, string fallbackMessage)
        {
            var location = Singleton<GameWorld>.Instance?.MainPlayer?.Location ?? "Unknown";
            var message = string.IsNullOrWhiteSpace(result) ? fallbackMessage : result;
            Methods.DisplayMessage($"{message} in {location}", ENotificationIconType.Default);
        }

        public static string GetFlairMessage()
        {
            var suffixes = new List<string>
            {
                ", good luck!", ", may the bots ever be in your favour.", ", you're probably screwed.",
                ", may your raids be bug-free.", ", enjoy the dumpster fire.", ", hope you brought snacks.",
                ", good luck, seriously.", ", prepare to be crushed.", ", you're about to get wrecked.", ", enjoy the show.",
                ", good luck, you'll need it.", ", enjoy the carnage.", ", try not to rage-quit.", ", don't say I didn't warn you.",
                ", best of luck surviving that.", ", it's going to be a long day for you.", ", be water my friend.",
                ", let the feelings of dread pass over you.", ", black a leg!", ", it's about to get ugly. Enjoy."
            };

            return suffixes[_rng.Next(suffixes.Count)];
        }
    }
}
