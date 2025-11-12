using System;

namespace Game.GameMath
{
    public class PerlinNoise3D
    {
        private int[] permutation;
        private int[] p;

        public PerlinNoise3D(int seed = 0)
        {
            Random random = new Random(seed);
            permutation = new int[256];

            // Заполняем массив последовательными числами
            for (int i = 0; i < 256; i++)
            {
                permutation[i] = i;
            }

            // Перемешиваем массив
            for (int i = 0; i < 256; i++)
            {
                int j = random.Next(i, 256);
                (permutation[i], permutation[j]) = (permutation[j], permutation[i]);
            }

            // Создаем удвоенный массив для удобства
            p = new int[512];
            for (int i = 0; i < 512; i++)
            {
                p[i] = permutation[i % 256];
            }
        }

        public float Noise(float x, float y, float z)
        {
            // Находим единичный куб, содержащий точку
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;
            int Z = (int)Math.Floor(z) & 255;

            // Находим относительные координаты точки внутри куба
            x -= (float)Math.Floor(x);
            y -= (float)Math.Floor(y);
            z -= (float)Math.Floor(z);

            // Вычисляем кривые веса
            float u = Fade(x);
            float v = Fade(y);
            float w = Fade(z);

            // Смешиваем 8 углов куба
            int A = p[X] + Y;
            int AA = p[A] + Z;
            int AB = p[A + 1] + Z;
            int B = p[X + 1] + Y;
            int BA = p[B] + Z;
            int BB = p[B + 1] + Z;

            return Lerp(w, Lerp(v, Lerp(u, Grad(p[AA], x, y, z),
                                        Grad(p[BA], x - 1, y, z)),
                                Lerp(u, Grad(p[AB], x, y - 1, z),
                                        Grad(p[BB], x - 1, y - 1, z))),
                        Lerp(v, Lerp(u, Grad(p[AA + 1], x, y, z - 1),
                                        Grad(p[BA + 1], x - 1, y, z - 1)),
                                Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1),
                                        Grad(p[BB + 1], x - 1, y - 1, z - 1))));
        }

        private float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        private float Grad(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        // Метод для получения октавного шума (фрактальный шум)
        public float FractalNoise(float x, float y, float z, int octaves = 4, float persistence = 0.5f)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;

            for (int i = 0; i < octaves; i++)
            {
                total += Noise(x * frequency, y * frequency, z * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }
    }
}