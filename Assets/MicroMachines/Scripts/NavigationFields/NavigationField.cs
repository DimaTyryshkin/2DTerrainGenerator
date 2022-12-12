using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace MicroMachines.CarAi
{
	public struct NavigationFieldCastResult
	{
		public Vector3 moveDirection;
		public NavigationFieldPoint[] basePoints;
	}
	
	public class NavigationField : MonoBehaviour
	{
		[NonSerialized] NavigationFieldPoint[] points;

		public NavigationFieldPoint[] Points
		{
			get
			{
				if (points == null)
					points = GetComponentsInChildren<NavigationFieldPoint>(true);

				return points;
			}
		}


		void Start()
		{
			if (points == null)
				points = GetComponentsInChildren<NavigationFieldPoint>();
		}

		public void ReloadPoints()
		{
			points = null;
		}

		public NavigationFieldCastResult GetValue(Vector3 worldPoint, int basePointCount, bool debug = false)
		{
			Assert.IsTrue(basePointCount>0);
			
			NavigationFieldCastResult result = new NavigationFieldCastResult();

			// TODO нам не надо сортировать все точки. Достаточно выбрать несколько самых близких.
			NavigationFieldPoint[] nearPoints = Points
				.OrderBy(x => Vector3.Distance(worldPoint, x.transform.position))
				.Take(basePointCount)
				.ToArray();

			float[] distances = nearPoints
				.Select(x => Vector3.Distance(worldPoint, x.transform.position))
				.ToArray();

			Vector3 resultDir = Vector3.zero;

			int n = 0;
			foreach (NavigationFieldPoint point in nearPoints)
			{
				float k = 1000f / (distances[n] * distances[n] * distances[n] + 0.1f);
				Vector3 p = point.GetPoint(worldPoint);
			 
				Vector3 dir = (p - worldPoint).normalized;
				
				//debugVectors[n] = vector;
				//debugVectorsRed[n] = point.transform.position - transform.position;

				resultDir += dir * k;
				n++;
			}

			result.moveDirection = resultDir.normalized;
			result.basePoints = nearPoints;
			return result;
		}
		
		NavigationFieldPoint GetPoint(int index)
		{
			index = Mathf.Clamp(0, Points.Length - 1,index);
			return Points[index];
		}
	}
}