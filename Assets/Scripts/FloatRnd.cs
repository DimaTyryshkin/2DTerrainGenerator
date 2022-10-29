using System;

namespace Terraria
{
    public sealed class FloatRnd
    {
        private Random rnd;

        public FloatRnd(int seed)
        {
            rnd = new Random(seed);
        }

        public float Range(float min, float max)
        {
            return min + (float)rnd.NextDouble() * (max - min);
        }
    }
}