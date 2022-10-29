using System;
using UnityEngine;

namespace Terraria.NoiseGeneration
{
    public sealed class NoiseSummator : NoiseGeneratorAbstract
    {
        public enum Operation
        {
            Summ = 0,
            Sultiply,
        }

        [Serializable]
        public class NoiseInfo
        {
            public NoiseGeneratorAbstract noise;
            public float weight = 1;
        }

        [SerializeField] private Operation operation;
        [SerializeField] private NoiseInfo[] noises;

        public override float GetPoint(float x, float y)
        {
            if (operation == Operation.Summ)
            {
                float density = 0; 
                float totalWeight = 0;
                foreach (var noise in noises)
                {
                    totalWeight += noise.weight;
                    density += noise.noise.GetPoint(x, y) * noise.weight;
                }

                return density;
            }

            if (operation == Operation.Sultiply)
            {
                float density = 1; 
                float totalWeight = 0;
                foreach (var noise in noises)
                {
                    totalWeight += noise.weight;
                    density *= noise.noise.GetPoint(x, y) * noise.weight;
                }

                return density;
            }

            throw new NotSupportedException();
        }
    }
}