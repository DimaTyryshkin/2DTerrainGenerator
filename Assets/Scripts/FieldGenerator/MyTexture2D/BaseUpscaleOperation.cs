namespace FieldGenerator
{
    public abstract class UpscaleTexture2dOperation //SlidingWindowOperation
    {
        protected int pixelsInWindowAmount;
        private int rPower2;
        private int w;
        private int h;

        public MyTexture2D Apply(MyTexture2D textureIn, int r)
        {
            rPower2 = r * r;
            w = textureIn.width * 2;
            h = textureIn.height * 2;

            MyTexture2D textureOut = new MyTexture2D(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    OnWindowStart();
                    ApplyWondow(textureIn, textureOut, x, y, r);
                }
            }

            return textureOut;
        }

        private void ApplyWondow(MyTexture2D textureIn, MyTexture2D textureOut, int centerX, int centerY, int r)
        {
            pixelsInWindowAmount = 0;
            OnWindowStart();

            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    if ((dx * dx + dy * dy) > rPower2)
                        continue;

                    int x = dx + centerX;
                    int y = dy + centerY;

                    if (x < 0 || y < 0 || x >= w || y >= h)
                        continue;

                    int oldX = x / 2;
                    int oldY = y / 2;

                    pixelsInWindowAmount++;
                    Kernel(textureIn, textureOut, x, y, oldX, oldY);
                }
            }

            int oldX2 = centerX / 2;
            int oldY2 = centerY / 2;

            OnWindowEnd(textureIn, textureOut, centerX, centerY, oldX2, oldY2);
        }

        protected abstract void OnWindowStart();
        protected abstract void OnWindowEnd(MyTexture2D textureIn, MyTexture2D textureOut, int x, int y, int oldX, int oldY);
        protected abstract void Kernel(MyTexture2D textureIn, MyTexture2D textureOut, int x, int y, int oldX, int oldY);
    }
}
