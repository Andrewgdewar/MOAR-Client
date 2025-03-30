using MOAR.Helpers;

namespace MOAR
{
    /// <summary>
    /// Represents a response from the server containing a list of spawn presets.
    /// </summary>
    public class GetPresetsListResponse
    {
        public Preset[] data { get; set; }
    }
}
