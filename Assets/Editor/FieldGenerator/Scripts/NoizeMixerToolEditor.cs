using GamePackages.Core.Editor;
using GamePackages.Core.ScriptableObjectEditors.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FieldGenerator
{
    class NoizeMixerToolEditor : ScriptableObjectEditor<NoizeMixerToolModel>
    {
        Vector2 scrollPos;
        TextureDrawer[] textures;

        bool NeedRedraw { get; set; } = true;

        public override void OnGui()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("ReDraw"))
                        DrawTextures();

                    if (GUILayout.Button("Reset Zoom"))
                    {
                        foreach (var t in textures)
                        {
                            t.offset = Vector2.zero;
                            t.scale = 1;
                        }
                    }
                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            {
                GUILayout.BeginVertical();
                {
                    foreach (var texture in textures)
                    {
                        texture.Draw(asset.maxWidthOnGui, asset.maxWidthOnGui, asset.maxWidthOnGui, asset.maxWidthOnGui);
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

            MyTexture2D[] newTextures = asset.DrawTexturesForEditorTool(asset.mapSize.x, asset.mapSize.y, offset);
            for (int i = 0; i < newTextures.Length; i++)
            {
                if (asset.drawChanks)
                    newTextures[i] = asset.DrawChanks(newTextures[i], offset, Color.magenta);
            }

            bool noChangesInTextures = textures != null && newTextures.Length == textures.Length;
            if (noChangesInTextures)
            {
                for (int i = 0; i < textures.Length; i++)
                {
                    textures[i].SetTexture(newTextures[i].ToTexture());
                }
            }
            else
            {
                textures = newTextures.Select(t => new TextureDrawer(t.ToTexture())).ToArray();
            }
            return;
        }
    }
}
