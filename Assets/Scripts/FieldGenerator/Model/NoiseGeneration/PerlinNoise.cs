using Game.GameMath;
using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public sealed class PerlinNoise : Function3D
    {
        [SerializeField] private int seed = 1;
        [SerializeField] private float perlinScale = 1;
        [SerializeField] private float min = 0;
        [SerializeField] private float max = 1;

        PerlinNoise3D perlinNoise;
        Vector3 offset;
        public float Min => min;
        public float Max => max;


        public override float GetValue(Vector3 p)
        {
            if (perlinNoise == null)
            {
                perlinNoise = new PerlinNoise3D(seed);
                offset = GetOffset();
            }

            p += offset;

            //float noiseValue = Mathf.PerlinNoise(p.x * perlinScale, p.y * perlinScale);
            float noiseValue = Mathf.InverseLerp(-1, 1, perlinNoise.Noise(p.x * perlinScale, p.y * perlinScale, p.z * perlinScale));
            return Mathf.Lerp(min, max, noiseValue);
        }

        Vector3 GetOffset()
        {
            FloatRnd rnd = new FloatRnd(seed);
            return new Vector3(rnd.Range(0f, 10000f), rnd.Range(0f, 10000f), rnd.Range(0f, 10000f));
        }
    }
}

