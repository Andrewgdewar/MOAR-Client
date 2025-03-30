using System;
using BepInEx.Configuration;

namespace MOAR.Helpers
{
    /// <summary>
    /// Custom UI metadata used for BepInEx ConfigurationManager integration.
    /// Enables enhanced editor behavior for config fields like hotkeys and sliders.
    /// </summary>
    internal sealed class ConfigurationManagerAttributes
    {
        /// <summary>
        /// If true, numeric ranges will be shown as percentage sliders.
        /// </summary>
        public bool? ShowRangeAsPercent { get; set; }

        /// <summary>
        /// Custom UI drawing method for this config entry.
        /// </summary>
        public Action<ConfigEntryBase> CustomDrawer { get; set; }

        /// <summary>
        /// Custom UI drawing method for hotkey input fields.
        /// </summary>
        public CustomHotkeyDrawerFunc CustomHotkeyDrawer { get; set; }

        /// <summary>
        /// If false, this config entry will be hidden from the config UI.
        /// </summary>
        public bool? Browsable { get; set; }

        /// <summary>
        /// Optional category or group name to display in the UI.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Optional override for the default value shown in the UI.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// If true, hides the reset-to-default button in the config UI.
        /// </summary>
        public bool? HideDefaultButton { get; set; }

        /// <summary>
        /// If true, hides the setting's display name in the UI.
        /// </summary>
        public bool? HideSettingName { get; set; }

        /// <summary>
        /// Tooltip shown for the setting in the UI.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional label shown instead of the property name.
        /// </summary>
        public string DispName { get; set; }

        /// <summary>
        /// Controls order of display in the configuration UI.
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// If true, this field will be read-only in the UI.
        /// </summary>
        public bool? ReadOnly { get; set; }

        /// <summary>
        /// If true, this field is shown only in advanced mode.
        /// </summary>
        public bool? IsAdvanced { get; set; }

        /// <summary>
        /// Optional function to convert an object to a string for display.
        /// </summary>
        public Func<object, string> ObjToStr { get; set; }

        /// <summary>
        /// Optional function to convert a string back to an object.
        /// </summary>
        public Func<string, object> StrToObj { get; set; }

        /// <summary>
        /// Delegate signature for drawing hotkey config fields.
        /// </summary>
        public delegate void CustomHotkeyDrawerFunc(ConfigEntryBase setting, ref bool isCurrentlyAcceptingInput);
    }
}
