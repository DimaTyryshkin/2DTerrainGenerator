using UnityEngine;

namespace Terraria.NoiseGeneration
{
    public sealed class WaterLine : NoiseGeneratorAbstract
    {
        [SerializeField] private NoiseGeneratorAbstract noiseGenerator;
      
        [SerializeField] private Terrain terrain;

        private Vector2 offset;

        public override float GetPoint(float x, float y)
        {
            float noiseValue = noiseGenerator.GetPoint(x, y);
            float heightUnderWater = y;
            float density = noiseValue - heightUnderWater * terrain.HeightScale;
            return density;
        }
    }
}