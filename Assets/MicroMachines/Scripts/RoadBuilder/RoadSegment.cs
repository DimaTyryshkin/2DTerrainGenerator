using UnityEngine;

namespace MicroMachines.RoadBuilder
{ 
	public abstract class RoadSegment:MonoBehaviour
	{
		public string SegmentName => gameObject.name; 
		
		public abstract Transform Enter { get; }

		public abstract Transform Exit { get; }
	}
}