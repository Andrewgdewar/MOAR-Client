using System;
using System.Linq;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFT.Communications;
using UnityEngine;
using MOAR.Helpers;

namespace MOAR.Helpers
{
    internal class Settings
    {
        private static ConfigFile _config;

        public static ConfigEntry<bool> ShowPresetOnRaidStart;
        public static bool IsFika;
        public static ConfigSettings ServerStoredValues;
        public static ConfigSettings ServerStoredDefaults;

        public static ConfigEntry<bool> debug;
        public static ConfigEntry<bool> factionAggression;
        public static ConfigEntry<bool> enablePointOverlay;

        public static ConfigEntry<double> pmcDifficulty;
        public static ConfigEntry<double> scavDifficulty;

        public static ConfigEntry<bool> zombiesEnabled;
        public static ConfigEntry<double> zombieWaveDistribution;
        public static ConfigEntry<double> zombieWaveQuantity;
        public static ConfigEntry<double> zombieHealth;

        public static ConfigEntry<double> scavWaveDistribution;
        public static ConfigEntry<double> scavWaveQuantity;
        public static ConfigEntry<double> pmcWaveDistribution;
        public static ConfigEntry<double> pmcWaveQuantity;

        public static ConfigEntry<bool> startingPmcs;
        public static ConfigEntry<bool> spawnSmoothing;
        public static ConfigEntry<bool> randomSpawns;

        public static ConfigEntry<int> maxBotCap;
        public static ConfigEntry<int> maxBotPerZone;

        public static ConfigEntry<double> scavGroupChance;
        public static ConfigEntry<double> pmcGroupChance;
        public static ConfigEntry<double> sniperGroupChance;

        public static ConfigEntry<int> pmcMaxGroupSize;
        public static ConfigEntry<int> scavMaxGroupSize;
        public static ConfigEntry<double> sniperMaxGroupSize;

        public static ConfigEntry<bool> bossOpenZones;
        public static ConfigEntry<bool> randomRaiderGroup;
        public static ConfigEntry<int> randomRaiderGroupChance;
        public static ConfigEntry<bool> randomRogueGroup;
        public static ConfigEntry<int> randomRogueGroupChance;
        public static ConfigEntry<bool> disableBosses;
        public static ConfigEntry<int> mainBossChanceBuff;
        public static ConfigEntry<bool> bossInvasion;
        public static ConfigEntry<int> bossInvasionSpawnChance;
        public static ConfigEntry<bool> gradualBossInvasion;

        public static ConfigEntry<KeyboardShortcut> DeleteBotSpawn;
        public static ConfigEntry<KeyboardShortcut> AddBotSpawn;
        public static ConfigEntry<KeyboardShortcut> AddSniperSpawn;
        public static ConfigEntry<KeyboardShortcut> AddPlayerSpawn;
        public static ConfigEntry<KeyboardShortcut> AnnounceKey;

        public static ConfigEntry<string> currentPreset;
        public static Preset[] PresetList;
        public static double LastUpdatedServer;
        public static ManualLogSource Log;

        public static void Init(ConfigFile config)
        {
            _config = config;
            Log = Plugin.LogSource;
            IsFika = Chainloader.PluginInfos.ContainsKey("com.fika.core");

            ServerStoredDefaults = Routers.GetDefaultConfig();
            PresetList = Routers.GetPresetsList();
            ServerStoredValues = Routers.GetServerConfigWithOverrides();

            string fallbackPresetName = _config.Bind("1. Main Settings", "Moar Preset", "live-like").Value;
            string liveLabel = Routers.GetCurrentPresetLabel()?.Trim().ToLowerInvariant();
            string fallbackName = fallbackPresetName?.Trim().ToLowerInvariant();

            var livePreset = PresetList.FirstOrDefault(p =>
                p.Label?.Trim().ToLowerInvariant() == liveLabel ||
                p.Name?.Trim().ToLowerInvariant() == liveLabel)
                ?? PresetList.FirstOrDefault(p => p.Name?.Trim().ToLowerInvariant() == fallbackName)
                ?? PresetList.First();

            currentPreset = _config.Bind("1. Main Settings", "Moar Preset", livePreset.Name, new ConfigDescription("Preset to apply.", new AcceptableValueList<string>(PresetList.Select(p => p.Name).ToArray())));
            ShowPresetOnRaidStart = _config.Bind("1. Main Settings", "Preset Announce On/Off", true, "Announce preset at raid start");
            AnnounceKey = _config.Bind("1. Main Settings", "Announce Key", new KeyboardShortcut(KeyCode.End), "Hotkey to announce current preset");

            factionAggression = _config.Bind("1. Main Settings", "Faction Based Aggression On/Off", false);
            startingPmcs = _config.Bind("1. Main Settings", "Starting PMCS On/Off", ServerStoredDefaults.startingPmcs);
            spawnSmoothing = _config.Bind("1. Main Settings", "spawnSmoothing On/Off", ServerStoredDefaults.spawnSmoothing);
            randomSpawns = _config.Bind("1. Main Settings", "randomSpawns On/Off", ServerStoredDefaults.randomSpawns);
            pmcDifficulty = _config.Bind("1. Main Settings", "Pmc difficulty", ServerStoredDefaults.pmcDifficulty);
            scavDifficulty = _config.Bind("1. Main Settings", "Scav difficulty", ServerStoredDefaults.scavDifficulty);

            maxBotCap = _config.Bind("2. Custom game Settings", "MaxBotCap", ServerStoredDefaults.maxBotCap);
            maxBotPerZone = _config.Bind("2. Custom game Settings", "MaxBotPerZone", ServerStoredDefaults.maxBotPerZone);

            BindGroupSettings();
            BindZombieSettings();
            BindWaveSettings();
            BindDebugSettings();
            BindHotkeySettings();

            currentPreset.SettingChanged += (_, _) => OnPresetChange();
            spawnSmoothing.SettingChanged += (_, _) => OnStartingPmcsChanged();
            randomSpawns.SettingChanged += (_, _) => OnStartingPmcsChanged();
            startingPmcs.SettingChanged += (_, _) => OnStartingPmcsChanged();

            if (ShowPresetOnRaidStart.Value)
                Methods.DisplayMessage($"Live preset: {livePreset.Label}", ENotificationIconType.Quest);

            if (IsFika)
                Log.LogInfo("FIKA detected: awaiting manual settings pull");
        }

