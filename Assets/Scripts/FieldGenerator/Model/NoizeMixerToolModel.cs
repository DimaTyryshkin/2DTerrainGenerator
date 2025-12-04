using FieldGenerator;
using GamePackages.Core;
using GamePackages.Core.ScriptableObjectEditors;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.NoizeMixerTool
{
    public class NoizeMixerToolModel : EntityRoot
    {
        public int maxWidthOnGui;
        public float clamp = 0.5f;
        public float fr = 1;
        public float am = 1;
        //public Vector2Int offset;
        public Vector2Int mapSize;
        public bool drawChanks;
        public Octave[] octaves;

        public MyTexture2D DrawMainTexture(int w, int h, Vector2Int position2D)
        {
            //return DrawFlatTexture(w, h, 0.01f);

            MyTexture2D[] textures = DrawOctaves(w, h, position2D);
            MyTexture2D sumTexture = SumTextures(textures, withNormalization: false);

            return sumTexture;

            Color clampColor = new Color(clamp, clamp, clamp, 1);

            sumTexture = Clamp(sumTexture, clampColor, Color.white);
            sumTexture = Normilize(sumTexture);

            sumTexture = ReplaceColor(sumTexture, Color.black, Color.blue);

            //sumTexture = asset.ShowWhite(sumTexture);
            //sumTexture = asset.ShowDark(sumTexture);

            return sumTexture;
        }

        public MyTexture2D[] DrawOctaves(int w, int h, Vector2Int position2D)
        {
            MyTexture2D[] textures = new MyTexture2D[octaves.Length];

            for (int i = 0; i < octaves.Length; i++)
                textures[i] = DrawOctave(octaves[i], w, h, position2D);

            return textures;
        }

        public MyTexture2D DrawOctave(Octave octave, int w, int h, Vector2Int position2D)
        {
            return DrawPerlinNoize(
                      w,
                      h,
                      octave.frequency,
                      octave.amplitude,
                      position2D);
        }

        public MyTexture2D DrawChanks(MyTexture2D texture, Vector2Int position2D, Color color)
        {
            int width = texture.width;
            int height = texture.height;

            //int startX = Chunk.chunkSize - position2D.x % Chunk.chunkSize;
            //int startY = Chunk.chunkSize - position2D.y % Chunk.chunkSize;

            int startX = position2D.x % Chunk.chunkSize;
            int startY = position2D.y % Chunk.chunkSize;

            // vertical lines
            for (int x = startX; x < width; x += Chunk.chunkSize)
            {
                for (int y = 0; y < height; y++)
                    texture[x, y] = color;
            }

            // horizont lines
            for (int y = startY; y < height; y += Chunk.chunkSize)
            {
                for (int x = 0; x < width; x++)
                    texture[x, y] = color;
            }

            return texture;
        }

        public MyTexture2D DrawPerlinNoize(int w, int h, float frequency, float amplitude, Vector2Int position2D)
        {
            MyTexture2D texture = new MyTexture2D(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float value = amplitude * Mathf.PerlinNoise((x + position2D.x) * frequency, (y + position2D.y) * frequency);
                    Color color = new Color(value, value, value);

                    texture[x, y] = color;
                }
            }

            return texture;
        }


        public MyTexture2D DrawFlatTexture(int w, int h, float value)
        {
            MyTexture2D texture = new MyTexture2D(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color color = new Color(value, value, value);
                    texture[x, y] = color;
                }
            }

            return texture;
        }

        public MyTexture2D DrawPerlinNoize2(int w, int h, float frequency, float amplitude)
        {
            float offsetX = 100000;
            float offsetY = 50000;
            MyTexture2D texture = new MyTexture2D(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float divX = am * Mathf.PerlinNoise((x + offsetX) * fr, (y + offsetX) * fr); //TODO position2D
                    float divY = am * Mathf.PerlinNoise((x + offsetY) * fr, (y + offsetY) * fr);
                    float value = amplitude * Mathf.PerlinNoise((x + divX) * frequency, (y + divY) * frequency);
                    Color color = new Color(value, value, value);

                    texture[x, y] = color;
                }
            }

            return texture;
        }

        public MyTexture2D DrawPerlinWorms(MyTexture2D texture, float minValue, float maxValue)
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

        public MyTexture2D Clamp(MyTexture2D texture, Color min, Color max)
        {
            return Operation(texture, c => ColorExtension.Clamp(c, min, max));
        }

        public MyTexture2D Negative(MyTexture2D texture)
        {
            return Operation(texture, ColorExtension.Negative);
        }

        public MyTexture2D ReplaceColor(MyTexture2D texture, Color replace, Color to)
        {
            return Operation(texture, c => ColorExtension.CompareRGBApproximately(c, replace) ? to : c);
        }

        public MyTexture2D ShowWhite(MyTexture2D texture) => ReplaceColor(texture, Color.white, Color.magenta);

        public MyTexture2D ShowDark(MyTexture2D texture) => ReplaceColor(texture, Color.black, Color.magenta);

        public MyTexture2D Operation(MyTexture2D texture, Func<Color, Color> func)
        {
            int width = texture.width;
            int height = texture.height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color c = texture[x, y];
                    c = func(c);
                    texture[x, y] = c;
                }
            }

            return texture;
        }

        public MyTexture2D Normilize(MyTexture2D texture, float minValue = 0, float maxValue = 1)
        {
            int width = texture.width;
            int height = texture.height;

            float max = 0;
            float min = 1;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = texture[x, y];
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
                    Color c = texture[x, y];
                    float rNew = MathExtension.MapUnclamped(min, max, minValue, maxValue, c.r);
                    float gnew = MathExtension.MapUnclamped(min, max, minValue, maxValue, c.g);
                    float bNew = MathExtension.MapUnclamped(min, max, minValue, maxValue, c.b);
                    float aNew = MathExtension.MapUnclamped(min, max, minValue, maxValue, c.a);
                    c = new Color(rNew, gnew, bNew, aNew);
                    texture[x, y] = c;
                }
            }

            return texture;
        }

        public MyTexture2D SumTextures(MyTexture2D[] textures, bool withNormalization = true)
        {
            int width = textures[0].width;
            int height = textures[0].height;
            MyTexture2D temp = new MyTexture2D(width, height);

            for (int i = 0; i < textures.Length; i++)
            {
                MyTexture2D t = textures[i];
                Assert.AreEqual(width, t.width);
                Assert.AreEqual(height, t.height);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color c = t[x, y];
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

            MyTexture2D texture = new MyTexture2D(width, height);
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
                    texture[x, y] = c;
                }
            }

            return texture;
        }
    }

    [Serializable]
    public struct Octave
    {
        public float frequency;
        public float amplitude;
    }
}
