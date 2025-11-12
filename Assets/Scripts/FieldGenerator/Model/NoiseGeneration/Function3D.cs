using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public abstract class Function3D : ScriptableObject
    {
        public abstract float GetValue(Vector3 p);
    }
}