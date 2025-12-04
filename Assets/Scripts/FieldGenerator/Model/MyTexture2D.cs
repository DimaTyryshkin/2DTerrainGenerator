using UnityEngine;

namespace Game.NoizeMixerTool
{
    public class MyTexture2D
    {
        public readonly int width;
        public readonly int height;

        Color[] colors;

        public MyTexture2D(int width, int height)
        {
            this.width = width;
            this.height = height;

            colors = new Color[width * height];
        }

        public Color this[int x, int y]
        {
            get { return colors[y * width + x]; }
            set { colors[y * width + x] = value; }
        }

        public Texture2D ToTexture()
        {
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(colors);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            return texture;
        }
    }
}
