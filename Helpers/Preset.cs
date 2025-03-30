namespace MOAR.Helpers
{
    /// <summary>
    /// Represents a named spawn configuration preset.
    /// </summary>
    public class Preset
    {
        public string Name { get; }
        public string Label { get; }
        public object Settings { get; }

        public Preset(string name, string label, object settings)
        {
            Name = name;
            Label = label;
            Settings = settings;
        }
    }
}
