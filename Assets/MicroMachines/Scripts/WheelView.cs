using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines
{
	public class WheelView : MonoBehaviour
	{
		[IsntNull] public TrailRenderer trailRenderer;
		[IsntNull] public Transform wheelView;
		[SerializeField, IsntNull] Transform forwardRotation;
		[SerializeField] bool isSteering;

		Vector3 originWheelLocalPosition;
		
		public void Init()
		{
			originWheelLocalPosition = transform.localPosition;
		}

		//public void SetForwardRotation(float angle, Vector3 newWorldPosition)
		public void SetForwardRotation(Quaternion newRotation )
		{
			forwardRotation.localRotation = newRotation;
		}
		
		public void SetSteerRotation(Quaternion newRotation )
		{
			if(isSteering)
				transform.localRotation = newRotation;
		}

		public void ApplySpringMovement(WheelCollider collider)
		{
			collider.GetWorldPose(out var position, out var rotation);
			
			transform.transform.position = position;
			var localPosition = transform.localPosition;
			localPosition.x = originWheelLocalPosition.x;
			transform.localPosition = localPosition;
		}
	}
}