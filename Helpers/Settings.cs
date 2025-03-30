using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFT.Communications;
using Newtonsoft.Json;
using UnityEngine;
using MOAR.Helpers;

namespace MOAR.Helpers
{
    internal class Settings
    {
        private static ConfigFile _config;
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

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
        public static List<Preset> PresetList;
        public static double LastUpdatedServer;
        public static ManualLogSource Log;

        public static void Init(ConfigFile config)
        {
            _config = config;
            Log = Plugin.LogSource;
            IsFika = Chainloader.PluginInfos.ContainsKey("com.fika.core");

            ServerStoredDefaults = Routers.GetDefaultConfig();
            PresetList = Routers.GetPresetsList()?.ToList() ?? new List<Preset>();
            ServerStoredValues = Routers.GetServerConfigWithOverrides();

            string fallbackPresetName = config.Bind(new ConfigDefinition("1. Main Settings", "Moar Preset Fallback"), "live-like").Value;
            string liveLabel = Routers.GetCurrentPresetLabel()?.Trim().ToLowerInvariant();
            string fallbackName = fallbackPresetName?.Trim().ToLowerInvariant();

            var livePreset = PresetList.FirstOrDefault(p =>
                p.Label?.Trim().ToLowerInvariant() == liveLabel ||
                p.Name?.Trim().ToLowerInvariant() == liveLabel)
                ?? PresetList.FirstOrDefault(p => p.Name?.Trim().ToLowerInvariant() == fallbackName)
                ?? PresetList.FirstOrDefault();

            currentPreset = config.Bind(new ConfigDefinition("1. Main Settings", "Moar Preset"),
                livePreset?.Name ?? "live-like",
                new ConfigDescription("Preset to apply.", new AcceptableValueList<string>(PresetList.Select(p => p.Name).ToArray())));

            ShowPresetOnRaidStart = config.Bind(new ConfigDefinition("1. Main Settings", "Preset Announce On/Off"), true);
            AnnounceKey = config.Bind(new ConfigDefinition("1. Main Settings", "Announce Key"), new KeyboardShortcut(KeyCode.End));

            factionAggression = config.Bind(new ConfigDefinition("1. Main Settings", "Faction Based Aggression On/Off"), false);
            startingPmcs = config.Bind(new ConfigDefinition("1. Main Settings", "Starting PMCS On/Off"), ServerStoredDefaults.startingPmcs);
            spawnSmoothing = config.Bind(new ConfigDefinition("1. Main Settings", "spawnSmoothing On/Off"), ServerStoredDefaults.spawnSmoothing);
            randomSpawns = config.Bind(new ConfigDefinition("1. Main Settings", "randomSpawns On/Off"), ServerStoredDefaults.randomSpawns);
            pmcDifficulty = config.Bind(new ConfigDefinition("1. Main Settings", "Pmc difficulty"), ServerStoredDefaults.pmcDifficulty);
            scavDifficulty = config.Bind(new ConfigDefinition("1. Main Settings", "Scav difficulty"), ServerStoredDefaults.scavDifficulty);

            maxBotCap = config.Bind(new ConfigDefinition("2. Custom game Settings", "MaxBotCap"), ServerStoredDefaults.maxBotCap);
            maxBotPerZone = config.Bind(new ConfigDefinition("2. Custom game Settings", "MaxBotPerZone"), ServerStoredDefaults.maxBotPerZone);

            BindGroupSettings();
            BindZombieSettings();
            BindWaveSettings();
            BindDebugSettings();
            BindHotkeySettings();

            currentPreset.SettingChanged += (_, _) => OnPresetChange();
            spawnSmoothing.SettingChanged += (_, _) => OnStartingPmcsChanged();
            randomSpawns.SettingChanged += (_, _) => OnStartingPmcsChanged();
            startingPmcs.SettingChanged += (_, _) => OnStartingPmcsChanged();

            if (!IsFika && ShowPresetOnRaidStart.Value && livePreset != null)
                Methods.DisplayMessage($"Live preset: {livePreset.Label}", ENotificationIconType.Quest);
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
            AddBotSpawn = _config.Bind(new ConfigDefinition("4. Advanced", "Add a bot spawn"), default(KeyboardShortcut));
            AddSniperSpawn = _config.Bind(new ConfigDefinition("4. Advanced", "Add a sniper spawn"), default(KeyboardShortcut));
            DeleteBotSpawn = _config.Bind(new ConfigDefinition("4. Advanced", "Delete a bot spawn"), default(KeyboardShortcut));
            AddPlayerSpawn = _config.Bind(new ConfigDefinition("4. Advanced", "Add a player spawn"), default(KeyboardShortcut));
            enablePointOverlay = _config.Bind(new ConfigDefinition("4. Advanced", "Spawnpoint overlay On/Off"), false);
        }

