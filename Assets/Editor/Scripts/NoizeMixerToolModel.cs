using System;
using GamePackages.Core;
using GamePackages.Core.ScriptableObjectEditors;
using NUnit.Framework;
using UnityEngine;

namespace Game.NoizeMixerTool
{
    class NoizeMixerToolModel : EntityRoot
    {
        public int maxWidthOnGui;
        public float clamp = 0.5f;
        public float fr = 1;
        public float am = 1;
        public Vector2Int mapSize;
        public Octave[] octaves;

        public Texture2D[] DrawOctaves()
        {
            Texture2D[] textures = new Texture2D[octaves.Length];

            for (int i = 0; i < octaves.Length; i++)
                textures[i] = DrawOctave(octaves[i]);

            return textures;
        }

        public Texture2D DrawOctave(Octave octave)
        {
            return DrawPerlinNoize(
                      mapSize.x,
                      mapSize.y,
                      octave.frequency,
                      octave.amplitude);
        }

        public Texture2D DrawPerlinNoize(int w, int h, float frequency, float amplitude)
        {
            Texture2D texture = new Texture2D(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float value = amplitude * Mathf.PerlinNoise(x * frequency, y * frequency);
                    Color color = new Color(value, value, value);

                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        public Texture2D DrawPerlinNoize2(int w, int h, float frequency, float amplitude)
        {
            float offsetX = 100000;
            float offsetY = 50000;
            Texture2D texture = new Texture2D(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float divX = am * Mathf.PerlinNoise((x + offsetX) * fr, (y + offsetX) * fr);
                    float divY = am * Mathf.PerlinNoise((x + offsetY) * fr, (y + offsetY) * fr);
                    float value = amplitude * Mathf.PerlinNoise((x + divX) * frequency, (y + divY) * frequency);
                    Color color = new Color(value, value, value);

                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        public Texture2D DrawPerlinWorms(Texture2D texture, float minValue, float maxValue)
        {
            Color min = ColorExtension.Create(minValue);
            Color max = ColorExtension.Create(maxValue);
            Color black = Color.black;
            return Operation(texture, c =>
            {
                if (min.GreaterThen(c))
                    return black;

                if (c.GreaterThen(max))
                    return black;

                return c;
            });
        }

        public Texture2D Clamp(Texture2D texture, Color min, Color max)
        {
            return Operation(texture, c => ColorExtension.Clamp(c, min, max));
        }

        public Texture2D Negative(Texture2D texture)
        {
            return Operation(texture, ColorExtension.Negative);
        }

        public Texture2D ReplaceColor(Texture2D texture, Color replace, Color to)
        {
            return Operation(texture, c => ColorExtension.CompareRGBApproximately(c, replace) ? to : c);
        }

        public Texture2D ShowWhite(Texture2D texture) => ReplaceColor(texture, Color.white, Color.magenta);
        public Texture2D ShowDark(Texture2D texture) => ReplaceColor(texture, Color.black, Color.magenta);


        public Texture2D Operation(Texture2D texture, Func<Color, Color> func)
        {
            int width = texture.width;
            int height = texture.height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color c = texture.GetPixel(x, y);
                    c = func(c);
                    texture.SetPixel(x, y, c);
                }
            }

            texture.Apply();
            return texture;
        }

        public Texture2D Normilize(Texture2D texture, float minValue = 0, float maxValue = 1)
        {
            int width = texture.width;
            int height = texture.height;

            float max = 0;
            float min = 1;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = texture.GetPixel(x, y);
                    max = max < color.r ? color.r : max;
                    max = max < color.g ? color.g : max;
                    max = max < color.b ? color.b : max;

                    min = min > color.r ? color.r : min;
                    min = min > color.g ? color.g : min;
                    min = min > color.b ? color.b : min;

                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color c = texture.GetPixel(x, y);
                    float rNew = MathExtension.MapUnclamped(min, max, minValue, maxValue, c.r);
                    float gnew = MathExtension.MapUnclamped(min, max, minValue, maxValue, c.g);
                    float bNew = MathExtension.MapUnclamped(min, max, minValue, maxValue, c.b);
                    float aNew = MathExtension.MapUnclamped(min, max, minValue, maxValue, c.a);
                    c = new Color(rNew, gnew, bNew, aNew);
                    texture.SetPixel(x, y, c);
                }
            }
            texture.Apply();
            return texture;
        }

        public Texture2D SumTextures(Texture2D[] textures, bool withNormalization = true)
        {
            int width = textures[0].width;
            int height = textures[0].height;
            Color[,] temp = new Color[width, height];

            for (int i = 0; i < textures.Length; i++)
            {
                Texture2D t = textures[i];
                Assert.AreEqual(width, t.width);
                Assert.AreEqual(height, t.height);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color c = t.GetPixel(x, y);
                        temp[x, y] += c;
                    }
                }
            }

            float maxValue = 1;
            float minValue = 0;
            if (withNormalization)
            {
                maxValue = 0;
                minValue = 1;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color color = temp[x, y];
                        maxValue = maxValue < color.r ? color.r : maxValue;
                        maxValue = maxValue < color.g ? color.g : maxValue;
                        maxValue = maxValue < color.b ? color.b : maxValue;

                        minValue = minValue > color.r ? color.r : minValue;
                        minValue = minValue > color.g ? color.g : minValue;
                        minValue = minValue > color.b ? color.b : minValue;
                    }
                }
            }

            Texture2D texture = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color c = temp[x, y];
                    float rNew = MathExtension.MapUnclamped(minValue, maxValue, 0, 1, c.r);
                    float gnew = MathExtension.MapUnclamped(minValue, maxValue, 0, 1, c.g);
                    float bNew = MathExtension.MapUnclamped(minValue, maxValue, 0, 1, c.b);
                    float aNew = MathExtension.MapUnclamped(minValue, maxValue, 0, 1, c.a);
                    c = new Color(rNew, gnew, bNew, aNew);
                    texture.SetPixel(x, y, c);
                }
            }
            texture.Apply();
            return texture;
        }
    }

    [Serializable]
    struct Octave
    {
        public float frequency;
        public float amplitude;
    }
}
