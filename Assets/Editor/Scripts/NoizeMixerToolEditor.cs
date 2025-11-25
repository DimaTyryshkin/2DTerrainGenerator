using System.Linq;
using GamePackages.Core;
using GamePackages.Core.ScriptableObjectEditors.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.NoizeMixerTool
{
    class NoizeMixerToolEditor : ScriptableObjectEditor<NoizeMixerToolModel>
    {
        Vector2 worldPos;

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
                        DrawWorld(sumTexture);

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

            if (textures == null || textures.Any(x => !x))
                textures = new Texture2D[0];

            if (asset.maxWidthOnGui < 32)
            {
                asset.maxWidthOnGui = 32;
                EditorUtility.SetDirty(asset);
            }
        }

        private void DrawTextures()
        {
            textures = asset.DrawOctaves();
            sumTexture = asset.SumTextures(textures);
            //sumTexture = asset.Normilize(sumTexture);

            float clamp = asset.clamp;
            Color clampColor = new Color(clamp, clamp, clamp, 1);
            //sumTexture = asset.Normilize(sumTexture);
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
                sumTexture = asset.DrawChanks(sumTexture, asset.offset, 16, Color.magenta);

            //sumTexture = asset.ShowWhite(sumTexture);
            //sumTexture = asset.ShowDark(sumTexture);
        }


        int scale = 1;
        bool isDrag;
        int maxScale = 16;
        Vector2 offset;
        Rect texCoords;
        Rect viewRect;
        Texture2D texture;
        void CalculateTexCoords()
        {
            float textCoordsSize = 1f / scale;
            texCoords = new Rect(offset.x, offset.y, textCoordsSize, textCoordsSize);
        }

        private void DrawWorld(Texture2D texture)
        {
            GUILayout.BeginVertical();
            {
                //DrawTexture(texture);

                this.texture = texture;
                Vector2Int size = GUILayoutExtension.GetTextureSize(texture.width, texture.height, asset.maxWidthOnGui, 0);
                GUILayoutOption width = GUILayout.Width(size.x);
                viewRect = GUILayoutUtility.GetRect(0, 0, width, GUILayout.Height(size.y));

                CalculateTexCoords();
                GUI.DrawTextureWithTexCoords(viewRect, texture, texCoords);

                Vector2 mousePos = Event.current.mousePosition;
                if (viewRect.Contains(mousePos))
                {
                    bool isClick = false;
                    if (Event.current.type == EventType.MouseDown)
                    {
                        isDrag = false;
                        Event.current.Use();
                    }

                    if (Event.current.type == EventType.MouseDrag)
                    {
                        isDrag = true;
                        Vector2 delta = Event.current.delta;
                        delta.x *= -1f / (texture.width * scale);
                        delta.y *= 1f / (texture.height * scale);
                        offset += delta;

                        Event.current.Use();
                    }

                    if (Event.current.type == EventType.MouseUp)
                    {
                        if (!isDrag)
                        {
                            isClick = true;
                        }

                        isDrag = false;
                        Event.current.Use();
                    }

                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        float scroll = Mathf.Sign(Event.current.delta.y);
                        if (!Mathf.Approximately(scroll, 0))
                        {
                            Vector2 mouseNormilizedPosOld = ScreenPointToNormilizedTextCoord(mousePos);
                            scale = Mathf.Clamp(scale - (int)scroll, 1, maxScale);
                            CalculateTexCoords();
                            Vector2 mouseNormilizedPosNew = ScreenPointToNormilizedTextCoord(mousePos);

                            Vector2 delta = mouseNormilizedPosOld - mouseNormilizedPosNew;
                            offset += delta;

                            Event.current.Use();
                        }
                    }

                    if (isClick)
                    {
                        Vector2Int mouseTextCoord = ScreenPointToTextCoord(mousePos);
                        texture.SetPixel(mouseTextCoord.x, mouseTextCoord.y, Color.magenta);
                        texture.Apply();
                    }
                }


                GUILayout.BeginHorizontal();
                {
                    scale = EditorGUILayout.IntSlider(scale, 1, maxScale, width);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        Vector2 ScreenPointToNormilizedTextCoord(Vector2 point)
        {
            Vector2 normPos = Rect.PointToNormalized(viewRect, point);
            Vector2 add = new Vector2(normPos.x * texCoords.width, (1f - normPos.y) * texCoords.width);
            Vector2 pointNormilizedTextCoord = offset + add;
            return pointNormilizedTextCoord;
        }

        Vector2Int ScreenPointToTextCoord(Vector2 point)
        {
            Vector2 pointNormilizedTextCoord = ScreenPointToNormilizedTextCoord(point);

            Vector2Int pointTextCoord = new Vector2Int(
                Mathf.RoundToInt(texture.width * pointNormilizedTextCoord.x - 0.5f),
                Mathf.RoundToInt(texture.height * pointNormilizedTextCoord.y - 0.5f));

            return pointTextCoord;
        }

        void DrawTexture(Texture2D texture)
        {
            Vector2Int size = GUILayoutExtension.GetTextureSize(texture.width, texture.height, asset.maxWidthOnGui, 0);
            Rect rect = GUILayoutUtility.GetRect(0, 0, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUI.DrawTexture(rect, texture);
        }
    }
}