        public static void AnnounceManually()
        {
            if (IsFika) return;
            var selected = PresetList.FirstOrDefault(p => p.Name == currentPreset.Value);
            if (selected != null)
                Methods.DisplayMessage($"Current preset: {selected.Label}", ENotificationIconType.Quest);
            else
                Methods.DisplayMessage("Unknown preset selected", ENotificationIconType.Alert);
        }

        private static void OnPresetChange()
        {
            var selected = PresetList.FirstOrDefault(p => p.Name == currentPreset.Value);
            if (selected != null)
            {
                string label = selected.Label ?? selected.Name;
                if (!IsFika) Methods.DisplayMessage($"Current preset: {label}", ENotificationIconType.Quest);
                ApplyPresetSettings(selected.Settings);
                Routers.SetPreset(selected.Name);
            }
            else
            {
                Methods.DisplayMessage("Unknown preset selected", ENotificationIconType.Alert);
            }
        }

        private static void ApplyPresetSettings(object settings)
        {
            if (settings == null) return;

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(settings));

                void TrySet<T>(string key, Action<T> apply)
                {
                    if (dict.TryGetValue(key, out var value))
                    {
                        try { apply((T)Convert.ChangeType(value, typeof(T))); } catch { }
                    }
                }

                TrySet<bool>("randomSpawns", val => randomSpawns.Value = val);
                TrySet<bool>("spawnSmoothing", val => spawnSmoothing.Value = val);
                TrySet<bool>("startingPmcs", val => startingPmcs.Value = val);

                TrySet<double>("scavGroupChance", val => scavGroupChance.Value = val);
                TrySet<int>("scavMaxGroupSize", val => scavMaxGroupSize.Value = val);
                TrySet<double>("pmcGroupChance", val => pmcGroupChance.Value = val);
                TrySet<int>("pmcMaxGroupSize", val => pmcMaxGroupSize.Value = val);

                TrySet<double>("scavWaveQuantity", val => scavWaveQuantity.Value = val);
                TrySet<double>("pmcWaveQuantity", val => pmcWaveQuantity.Value = val);

                TrySet<int>("mainBossChanceBuff", val => mainBossChanceBuff.Value = val);
                TrySet<bool>("bossOpenZones", val => bossOpenZones.Value = val);
                TrySet<bool>("bossInvasion", val => bossInvasion.Value = val);
                TrySet<int>("bossInvasionSpawnChance", val => bossInvasionSpawnChance.Value = val);
                TrySet<bool>("gradualBossInvasion", val => gradualBossInvasion.Value = val);

                TrySet<bool>("randomRaiderGroup", val => randomRaiderGroup.Value = val);
                TrySet<int>("randomRaiderGroupChance", val => randomRaiderGroupChance.Value = val);
                TrySet<bool>("randomRogueGroup", val => randomRogueGroup.Value = val);
                TrySet<int>("randomRogueGroupChance", val => randomRogueGroupChance.Value = val);

                TrySet<int>("sniperMaxGroupSize", val => sniperMaxGroupSize.Value = val);
                TrySet<double>("sniperGroupChance", val => sniperGroupChance.Value = val);

            }
            catch (Exception ex)
            {
                Log.LogError($"[ApplyPresetSettings] Failed to apply preset: {ex.Message}");
            }
        }

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
