using System;
using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public sealed class NoiseGenerator : NoiseGeneratorAbstract
    {
        [SerializeField] private int seed = 1; 
        [SerializeField] private float perlinScale = 1; 
        [SerializeField] private float min = 0; 
        [SerializeField] private float max = 1; 

        Vector2 offset;

        public float Min => min;
        public float Max => max;


        public override float GetPoint(float x, float y)
        {
            offset = GenerateOffset();
            float noiseValue = Mathf.PerlinNoise((x + offset.x) * perlinScale, (y + offset.y) * perlinScale);
            return Mathf.Lerp(min, max, noiseValue);
        }

        Vector2 GenerateOffset()
        {
            FloatRnd rnd = new FloatRnd(seed);
            return new Vector2(rnd.Range(0f, 10000f), rnd.Range(0f, 10000f));
        }
    }
}