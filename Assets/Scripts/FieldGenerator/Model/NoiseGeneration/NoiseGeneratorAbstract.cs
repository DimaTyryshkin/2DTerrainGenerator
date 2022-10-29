using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public abstract class NoiseGeneratorAbstract : ScriptableObject
    {
        public abstract float GetPoint(float x, float y);
    }
}