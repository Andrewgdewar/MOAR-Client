using BepInEx;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using HarmonyLib;
using MOAR.Helpers;
using MOAR.Patches;

namespace MOAR
{
    [
        BepInPlugin("MOAR.settings", "MOAR", "3.0.1"),
        BepInDependency("com.fika.core", BepInDependency.DependencyFlags.SoftDependency)
    ]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;

        private void Awake()
        {
            var harmony = new Harmony("com.example.botzonepatch");
            harmony.PatchAll();
        }

        private void Start()
        {
            LogSource = Logger;

            Settings.Init(Config);
            Routers.Init(Config);

            new SniperPatch().Enable();
            new AddEnemyPatch().Enable();
            if (Settings.enablePointOverlay.Value)
            {
                new OnGameStartedPatch().Enable();
            }

            new NotificationPatch().Enable();
        }

        private void Update()
        {
            if (Settings.DeleteBotSpawn.Value.BetterIsDown())
            {
                if (Singleton<GameWorld>.Instance.MainPlayer != null)
                {
                    Routers.DeleteBotSpawn();
                    Methods.DisplayMessage(
                        "Deleted 1 bot spawn point from "
                        + Singleton<GameWorld>.Instance.MainPlayer.Location,
                        EFT.Communications.ENotificationIconType.Default
                    );
                }
            }

            if (Settings.AddBotSpawn.Value.BetterIsDown())
            {
                if (Singleton<GameWorld>.Instance.MainPlayer != null)
                {
                    Routers.AddBotSpawn();
                    Methods.DisplayMessage(
                        "Added 1 bot spawn point to "
                        + Singleton<GameWorld>.Instance.MainPlayer.Location,
                        EFT.Communications.ENotificationIconType.Default
                    );
                }
            }

            if (Settings.AddSniperSpawn.Value.BetterIsDown())
            {
                if (Singleton<GameWorld>.Instance.MainPlayer != null)
                {
                    Routers.AddSniperSpawn();
                    Methods.DisplayMessage(
                        "Added 1 sniper spawn point to "
                        + Singleton<GameWorld>.Instance.MainPlayer.Location,
                        EFT.Communications.ENotificationIconType.Default
                    );
                }
            }

            if (Settings.AddPlayerSpawn.Value.BetterIsDown())
            {
                if (Singleton<GameWorld>.Instance.MainPlayer != null)
                {
                    Routers.AddPlayerSpawn();
                    Methods.DisplayMessage(
                        "Added 1 player spawn point to "
                        + Singleton<GameWorld>.Instance.MainPlayer.Location,
                        EFT.Communications.ENotificationIconType.Default
                    );
                }
            }

            if (Settings.AnnounceKey.Value.BetterIsDown())
            {
                Methods.DisplayMessage(
                    "Current preset is " + Routers.GetAnnouncePresetName(),
                    EFT.Communications.ENotificationIconType.EntryPoint
                );
            }
        }
    };
}