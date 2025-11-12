using System;
using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public sealed class Operator : Function3D
    {
        public enum Operation
        {
            Summ = 0,
            Multiply,
        }

        [Serializable]
        public class NoiseInfo
        {
            public Function3D noise;
            public float weight = 1;
        }

        [SerializeField] private Operation operation;
        [SerializeField] private NoiseInfo[] noises;

        public override float GetValue(Vector3 p)
        {
            if (operation == Operation.Summ)
            {
                float density = 0;
                float totalWeight = 0;
                foreach (NoiseInfo noise in noises)
                {
                    totalWeight += noise.weight;
                    density += noise.noise.GetValue(p) * noise.weight;
                }

                return density;
            }

            if (operation == Operation.Multiply)
            {
                float density = 1;
                float totalWeight = 0;
                foreach (var noise in noises)
                {
                    totalWeight += noise.weight;
                    density *= noise.noise.GetValue(p) * noise.weight;
                }

                return density;
            }

            throw new NotSupportedException();
        }
    }
}