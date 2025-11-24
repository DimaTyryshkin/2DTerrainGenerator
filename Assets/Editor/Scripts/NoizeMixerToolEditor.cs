using GamePackages.Core;
using GamePackages.Core.ScriptableObjectEditors.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.NoizeMixerTool
{
    class NoizeMixerToolEditor : ScriptableObjectEditor<NoizeMixerToolModel>
    {
        Vector2 scrollPos;
        Texture2D[] textures;
        Texture2D sumTexture;

        bool NeedRedraw { get; set; } = true;

        public override void OnGui()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("ReDraw"))
                    DrawTextures();

            }
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            {
                GUILayout.BeginVertical();
                {
                    if (sumTexture)
                        DrawTexture(sumTexture);

                    GUILayout.Space(6);
                    GUILayout.Label("Octaves");

                    foreach (var texture in textures)
                    {
                        DrawTexture(texture);
                        GUILayout.Space(6);
                    }


                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }

        public override void Validate()
        {
            if (NeedRedraw)
            {
                NeedRedraw = false;

                EditorUtility.DisplayProgressBar("NoizeMixerTool", "Drawing", 0);
                DrawTextures();
                EditorUtility.ClearProgressBar();
            }

            if (textures == null)
                textures = new Texture2D[0];

            if (asset.maxWidthOnGui < 32)
            {
                asset.maxWidthOnGui = 32;
                EditorUtility.SetDirty(asset);
            }
        }

        private void DrawTextures()
        {
            //textures = asset.DrawOctaves();

            //sumTexture = asset.SumTextures(textures);
            //sumTexture = asset.Normilize(sumTexture);

            float clamp = asset.clamp;
            Color clampColor = new Color(clamp, clamp, clamp, 1);
            //            sumTexture = asset.Normilize(sumTexture);
            //sumTexture = asset.Clamp(sumTexture, clampColor, Color.white);
            //sumTexture = asset.Normilize(sumTexture);
            sumTexture = asset.DrawPerlinNoize2(
                asset.mapSize.x,
                asset.mapSize.y,
                asset.octaves[0].frequency,
                1);
            sumTexture = asset.DrawPerlinWorms(sumTexture, 0.49f, 0.51f);
            sumTexture = asset.Normilize(sumTexture);
            //sumTexture = asset.ReplaceColor(sumTexture, clampColor, Color.magenta);

            //sumTexture = asset.ShowWhite(sumTexture);
            //sumTexture = asset.ShowDark(sumTexture);
        }

        void DrawTexture(Texture2D texture)
        {
            Vector2Int size = GUILayoutExtension.GetTextureSize(texture.width, texture.height, asset.maxWidthOnGui, 0);
            Rect rect = GUILayoutUtility.GetRect(size.x, size.y, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUI.DrawTexture(rect, texture);
        }
    }
}
