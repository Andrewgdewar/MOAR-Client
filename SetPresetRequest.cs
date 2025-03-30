using System;

namespace MOAR
{
    /// <summary>
    /// Request payload sent from client to server to change the active spawn preset.
    /// </summary>
    public class SetPresetRequest
    {
        /// <summary>
        /// The internal name or label of the preset to activate.
        /// </summary>
        public string Preset { get; set; }
    }
}
