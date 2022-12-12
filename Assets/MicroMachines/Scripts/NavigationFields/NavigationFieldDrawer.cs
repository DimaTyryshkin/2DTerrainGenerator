using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using SiberianWellness.Common;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MicroMachines.CarAi
{
	[ExecuteInEditMode]
	public class NavigationFieldDrawer : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField, IsntNull] NavigationField navigationField;
		[SerializeField, IsntNull] Transform startPoint;

		[Header("Main")]
		
	    [SerializeField] [Range(1, 10)] float fieldStep = 5;
		[SerializeField] bool drawOnStart;
		[SerializeField] bool selectedMode;
		[SerializeField] bool autoUpdate;
		[Space]
		[SerializeField] bool drawTrajectory;
		[SerializeField] bool drawField;
		[SerializeField] bool drawBounds;

		[Header("Common")] 
		
		[SerializeField] [Range(1, 4)] int basePointsCount = 1;
		[SerializeField] [Range(0.1f, 10f)] float autoUpdatePeriod = 1;

		
		[Header("Full mode")] 
		
		[SerializeField] float inflateValue = 15;
		[SerializeField] float maxDistanceToDraw = 30;

		
		[Header("Selected mode")] 
		
		[SerializeField] float selectedModeMaxDistanceToDraw = 30;

		
		[Header("Trajectory")]
		
		[SerializeField] float trajectoryLenght;
		[SerializeField] float trajectoryStep;
		[SerializeField] [Range(0.01f,1)] float smoothingFactor;

		List<Vector3> trajectoryPoints;
		List<Vector3> fieldPointsPositions;
		List<Vector3> fieldPointsTarget;
		Bounds bounds;
		Transform lastSelected;

		DateTime nextUpdateTime;


		void Start()
		{
			if (drawOnStart)
			{
				selectedMode = false;
				drawTrajectory = false;
				drawField = true;
				autoUpdate = false;
				BuildAll();
			}
		}

		void OnEnable()
		{
			SceneView.duringSceneGui -= OnSceneGui;
			SceneView.duringSceneGui += OnSceneGui;
		}

		void OnDisable()
		{
			SceneView.duringSceneGui -= OnSceneGui;
		}

		void OnDrawGizmos()
		{
			if(!enabled)
				return;
			   
			if (drawBounds)
				GizmosExtension.DrawBounds(bounds);

			if (drawField && fieldPointsPositions != null && fieldPointsTarget != null)
			{
				Gizmos.color = Color.yellow;
				for (int i = 0; i < fieldPointsPositions.Count; i++)
					GizmosExtension.DrawArrowXZ(fieldPointsPositions[i], fieldPointsTarget[i], 0.5f);
			}

			if (drawTrajectory && trajectoryPoints != null)
			{
				Gizmos.color = Color.green;
				for (int i = 0; i < trajectoryPoints.Count - 1; i++)
				{
					Vector3 p1 = trajectoryPoints[i];
					Vector3 p2 = trajectoryPoints[i + 1];
					Gizmos.DrawLine(p1, p2);
				}
			}
		}

		void OnSceneGui(SceneView obj)
		{
			Handles.BeginGUI();
			{
				GUILayout.BeginVertical(GUILayout.Width(150));
				{
					if (autoUpdate)
						GUI.color = Color.red;

					if (GUILayout.Button(autoUpdate ? "Disable auto Update" : "Enable auto Update"))
						autoUpdate = !autoUpdate;

					GUI.color = Color.white;
					
					if (!autoUpdate)
						if (GUILayout.Button("Update"))
							BuildAll();

					if (GUILayout.Button("Select"))
						Selection.activeObject = gameObject;
				}
				GUILayout.EndVertical();
			}
			Handles.EndGUI();

			if (autoUpdate)
			{
				if (DateTime.Now > nextUpdateTime)
				{
					BuildAll();
					nextUpdateTime = DateTime.Now + TimeSpan.FromSeconds(autoUpdatePeriod);
				}
			}
		}


		[Button()]
		void Build()
		{
			var t = DateTime.Now;
			BuildAll();
			Debug.Log($"Time={(DateTime.Now - t).TotalSeconds}");
		}

		void BuildAll()
		{
			navigationField.ReloadPoints();
			
			bounds = selectedMode ? GetBoundsForSelectedGo() : GetFullBounds();

			fieldPointsPositions = null;
			if(drawField)
				BuildField();

			trajectoryPoints = null;
			if(drawTrajectory)
				BuildTrajectory();
		}

		void BuildField()
		{
			if(selectedMode && !lastSelected)
				return;
			 
			Vector3 min = bounds.min;

			if (fieldStep < 0.1f)
				throw new Exception("Step < 0.1f");

			int xPoints = (int)(bounds.size.x / fieldStep) + 1;
			int zPoints = (int)(bounds.size.z / fieldStep) + 1;

			fieldPointsPositions = new List<Vector3>(10000);
			fieldPointsTarget = new List<Vector3>(10000);

			for (int xIndex = 0; xIndex < xPoints; xIndex++)
			{
				float x = min.x + xIndex * fieldStep;
				for (int zIndex = 0; zIndex < zPoints; zIndex++)
				{
					float z = min.z + zIndex * fieldStep;

					x = Mathf.Round(x / fieldStep) * fieldStep;
					z = Mathf.Round(z / fieldStep) * fieldStep;

					Vector3 point = new Vector3(x, 0, z);
					var castResult = navigationField.GetValue(point, basePointsCount);

					bool canAddPoint = false;
					if (selectedMode)
					{
						float distanceToBasePoint = Vector3.Distance(lastSelected.position, point);
						canAddPoint = distanceToBasePoint < selectedModeMaxDistanceToDraw;
					}
					else
					{
						float distanceToBasePoint = castResult.basePoints.Min(p => Vector3.Distance(p.transform.position, point));
						canAddPoint = distanceToBasePoint < maxDistanceToDraw;
					}
					
					if (canAddPoint)
					{
						fieldPointsPositions.Add(point);
						fieldPointsTarget.Add(point + castResult.moveDirection * fieldStep * 0.5f);
					}
				}
			}
		}

		void BuildTrajectory()
		{
			Assert.IsTrue(trajectoryStep >= 0.1f);
			Assert.IsTrue(trajectoryLenght < 10000);
			Assert.IsTrue(navigationField.Points.Length > 0);

			float lenght = 0;
			Vector3 p = startPoint.position;
			Vector3 oldDir = Vector3.zero;

			trajectoryPoints = new List<Vector3>(10000);
			trajectoryPoints.Add(p);
			while (lenght < trajectoryLenght) 
			{
				Vector3 dir = navigationField.GetValue(p, basePointsCount).moveDirection * trajectoryStep;
				oldDir = Vector3.MoveTowards(oldDir, dir, smoothingFactor * trajectoryStep);
				p += oldDir;
				lenght += oldDir.magnitude;
				trajectoryPoints.Add(p);
			}
		}

		Bounds GetFullBounds()
		{
			Vector3 firstPoint = navigationField.Points[0].transform.position;
			Bounds newBounds = new Bounds(firstPoint, Vector3.zero);

			foreach (NavigationFieldPoint point in navigationField.Points)
			{
				Vector3 p1 = point.GetPoint(point.transform.position);
				Vector3 p2 = point.transform.position;

				newBounds = BoundsExtension.Encompass(newBounds, BoundsExtension.FromMinMax(p1, p2));
			}

			newBounds = BoundsExtension.Inflate(newBounds, inflateValue);
			newBounds = BoundsExtension.ToFlatXZ(newBounds);

			return newBounds;
		}

		Bounds GetBoundsForSelectedGo()
		{ 
			if (Selection.activeGameObject)
			{
				var point = Selection.activeGameObject.GetComponent<NavigationFieldPoint>();
				if (point)
					lastSelected = point.transform;
			}

			if (!lastSelected)
				return new Bounds(Vector3.zero, Vector3.zero);

			Vector3 pointPos = lastSelected.position;
			var newBounds = new Bounds(pointPos, Vector3.zero);
			newBounds = BoundsExtension.Inflate(newBounds, selectedModeMaxDistanceToDraw);
			newBounds = BoundsExtension.ToFlatXZ(newBounds);

			return newBounds;
		}
#endif
	}
}