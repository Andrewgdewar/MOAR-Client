using System;

namespace MOAR
{
    /// <summary>
    /// Request model for setting the current preset by name.
    /// </summary>
    public class SetPresetRequest
    {
        public string Preset { get; set; }
    }
}
