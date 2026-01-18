using GamePackages.Core;
using GamePackages.Core.ScriptableObjectEditors;
using GamePackages.GamePackagesMath;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace FieldGenerator
{
    public class NoizeMixerToolModel : EntityRoot
    {
        public Vector2Int mapSize = new Vector2Int(512, 512);
        public int maxWidthOnGui = 1024;
        public bool drawChanks;
        [SerializeField] Texture2D tenperatureAndHumidity;
        [SerializeField] Gradient gradient;
        [SerializeField] Gradient gradient2;

        [Header("MainTexture")]
        public int seed;
        [Range(0, 1)] public float density;
        [Range(0, 1)] public float smofeFactor;
        [Range(1, 10)] public int smofeRange;
        public bool drawWorms;
        public bool isWhiteWorms;
        public RangeMinMax wormsRange = new RangeMinMax(0.4f, 0.6f);
        public int octav = 1;
        public float clamp = 0.5f;
        public float fr = 1;
        public float am = 1;
        //public Vector2Int offset;


        public Octave[] octaves;


        public MyTexture2D[] DrawTexturesForEditorTool(int w, int h, Vector2Int position2D)
        {
            return new MyTexture2D[]
            {
               ContinentMap512(),
               UpscaleStart(Color.white, seed, density, 2, 128),

               UpscaleStart(Color.white, seed, density, 2, 512),
               UpscaleStart(Color.white, seed, density, 2, 256),
               UpscaleStart(Color.white, seed, density, 2, 128),
               UpscaleStart(Color.white, seed, density, 2, 64),
               UpscaleStart(Color.white, seed, density, 2, 32),
               UpscaleStart(Color.white, seed, density, 2, 8),
               UpscaleStart(Color.white, seed, density, 2, 4),
               UpscaleStart(Color.white, seed, density, 2, 2)

               //Normilize( DrawGameTexture(w, h, position2D)), 
               //Normilize(DrawMix3Noize(w, h, position2D,   frequency: 0.006f,amplitude: 1,octav)),
               //DrawTemperatureAndHumidity(w, h,tenperatureAndHumidity,position2D,frequency: fr,amplitude: 1,octav),
               //DrawHeight(w, h, gradient, position2D,frequency: fr,amplitude: 1,octav),
               //ApplyColorGradient(Normilize(DrawPerlinNoize(w, h, position2D, fr,1,octav)), gradient2) 
               //ApplyColorGradient(DrawPerlinNoize(w, h, position2D, fr,1,octav), gradient2) 
               //DrawWorms(w, h, position2D),
               //DrawMix3Noize(w,h,position2D, fr,1,octav),
               //Draw3Bioms(w,h,position2D)
            };
        }


        public MyTexture2D GenerateWorldMap()
        {
            return ContinentMap512();
        }

        MyTexture2D ContinentMap512()
        {
            return SmothBorders.Do(
                    SmothBorders.Do(
                        UpscaleStart(Color.white, seed, density, 2, 128),
                        smofeRange, smofeFactor),
                    smofeRange, smofeFactor);
        }

        public MyTexture2D DrawGameTexture(int w, int h, Vector2Int position2D)
        {
            return Draw3Bioms(w, h, position2D);
        }

        public MyTexture2D DrawWorms(int w, int h, Vector2Int position2D)
        {
            MyTexture2D perlin = DrawPerlinNoize(w, h, position2D, fr, am, octav);

            if (drawWorms)
                perlin = DrawPerlinWorms(perlin, isWhiteWorms, wormsRange);

            return perlin;
        }



        public MyTexture2D Draw3Bioms(int w, int h, Vector2Int position2D)
        {
            return DrawMix3Noize(w, h, position2D,
                frequency: 0.006f,
                amplitude: 1,
                octaves: 5);
        }

        MyTexture2D DrawMix3Noize(int w, int h, Vector2Int position2D, float frequency, float amplitude, int octaves = 1)
        {
            MyTexture2D texture = new MyTexture2D(w, h);

            float offset1 = 0;
            float offset2 = 8000;
            float offset3 = 4000;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float value1 = FractalNoise2D((x + offset1 + position2D.x), (y + position2D.y), frequency, amplitude, octaves);
                    float value2 = FractalNoise2D((x + offset2 + position2D.x), (y + position2D.y), frequency, amplitude, octaves);
                    float value3 = FractalNoise2D((x + offset3 + position2D.x), (y + position2D.y), frequency, amplitude, octaves);

                    Color color;

                    if (value1 > value2 && value1 > value3)
                    {
                        value1 = value1 - Mathf.Max(value2, value3);
                        color = new Color(value1, 0, 0);
                    }
                    else if (value2 > value1 && value2 > value3)
                    {
                        value2 = value2 - Mathf.Max(value1, value3);
                        color = new Color(0, value2, 0);
                    }
                    else
                    {
                        value3 = value3 - Mathf.Max(value1, value2);
                        color = new Color(0, 0, value3);
                    }

                    texture[x, y] = color;
                }
            }

            return texture;
        }

        MyTexture2D DrawTemperatureAndHumidity(int w, int h, Texture2D temperatureAndHumidity, Vector2Int position2D, float frequency, float amplitude, int octaves = 1)
        {
            MyTexture2D texture = new MyTexture2D(w, h);

            float offset1 = 0;
            float offset2 = (512 / 3) * 15;

            int width = temperatureAndHumidity.width;
            int height = temperatureAndHumidity.height;

            //Dictionary<int, int> stat = new Dictionary<int, int>();

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float value1 = FractalNoise2D((x + offset1 + position2D.x), (y + position2D.y), frequency, amplitude, octaves);
                    float value2 = FractalNoise2D((x + offset2 + position2D.x), (y + position2D.y), frequency, amplitude, octaves);

                    //int value = (int)Mathf.Lerp(0, 100, value1);
                    //stat[value] = 1 + (stat.ContainsKey(value) ? stat[value] : 0);

                    int tempetature = Mathf.Clamp((int)(value1 * width), 0, width);
                    int humidity = Mathf.Clamp((int)(value2 * height), 0, height);

                    texture[x, y] = temperatureAndHumidity.GetPixel(tempetature, humidity);

                }
            }

            //Debug.Log(stat.ToArray().OrderBy(x => x.Key).ToStringMultiline("Stat", pair => $"[{pair.Key:00}] = {pair.Value}"));
            return texture;
        }

        MyTexture2D DrawHeight(int w, int h, Gradient gradient, Vector2Int position2D, float frequency, float amplitude, int octaves = 1)
        {
            MyTexture2D texture = new MyTexture2D(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float value = FractalNoise2D((x + position2D.x), (y + position2D.y), frequency, amplitude, octaves);
                    texture[x, y] = gradient.Evaluate(value);

                }
            }
            return texture;
        }


        MyTexture2D ApplyColorGradient(MyTexture2D texture2D, Gradient gradient)
        {
            Assert.IsNotNull(texture2D);
            Assert.IsNotNull(gradient);
            return Operation(texture2D, color => gradient.Evaluate(color.maxColorComponent));
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
                position2D,
                octave.frequency,
                octave.amplitude);
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

        public MyTexture2D DrawPerlinNoize(int w, int h, Vector2Int position2D, float frequency, float amplitude, int octaves = 1)
        {
            MyTexture2D texture = new MyTexture2D(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float value = FractalNoise2D((x + position2D.x), (y + position2D.y), frequency, amplitude, octaves);
                    Color color = new Color(value, value, value);

                    texture[x, y] = color;
                }
            }

            return texture;
        }


        public MyTexture2D UpscaleStart(Color color, int seed, float density, int startSize, int endSize)
        {
            Assert.IsTrue(Mathf.IsPowerOfTwo(startSize));
            Assert.IsTrue(Mathf.IsPowerOfTwo(endSize));

            FloatRnd rnd = new FloatRnd(seed);


            MyTexture2D texture = new MyTexture2D(startSize, startSize);
            for (int x = 0; x < startSize; x++)
            {
                for (int y = 0; y < startSize; y++)
                {
                    texture[x, y] = rnd.Range(0.0f, 1.0f) <= 0.5f ? color : Color.black;
                }
            }

            int n = 0;

            UpscaleWithRandom operation = new UpscaleWithRandom(rnd, density);

            while (texture.width < endSize)
            {
                n++;
                Assert.IsTrue(n < 100);
                texture = operation.Apply(texture, 1);
                //texture = Upscale(texture, rnd, density);

            }

            return texture;
        }

        public MyTexture2D Upscale(MyTexture2D textureIn, FloatRnd rnd, float density)
        {
            int w = textureIn.width * 2;
            int h = textureIn.height * 2;

            MyTexture2D texture = new MyTexture2D(w, h);
            Vector2Int[] nears = new Vector2Int[5];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    nears[0] = new Vector2Int(x + 0, y + 0);
                    nears[1] = new Vector2Int(x + 0, y + 1);
                    nears[2] = new Vector2Int(x + 1, y + 0);
                    nears[3] = new Vector2Int(x + 0, y - 1);
                    nears[4] = new Vector2Int(x - 1, y + 0);

                    int amount = 0;
                    int sum = 0;
                    for (int i = 0; i < nears.Length; i++)
                    {
                        Vector2Int c = nears[i];

                        if (c.x < 0 || c.y < 0 || c.x >= w || c.y >= h)
                            continue;

                        amount++;
                        int oldX = c.x / 2;
                        int oldY = c.y / 2;
                        ;
                        Color oldColor = textureIn[oldX, oldY];
                        if (oldColor.maxColorComponent > Mathf.Epsilon)
                            sum++;
                    }

                    int oldX2 = x / 2;
                    int oldY2 = y / 2;
                    Color oldColor2 = textureIn[oldX2, oldY2];

                    if (sum < amount && sum > 0)
                    {
                        texture[x, y] = rnd.Range(0f, 1f) < density ? oldColor2 : Color.black;
                    }
                    else
                    {

                        texture[x, y] = oldColor2;
                    }
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

        public MyTexture2D DrawPerlinWorms(MyTexture2D texture, bool isWhiteTop, RangeMinMax range)
            => DrawPerlinWorms(texture, isWhiteTop, range.min, range.max);

        public MyTexture2D DrawPerlinWorms(MyTexture2D texture, bool isWhiteTop, float minValue, float maxValue)
        {
            Color min = ColorExtension.Create(minValue);
            Color max = ColorExtension.Create(maxValue);
            Color black = Color.black;
            Color white = Color.white;
            return Operation(texture, c =>
            {
                if (min.GreaterThen(c))
                    return black;

                if (c.GreaterThen(max))
                    return isWhiteTop ? white : black;

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


        public MyTexture2D SumTextures(MyTexture2D t1, MyTexture2D t2, bool withNormalization = true)
            => SumTextures(new MyTexture2D[] { t1, t2 }, withNormalization);

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


        public float FractalNoise2D(
            float x,
            float y,
            float frequency = 1,
            float amplitude = 1,
            int octaves = 4,
            float persistence = 0.5f)
        {
            float total = 0;
            float maxValue = 0;

            for (int i = 0; i < octaves; i++)
            {
                total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }
    }

    [Serializable]
    public struct Octave
    {
        public float frequency;
        public float amplitude;
    }
}
