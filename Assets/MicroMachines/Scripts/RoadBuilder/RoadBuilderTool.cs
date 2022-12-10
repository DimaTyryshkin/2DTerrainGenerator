using System;
using System.Collections.Generic;
using System.Linq;
using SiberianWellness.Common;
using SiberianWellness.NotNullValidation;
using UnityEditor;
using UnityEngine;

namespace MicroMachines.RoadBuilder
{
	public class RoadBuilderTool : MonoBehaviour
	{
		#if UNITY_EDITOR
		[IsntNull] public RoadBuilderToolSettings settings;
		[SerializeField, IsntNull] List<RoadSegment> segmentsInRoad;

		public IReadOnlyList<RoadSegment> SegmentsInRoad => segmentsInRoad;


		RoadSegment selectedSegment;

		public RoadSegment SelectedSegment
		{
			get => selectedSegment;
			set { selectedSegment = value; }
		}


		public RoadSegment AddFromPrefab(RoadSegment roadSegmentPrefab)
		{
			var go = PrefabUtility.InstantiatePrefab(roadSegmentPrefab.gameObject) as GameObject;
			go.name = roadSegmentPrefab.name;
			Undo.RegisterCreatedObjectUndo(go, nameof(RoadBuilderTool));
			Undo.SetTransformParent(go.transform, transform, nameof(RoadBuilderTool));
			go.transform.parent = transform;
			//RoadSegment newSegment = transform.InstantiateAsChild(roadSegmentPrefab);
			RoadSegment newSegment = go.GetComponent<RoadSegment>();
			Add(newSegment);
			return newSegment;
		}

		public void Add(RoadSegment newSegment)
		{
			Undo.RecordObject(this, nameof(RoadBuilderTool));
			if (segmentsInRoad.Count == 0)
			{
				newSegment.transform.rotation = Quaternion.identity;
				newSegment.transform.localPosition = Vector3.zero;
				
				segmentsInRoad.Add(newSegment);
			}
			else
			{
				if (!SelectedSegment)
					SelectedSegment = segmentsInRoad.Last();

				// Rotate
				Quaternion exitRot = SelectedSegment.Exit.rotation;
				Quaternion enterRot = newSegment.Enter.rotation;
				newSegment.transform.rotation *= enterRot.FromToRotation(exitRot);

				// Move
				Vector3 exitPos = SelectedSegment.Exit.position;
				Vector3 enterPos = newSegment.Enter.position;
				Vector3 deltaPos = exitPos - enterPos;
				newSegment.transform.position += deltaPos;
				
				segmentsInRoad.Insert(segmentsInRoad.IndexOf(SelectedSegment) + 1, newSegment);
			}

			SelectedSegment = newSegment;
		}

		public void Clear()
		{
			var copy = segmentsInRoad.ToArray();
			
			Undo.RecordObject(this, nameof(RoadBuilderTool));
			segmentsInRoad.Clear();
			
			foreach (var segment in copy)
				Undo.DestroyObjectImmediate(segment.gameObject);
		}

		 

		public void Remove(RoadSegment segment)
		{
			int index = segmentsInRoad.IndexOf(segment);
			if (index < 0)
				return;

			Undo.RecordObject(this, nameof(RoadBuilderTool));
			segmentsInRoad.RemoveAt(index);
			Undo.DestroyObjectImmediate(segment.gameObject);

			if (segmentsInRoad.Count > 0)
			{
				index = Mathf.Clamp(index - 1, 0, segmentsInRoad.Count - 1);
				SelectedSegment = segmentsInRoad[index];
			}
		}

		public void Rebuild()
		{
			var copySegmentsInRoad = segmentsInRoad
				.Where(x=>x)
				.ToArray();
			
			Undo.RecordObject(this, nameof(RoadBuilderTool));
			segmentsInRoad.Clear();
			foreach (RoadSegment oldSegment in copySegmentsInRoad)
				Add(oldSegment);
		}
		#endif
	}
}