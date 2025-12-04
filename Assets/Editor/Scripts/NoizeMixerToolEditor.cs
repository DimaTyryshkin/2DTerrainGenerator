using GamePackages.Core;
using GamePackages.Core.Editor;
using GamePackages.Core.ScriptableObjectEditors.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.NoizeMixerTool
{
    class NoizeMixerToolEditor : ScriptableObjectEditor<NoizeMixerToolModel>
    {
        Vector2 scrollPos;
        TextureDrawer sumTexture;
        TextureDrawer[] textures;

        bool NeedRedraw { get; set; } = true;

        public override void OnGui()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("ReDraw"))
                    DrawTextures();

                //if (GUILayout.Button("To Zero"))
                // offset = Vector2.zero;

            }
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            {
                GUILayout.BeginVertical();
                {
                    if (sumTexture != null)
                        sumTexture.Draw(maxWidth: asset.maxWidthOnGui);

                    GUILayout.Space(6);
                    GUILayout.Label("Octaves");

                    foreach (var texture in textures)
                    {
                        texture.Draw(maxWidth: asset.maxWidthOnGui);
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
                textures = new TextureDrawer[0];

            if (asset.maxWidthOnGui < 32)
            {
                asset.maxWidthOnGui = 32;
                EditorUtility.SetDirty(asset);
            }
        }

        private void DrawTextures()
        {
            Vector2Int offset = Vector2Int.zero;

            MyTexture2D[] textures = asset.DrawOctaves(asset.mapSize.x, asset.mapSize.y, offset);
            MyTexture2D sumTexture = asset.DrawMainTexture(asset.mapSize.x, asset.mapSize.y, offset);
            sumTexture = asset.Normilize(sumTexture);

            if (asset.drawChanks)
                sumTexture = asset.DrawChanks(sumTexture, offset, Color.magenta);

            this.sumTexture = new TextureDrawer(sumTexture.ToTexture());
            this.textures = textures.Select(t => new TextureDrawer(t.ToTexture())).ToArray();
            return;
            sumTexture = asset.SumTextures(textures);

            float clamp = asset.clamp;
            Color clampColor = new Color(clamp, clamp, clamp, 1);

            sumTexture = asset.Clamp(sumTexture, clampColor, Color.white);
            sumTexture = asset.Normilize(sumTexture);
            //sumTexture = asset.DrawPerlinNoize2(
            //    asset.mapSize.x,
            //    asset.mapSize.y,
            //    asset.octaves[0].frequency,
            //    1);
            //sumTexture = asset.DrawPerlinWorms(sumTexture, 0.49f, 0.51f);
            //sumTexture = asset.Normilize(sumTexture);
            sumTexture = asset.ReplaceColor(sumTexture, Color.black, Color.blue);
            if (asset.drawChanks)
                sumTexture = asset.DrawChanks(sumTexture, offset, Color.magenta);

            //sumTexture = asset.ShowWhite(sumTexture);
            //sumTexture = asset.ShowDark(sumTexture);

            this.sumTexture = new TextureDrawer(sumTexture.ToTexture());
            this.textures = textures.Select(t => new TextureDrawer(t.ToTexture())).ToArray();
        }


        void DrawTexture(Texture2D texture)
        {
            Vector2Int size = GUILayoutExtension.GetTextureSize(texture.width, texture.height, asset.maxWidthOnGui, 0);
            Rect rect = GUILayoutUtility.GetRect(0, 0, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUI.DrawTexture(rect, texture);
        }

    }
}
