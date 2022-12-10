using NaughtyAttributes;
using SiberianWellness.Common;
using UnityEngine;

namespace MicroMachines.RoadBuilder
{
	[CreateAssetMenu]
	public class RoadBuilderToolSettings : ObjectCollection<RoadSegment>
	{
#if UNITY_EDITOR
		[Button()]
		void Load()
		{
			LoadFromCurrentDirectory(true);
		}
#endif
	}
}