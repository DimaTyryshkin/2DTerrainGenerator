using System;
using System.Linq;
using SiberianWellness.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace MicroMachines.RoadBuilder
{
	public class RoadBuilderToolWindow : EditorWindow
	{
		RoadBuilderTool tool;


		public void Init(RoadBuilderTool tool)
		{
			Assert.IsNotNull(tool);
			this.tool = tool;
		}


		void OnEnable()
		{
			SceneView.duringSceneGui += OnDuringSceneGui;
		}

		void OnDisable()
		{
			SceneView.duringSceneGui -= OnDuringSceneGui;
		}
 
		void OnGUI()
		{
			if (!tool)
				return;

			GUI.color = Color.white;
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical("Box", GUILayout.Width(150));
				{
					GUILayout.Box("Exist");
					foreach (RoadSegment segment in tool.SegmentsInRoad.ToArray())
					{
						GUILayout.BeginHorizontal();
						{
							if(segment == tool.SelectedSegment)
								GUI.color = Color.gray;
							
							if (GUILayout.Button(segment.SegmentName))
							{
								tool.SelectedSegment = segment;
								SelectObject( segment.gameObject);
							}
							GUI.color = Color.white;
							
							if (GUILayout.Button("x", GUILayout.Width(30)))
							{
								tool.Remove(segment);
								SelectObject( tool.SelectedSegment.gameObject);
							}
						}
						GUILayout.EndHorizontal();
					}
					
					GUILayout.Space(5);
					if (GUILayout.Button("Rebuild"))
						tool.Rebuild();
					
					GUILayout.Space(5);
					if (GUILayout.Button("Clear"))
						tool.Clear();
				}
				GUILayout.EndVertical();
				
				GUILayout.BeginVertical("Box");
				{
					GUILayout.Box("Add new");
					foreach (RoadSegment segment in tool.settings.collection)
					{
						if (GUILayout.Button(segment.SegmentName))
						{
							tool.AddFromPrefab(segment);
							SelectObject(tool.SelectedSegment);
						}
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal(); 
		}

		RoadSegment lastSelectedRoadSegment;
		Bounds bounds;
		void OnDuringSceneGui(SceneView _)
		{ 
			if (tool.SelectedSegment)
			{
				if (lastSelectedRoadSegment != tool.SelectedSegment)
				{
					lastSelectedRoadSegment = tool.SelectedSegment;
					bounds = tool.SelectedSegment.transform.GetTotalRendererBounds();
				}

				Handles.color = Color.yellow;
				Handles.DrawWireCube(bounds.center, bounds.size * 1.3f);
				Handles.color = Color.white;
				
				HandleUtility.Repaint();
			}
		}
 
		void SelectObject(Object go)
		{
			Selection.activeObject = go;
		} 
	}
	
	
}