using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.Assertions;

namespace MicroMachines
{
	public class ThirdPersonFollowCamera : MonoBehaviour
	{
		[SerializeField, IsntNull] Transform startOffsetMarker;
		[SerializeField] float k;

		Transform target;
		float startFlatDistance;
		float startHeight;

		public void Init()
		{
			Vector3 toTarget = transform.position - startOffsetMarker.position;
			startHeight = toTarget.y;
			toTarget.y = 0;
			startFlatDistance = toTarget.magnitude;
		}

		public void SetTarget(Transform target)
		{
			Assert.IsNotNull(target);
			this.target = target;
		}

		void LateUpdate()
		{
			Vector3 toTarget = target.position - transform.position;
			toTarget.y = 0;
			toTarget = toTarget.normalized * startFlatDistance;

			Vector3 newPosition = target.position + Vector3.up * startHeight - toTarget;
			transform.position = Vector3.Lerp(transform.position, newPosition, k * Time.deltaTime);
			transform.LookAt(target);
		}
	}
}