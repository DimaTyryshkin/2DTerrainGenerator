using UnityEngine;

namespace Terraria
{
    public abstract class NoiseGeneratorAbstract : ScriptableObject
    {
        public abstract float GetPoint(float x, float y);
    }
}