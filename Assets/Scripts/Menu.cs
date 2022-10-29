using FieldGenerator;
using UnityEngine;

namespace Terraria
{
    class Menu : MonoBehaviour
    {
        private void OnGUI()
        {
            GUI.matrix = GUI.matrix * Matrix4x4.Scale(Vector3.one * 4);
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Redraw", GUILayout.Width(200)))
                {
                    RedrawAll();
                }
            }
            GUILayout.EndVertical();
        }

        [ContextMenu("RedrawAll")]
        public void RedrawAll()
        {
            var allDrawers = GameObject.FindObjectsOfType<NoiseDrawer>();
            foreach (var drawer in allDrawers)
                drawer.Draw();
            
            Debug.Log("Draw");
        }
    }
}