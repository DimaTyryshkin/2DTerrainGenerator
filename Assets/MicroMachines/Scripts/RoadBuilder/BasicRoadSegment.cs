using SiberianWellness.NotNullValidation;
using UnityEngine;


namespace MicroMachines.RoadBuilder
{
	public class BasicRoadSegment : RoadSegment
	{
		[SerializeField, IsntNull] public Transform enter;
		[SerializeField, IsntNull] public Transform exit;

		public override Transform Enter => enter;
		public override Transform Exit => exit;
	}
}