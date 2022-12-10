using System;
using System.Collections.Generic;
using UnityEngine;

namespace MicroMachines.CarAi
{
	[Serializable]
	public class CarPathRecord
	{
		public List<CarPathPoint> points = new List<CarPathPoint>();
	}
	
	[Serializable]
	public struct CarPathPoint
	{
		public float time;
		public Vector3 pos;
		public Quaternion rot;
		public Quaternion[] wheelsRots;
		public Vector3[] wheelsPos;
	}
}