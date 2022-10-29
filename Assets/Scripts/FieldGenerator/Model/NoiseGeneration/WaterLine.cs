using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public sealed class WaterLine : NoiseGeneratorAbstract
    {
        [SerializeField, IsntNull]  NoiseGeneratorAbstract noiseGenerator;
        [SerializeField, IsntNull]  NoiseField noiseField;

        private Vector2 offset;

        public override float GetPoint(float x, float y)
        {
            float noiseValue = noiseGenerator.GetPoint(x, y);
            float heightUnderWater = y;
            float density = noiseValue - heightUnderWater * noiseField.HeightScale;
            return density;
        }
    }
}