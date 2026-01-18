using GamePackages.GamePackagesMath;
using UnityEngine;
using UnityEngine.Assertions;

namespace FieldGenerator
{
    public class UpscaleWithRandom : UpscaleTexture2dOperation
    {
        private readonly FloatRnd rnd;
        private readonly float density;

        int whitePixelAmount;

        public UpscaleWithRandom(FloatRnd rnd, float density)
        {
            Assert.IsNotNull(rnd);

            this.rnd = rnd;
            this.density = density;
        }

        public static MyTexture2D Do(MyTexture2D texture, int seed, float density, int r)
        {
            UpscaleWithRandom operation = new UpscaleWithRandom(new FloatRnd(seed), density);
            return operation.Apply(texture, r);
        }

        protected override void OnWindowStart()
        {
            whitePixelAmount = 0;
        }

        protected override void Kernel(MyTexture2D textureIn, MyTexture2D textureOut, int x, int y, int oldX, int oldY)
        {
            Color oldColor = textureIn[oldX, oldY];
            if (oldColor.maxColorComponent > Mathf.Epsilon)
                whitePixelAmount++;
        }


        protected override void OnWindowEnd(MyTexture2D textureIn, MyTexture2D textureOut, int x, int y, int oldX, int oldY)
        {
            Color oldColor = textureIn[oldX, oldY];

            if (whitePixelAmount < pixelsInWindowAmount && whitePixelAmount > 0)
            {
                textureOut[x, y] = rnd.Range(0f, 1f) < density ? oldColor : Color.black;
            }
            else
            {
                textureOut[x, y] = oldColor;
            }
        }
    }

    public class SmothBorders : UpscaleTexture2dOperation
    {
        int whitePixelAmount;
        readonly float factor;

        protected override void OnWindowStart()
        {
            whitePixelAmount = 0;
        }


        public SmothBorders(float factor = 0.5f)
        {
            this.factor = factor;
        }

        public static MyTexture2D Do(MyTexture2D texture, int r, float factor = 0.5f)
        {
            SmothBorders operation = new SmothBorders(factor);
            return operation.Apply(texture, r);
        }

        protected override void Kernel(MyTexture2D textureIn, MyTexture2D textureOut, int x, int y, int oldX, int oldY)
        {
            Color oldColor = textureIn[oldX, oldY];
            if (oldColor.maxColorComponent > Mathf.Epsilon)
                whitePixelAmount++;
        }


        protected override void OnWindowEnd(MyTexture2D textureIn, MyTexture2D textureOut, int x, int y, int oldX, int oldY)
        {
            Color oldColor = textureIn[oldX, oldY];

            if (oldColor.maxColorComponent > Mathf.Epsilon)
            {
                textureOut[x, y] = oldColor;
            }
            else
            {
                if (whitePixelAmount > pixelsInWindowAmount * factor)
                    textureOut[x, y] = Color.white;
                else
                    textureOut[x, y] = Color.black;
            }
        }

    }
}
