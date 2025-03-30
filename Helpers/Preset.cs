using System;

namespace MOAR.Helpers
{
    /// <summary>
    /// Represents a named spawn configuration preset with an optional object-based settings payload.
    /// </summary>
    public class Preset
    {
        /// <summary>
        /// The internal name identifier for the preset (used as key).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The user-facing display label for the preset.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The arbitrary settings object applied when the preset is selected.
        /// </summary>
        public object Settings { get; set; }

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public Preset() { }

        /// <summary>
        /// Creates a new preset with the specified name, label, and settings.
        /// </summary>
        public Preset(string name, string label, object settings)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Label = label ?? name;
            Settings = settings ?? new object();
        }

        /// <summary>
        /// Returns the label or fallback name for debug or UI display.
        /// </summary>
        public override string ToString() => Label ?? Name ?? "Unnamed Preset";
    }
}
