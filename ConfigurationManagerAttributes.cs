using System;
using BepInEx.Configuration;

/// <summary>
/// Custom UI metadata for ConfigurationManager integration.
/// </summary>
internal sealed class ConfigurationManagerAttributes
{
    public bool? ShowRangeAsPercent { get; set; }
    public Action<ConfigEntryBase> CustomDrawer { get; set; }
    public CustomHotkeyDrawerFunc CustomHotkeyDrawer { get; set; }
    public bool? Browsable { get; set; }
    public string Category { get; set; }
    public object DefaultValue { get; set; }
    public bool? HideDefaultButton { get; set; }
    public bool? HideSettingName { get; set; }
    public string Description { get; set; }
    public string DispName { get; set; }
    public int? Order { get; set; }
    public bool? ReadOnly { get; set; }
    public bool? IsAdvanced { get; set; }
    public Func<object, string> ObjToStr { get; set; }
    public Func<string, object> StrToObj { get; set; }

    /// <summary>
    /// Custom hotkey drawer callback used for advanced UI rendering.
    /// </summary>
    public delegate void CustomHotkeyDrawerFunc(ConfigEntryBase setting, ref bool isCurrentlyAcceptingInput);
}
