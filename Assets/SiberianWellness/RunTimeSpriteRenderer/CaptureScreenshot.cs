using System.IO;
using UnityEngine;

namespace SiberianWellness.RunTimeSpriteRenderer
{ 
    public static class CaptureScreenshot
    {
        public static void RenderTextureToFile(Camera cam, int width, int height, string screengrabfile_path)
        {
            var txt = RenderTexture(cam, width, height);
            byte[] pngShot = ImageConversion.EncodeToPNG(txt);
            File.WriteAllBytes(screengrabfile_path, pngShot);
            
            Destroy(txt);
        }
 
        public static void RenderSimpleTextureToFile(Camera cam, int width, int height, string screengrabfile_path)
        {
            var txt = RenderSimpleTexture(cam, width, height);
            byte[] pngShot = ImageConversion.EncodeToPNG(txt);
            File.WriteAllBytes(screengrabfile_path, pngShot);
 
            Destroy(txt); 
        }
        
        static Texture2D RenderTexture(Camera cam, int width, int height)
        {
            // This is slower, but seems more reliable.
            var bak_cam_targetTexture    = cam.targetTexture;
            var bak_cam_clearFlags       = cam.clearFlags;
            var bak_cam_backgroundColor  = cam.backgroundColor;
            var bak_RenderTexture_active = UnityEngine.RenderTexture.active;

            var tex_white       = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var tex_black       = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
            // Must use 24-bit depth buffer to be able to fill background.
            var render_texture = UnityEngine.RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            var grab_area      = new Rect(0, 0, width, height);

            UnityEngine.RenderTexture.active = render_texture;
            cam.targetTexture    = render_texture;
            cam.clearFlags       = CameraClearFlags.SolidColor;

            cam.backgroundColor = Color.black;
            cam.Render();
            tex_black.ReadPixels(grab_area, 0, 0);
            tex_black.Apply();

            cam.backgroundColor = Color.white;
            cam.Render();
            tex_white.ReadPixels(grab_area, 0, 0);
            tex_white.Apply();

            // Create Alpha from the difference between black and white camera renders
            for (int y = 0; y < tex_transparent.height; ++y)
            {
                for (int x = 0; x < tex_transparent.width; ++x)
                {
                    float alpha = tex_white.GetPixel(x, y).r - tex_black.GetPixel(x, y).r;
                    alpha = 1.0f - alpha;
                    Color color;
                    if (alpha == 0)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = tex_black.GetPixel(x, y) / alpha;
                    }

                    color.a = alpha;
                    tex_transparent.SetPixel(x, y, color);
                }
            }
 

            cam.clearFlags       = bak_cam_clearFlags;
            cam.targetTexture    = bak_cam_targetTexture;
            cam.backgroundColor  = bak_cam_backgroundColor;
            UnityEngine.RenderTexture.active = bak_RenderTexture_active;
            UnityEngine.RenderTexture.ReleaseTemporary(render_texture);

            Destroy(tex_black);
            Destroy(tex_white);

            return tex_transparent;
        }

        public static Sprite RenderSimpleSprite(Camera cam, int width, int height)
        {
            var txt = RenderSimpleTexture(cam, width, height);
            var sprite = Sprite.Create(txt, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

            return sprite;
        }

        static Texture2D RenderSimpleTexture(Camera cam, int width, int height)
        {
            // Depending on your render pipeline, this may not work.
            var bak_cam_targetTexture    = cam.targetTexture;
            var bak_cam_clearFlags       = cam.clearFlags;
            var bak_RenderTexture_active = UnityEngine.RenderTexture.active;

            var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
            // Must use 24-bit depth buffer to be able to fill background.
            var render_texture = UnityEngine.RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            var grab_area      = new Rect(0, 0, width, height);

            UnityEngine.RenderTexture.active = render_texture;
            cam.targetTexture    = render_texture;
            cam.clearFlags       = CameraClearFlags.SolidColor;

            // Simple: use a clear background
            cam.backgroundColor = Color.clear;
            cam.Render();
            tex_transparent.ReadPixels(grab_area, 0, 0);
            tex_transparent.Apply();
 

            cam.clearFlags       = bak_cam_clearFlags;
            cam.targetTexture    = bak_cam_targetTexture;
            UnityEngine.RenderTexture.active = bak_RenderTexture_active;
            UnityEngine.RenderTexture.ReleaseTemporary(render_texture);

            tex_transparent.filterMode = FilterMode.Point;
            return tex_transparent;
        }

        static void Destroy(Object obj)
        {
            Texture2D.DestroyImmediate(obj);
        }
    }
}
