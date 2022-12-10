using UnityEditor;
using UnityEngine;

namespace MicroMachines.RoadBuilder
{
	[CustomEditor(typeof(RoadBuilderTool))]
	public class RoadBuilderToolEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("Open Editor"))
			{
				var tool = target as RoadBuilderTool;
				RoadBuilderToolWindow window = EditorWindow.GetWindow<RoadBuilderToolWindow>();
				window.Init(tool);
				window.titleContent = new GUIContent(tool.name);
			}
		}
	}
}