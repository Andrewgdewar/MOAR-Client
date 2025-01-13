using System;
using BepInEx.Configuration;
using Newtonsoft.Json;
namespace MOAR.Helpers
{
    internal class Routers
    {
        public static string GetCurrentPresetLabel()
        {
            var req = SPT.Common.Http.RequestHandler.GetJson("/moar/currentPreset");
            return req;
        }

        public static string GetAnnouncePresetLabel()
        {
            var req = SPT.Common.Http.RequestHandler.GetJson("/moar/announcePreset");
            return req;
        }

        public static string GetAnnouncePresetName()
        {
            var preset = GetAnnouncePresetLabel();

            var result = Array.Find(Settings.PresetList, (item) => item.Label.Equals(preset))?.Name;

            return result;
        }

        public static string GetCurrentPresetName()
        {
            var preset = GetCurrentPresetLabel();

            var result = Array.Find(Settings.PresetList, (item) => item.Label.Equals(preset))?.Name;

            return result;
        }

        public static Preset[] GetPresetsList()
        {
            return JsonConvert
                    .DeserializeObject<GetPresetsListResponse>(
                        SPT.Common.Http.RequestHandler.GetJson("/moar/getPresets")
                    )
                    ?.data ?? [];
        }

        public static ConfigSettings GetDefaultConfig()
        {
            return JsonConvert.DeserializeObject<ConfigSettings>(
                SPT.Common.Http.RequestHandler.GetJson("/moar/getDefaultConfig")
            );
        }

        public static ConfigSettings GetServerConfigWithOverrides()
        {
            return JsonConvert.DeserializeObject<ConfigSettings>(
                SPT.Common.Http.RequestHandler.GetJson("/moar/getServerConfigWithOverrides")
            );
        }

        public static string SetPreset(string preset)
        {
            var request = new SetPresetRequest { Preset = preset };

            var req = SPT.Common.Http.RequestHandler.PostJson(
                "/moar/setPreset",
                JsonConvert.SerializeObject(request)
            );

            return req.ToString(); // no need to parse bare strings
        }

        public static bool SetOverrideConfig(ConfigSettings configs)
        {
            var req = SPT.Common.Http.RequestHandler.PostJson(
                "/moar/setOverrideConfig",
                JsonConvert.SerializeObject(configs)
            );

            return true; // no need to parse bare strings
        }

        public static void Init(ConfigFile config) { }
    }
}
