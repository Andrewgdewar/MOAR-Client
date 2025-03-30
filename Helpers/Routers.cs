using System;
using System.Linq;
using BepInEx.Configuration;
using Newtonsoft.Json;
using SPT.Common.Http;
using MOAR.Helpers;

namespace MOAR.Helpers
{
    internal class Routers
    {
        public static void Init(ConfigFile config) { }

        // --- Preset Accessors ---

        public static string GetCurrentPresetLabel() => RequestHandler.GetJson("/moar/currentPreset");
        public static string GetAnnouncePresetLabel() => RequestHandler.GetJson("/moar/announcePreset");

        public static string GetCurrentPresetName()
        {
            var label = GetCurrentPresetLabel();
            var preset = Settings.PresetList?.FirstOrDefault(p => p.Label == label);
            return preset?.Name ?? "Unknown";
        }

        public static string GetAnnouncePresetName()
        {
            var label = GetAnnouncePresetLabel();
            var preset = Settings.PresetList?.FirstOrDefault(p => p.Label == label);
            return preset?.Name ?? "Unknown";
        }

        public static string SetPreset(string label)
        {
            try
            {
                var request = new SetPresetRequest { Preset = label };
                return RequestHandler.PostJson("/moar/setPreset", JsonConvert.SerializeObject(request));
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"[SetPreset] Failed to set preset '{label}': {ex.Message}");
                return "Failed to set preset.";
            }
        }

        public static Preset[] GetPresetsList()
        {
            try
            {
                var json = RequestHandler.GetJson("/moar/getPresets");
                var response = JsonConvert.DeserializeObject<GetPresetsListResponse>(json);
                return response?.data ?? Array.Empty<Preset>();
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"[GetPresetsList] Failed to fetch presets: {ex.Message}");
                return Array.Empty<Preset>();
            }
        }

        // --- Server Config ---

        public static ConfigSettings GetServerConfigWithOverrides()
        {
            try
            {
                var json = RequestHandler.GetJson("/moar/getServerConfig");
                return JsonConvert.DeserializeObject<ConfigSettings>(json) ?? new ConfigSettings();
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"[GetServerConfigWithOverrides] Error: {ex.Message}");
                return new ConfigSettings();
            }
        }

        public static ConfigSettings GetDefaultConfig()
        {
            try
            {
                var json = RequestHandler.GetJson("/moar/getDefaultConfig");
                return JsonConvert.DeserializeObject<ConfigSettings>(json) ?? new ConfigSettings();
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"[GetDefaultConfig] Error: {ex.Message}");
                return new ConfigSettings();
            }
        }

        public static void SetOverrideConfig(ConfigSettings settings)
        {
            try
            {
                RequestHandler.PostJson("/moar/setConfig", JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"[SetOverrideConfig] Failed to push config: {ex.Message}");
            }
        }

        // --- Spawn Tooling ---

        public static string AddBotSpawn() => PostPlayerLocationTo("/moar/addBotSpawn");
        public static string AddSniperSpawn() => PostPlayerLocationTo("/moar/addSniperSpawn");
        public static string DeleteBotSpawn() => PostPlayerLocationTo("/moar/deleteBotSpawn");
        public static string AddPlayerSpawn() => PostPlayerLocationTo("/moar/addPlayerSpawn");

        private static string PostPlayerLocationTo(string endpoint)
        {
            var request = Methods.GetPlayersCoordinatesAndLevel();
            return RequestHandler.PostJson(endpoint, JsonConvert.SerializeObject(request));
        }
    }
}
