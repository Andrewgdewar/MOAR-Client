using System.Collections.Generic;

namespace MOAR.Helpers
{
    /// <summary>
    /// Represents a deserialized server response containing available AI spawn presets.
    /// Used by the client to populate the dropdown and apply server-defined presets.
    /// </summary>
    public class GetPresetsListResponse
    {
        /// <summary>
        /// The list of all defined presets returned from the server.
        /// </summary>
        public List<Preset> data { get; set; }
    }
}
