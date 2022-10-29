using UnityEngine;

namespace Terraria.NoiseGeneration
{
    public abstract class NoiseGeneratorAbstract : ScriptableObject
    {
        public abstract float GetPoint(float x, float y);
    }
}