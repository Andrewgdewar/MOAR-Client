namespace MOAR
{
    /// <summary>
    /// Represents a basic 3D position used in spawn-related data exchanges.
    /// </summary>
    public class Ixyz
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Ixyz() { }

        public Ixyz(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString() => $"({x:F1}, {y:F1}, {z:F1})";
    }
}
