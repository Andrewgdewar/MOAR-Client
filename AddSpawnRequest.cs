using System;

namespace MOAR
{
    /// <summary>
    /// Represents a spawn request sent from the client to the server, including map name and position.
    /// </summary>
    public class AddSpawnRequest
    {
        public string map { get; set; }
        public Ixyz position { get; set; }

        public AddSpawnRequest() { }

        public AddSpawnRequest(string map, Ixyz position)
        {
            this.map = map;
            this.position = position;
        }

        public override string ToString() => $"Map: {map}, Position: {position}";
    }
}
