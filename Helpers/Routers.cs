using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx.Configuration;
using Newtonsoft.Json;
using SPT.Common.Http;
using MOAR.Helpers;

namespace MOAR.Helpers
{
    /// <summary>
    /// Handles all server API routing logic and HTTP requests for config and presets.
    /// </summary>
    internal static class Routers
    {
        public static void Init(ConfigFile config) { }

        // --- Preset Accessors ---

        public static string GetCurrentPresetLabel()
        {
            try
            {
                return RequestHandler.GetJson("/moar/currentPreset")?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogWarning($"[GetCurrentPresetLabel] Falling back to client config: {ex.Message}");
                return Settings.currentPreset?.Value ?? "live-like";
            }
        }

        public static string GetAnnouncePresetLabel()
        {
            try
            {
                return RequestHandler.GetJson("/moar/announcePreset")?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogWarning($"[GetAnnouncePresetLabel] Falling back to client config: {ex.Message}");
                return Settings.currentPreset?.Value ?? "live-like";
            }
        }

        public static string GetCurrentPresetName()
        {
            var label = GetCurrentPresetLabel();
            var preset = FindPresetByLabel(label);
            return preset?.Name ?? "Unknown";
        }

        public static string GetAnnouncePresetName()
        {
            var label = GetAnnouncePresetLabel();
            var preset = FindPresetByLabel(label);
            return preset?.Name ?? "Unknown";
        }

        private static Preset FindPresetByLabel(string label)
        {
            return Settings.PresetList?.FirstOrDefault(p =>
                p.Label.Equals(label, StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals(label, StringComparison.OrdinalIgnoreCase));
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

        public static List<Preset> GetPresetsList()
        {
            try
            {
                var json = RequestHandler.GetJson("/moar/getPresets");
                var response = JsonConvert.DeserializeObject<GetPresetsListResponse>(json);
                return response?.data?.ToList() ?? new List<Preset>();
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"[GetPresetsList] Failed to fetch presets: {ex.Message}");
                return new List<Preset>();
            }
        }

        // --- Server Config Management ---

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
                var json = JsonConvert.SerializeObject(settings);
                RequestHandler.PostJson("/moar/setConfig", json);
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
            try
            {
                var request = Methods.GetPlayersCoordinatesAndLevel();
                var json = JsonConvert.SerializeObject(request);
                return RequestHandler.PostJson(endpoint, json);
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"[PostPlayerLocationTo] Failed to post to {endpoint}: {ex.Message}");
                return "Error submitting player position.";
            }
        }
    }
}