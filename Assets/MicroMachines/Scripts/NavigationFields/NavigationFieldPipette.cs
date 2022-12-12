using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines.CarAi
{
	public class NavigationFieldPipette : MonoBehaviour
	{
		[SerializeField, IsntNull] NavigationField navigationField;
		[SerializeField, Range(1,10)] int basePointCount;
		
  
#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			if (!navigationField)
				return;

			navigationField.ReloadPoints();
			NavigationFieldCastResult result = navigationField.GetValue(transform.position, basePointCount);
			foreach (NavigationFieldPoint basePoint in result.basePoints)
			{
				Gizmos.DrawLine(transform.position, basePoint.transform.position);
			}

			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, transform.position + result.moveDirection * 10);

		}
#endif
	}
}