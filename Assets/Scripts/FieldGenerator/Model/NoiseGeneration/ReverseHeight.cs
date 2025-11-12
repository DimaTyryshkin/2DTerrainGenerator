using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public sealed class ReverseHeight : Function3D
    {
        public override float GetValue(Vector3 p)
        {
            return -p.y;
        }
    }
}