        private static void BindGroupSettings()
        {
            scavGroupChance = _config.Bind("2. Custom game Settings", "scavGroupChance Percentage", ServerStoredDefaults.scavGroupChance);
            pmcGroupChance = _config.Bind("2. Custom game Settings", "pmcGroupChance Percentage", ServerStoredDefaults.pmcGroupChance);
            sniperGroupChance = _config.Bind("2. Custom game Settings", "sniperGroupChance Percentage", ServerStoredDefaults.sniperGroupChance);
            pmcMaxGroupSize = _config.Bind("2. Custom game Settings", "pmcMaxGroupSize", ServerStoredDefaults.pmcMaxGroupSize);
            scavMaxGroupSize = _config.Bind("2. Custom game Settings", "scavMaxGroupSize", ServerStoredDefaults.scavMaxGroupSize);
            sniperMaxGroupSize = _config.Bind("2. Custom game Settings", "sniperMaxGroupSize", ServerStoredDefaults.sniperMaxGroupSize);
        }

        private static void BindZombieSettings()
        {
            zombiesEnabled = _config.Bind("2. Custom game Settings", "zombiesEnabled On/Off", ServerStoredDefaults.zombiesEnabled);
            zombieHealth = _config.Bind("2. Custom game Settings", "ZombieHealth", ServerStoredDefaults.zombieHealth);
        }

        private static void BindWaveSettings()
        {
            zombieWaveQuantity = _config.Bind("2. Custom game Settings", "ZombieWaveQuantity", ServerStoredDefaults.zombieWaveQuantity);
            zombieWaveDistribution = _config.Bind("2. Custom game Settings", "ZombieWaveDistribution", ServerStoredDefaults.zombieWaveDistribution);
            scavWaveQuantity = _config.Bind("2. Custom game Settings", "ScavWaveQuantity", ServerStoredDefaults.scavWaveQuantity);
            scavWaveDistribution = _config.Bind("2. Custom game Settings", "ScavWaveDistribution", ServerStoredDefaults.scavWaveDistribution);
            pmcWaveQuantity = _config.Bind("2. Custom game Settings", "PmcWaveQuantity", ServerStoredDefaults.pmcWaveQuantity);
            pmcWaveDistribution = _config.Bind("2. Custom game Settings", "PmcWaveDistribution", ServerStoredDefaults.pmcWaveDistribution);
        }

        private static void BindDebugSettings()
        {
            debug = _config.Bind("3.Debug", "debug On/Off", ServerStoredDefaults.debug);
        }

        private static void BindHotkeySettings()
        {
            AddBotSpawn = _config.Bind("4. Advanced", "Add a bot spawn", default(KeyboardShortcut));
            AddSniperSpawn = _config.Bind("4. Advanced", "Add a sniper spawn", default(KeyboardShortcut));
            DeleteBotSpawn = _config.Bind("4. Advanced", "Delete a bot spawn", default(KeyboardShortcut));
            AddPlayerSpawn = _config.Bind("4. Advanced", "Add a player spawn", default(KeyboardShortcut));
            enablePointOverlay = _config.Bind("4. Advanced", "Spawnpoint overlay On/Off", false);
        }

        private static void OnPresetChange()
        {
            var selected = PresetList.FirstOrDefault(p => p.Name == currentPreset.Value);
            if (selected != null)
            {
                string label = selected.Label ?? selected.Name;
                Methods.DisplayMessage($"Current preset: {label}", ENotificationIconType.Quest);
                ApplyPresetSettings(selected.Settings);

                // Only call SetPreset if the server isn't already using this preset
                var serverLabel = Routers.GetCurrentPresetLabel()?.Trim().ToLowerInvariant();
                var selectedLabel = selected.Label?.Trim().ToLowerInvariant();

                if (!string.Equals(serverLabel, selectedLabel, StringComparison.OrdinalIgnoreCase))
                {
                    Routers.SetPreset(selected.Name);
                    ServerStoredValues = Routers.GetServerConfigWithOverrides(); // Refresh
                }
            }
            else
            {
                Methods.DisplayMessage("Unknown preset selected", ENotificationIconType.Alert);
            }
        }


        private static void ApplyPresetSettings(object settings) { }

        private static void OnStartingPmcsChanged()
        {
            if (startingPmcs.Value)
            {
                randomSpawns.Value = true;
                spawnSmoothing.Value = false;
            }
        }
    }


}
