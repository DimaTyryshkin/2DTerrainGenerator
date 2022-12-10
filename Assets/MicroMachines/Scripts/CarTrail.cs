using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines
{
	public class CarTrail : MonoBehaviour
	{
		[SerializeField, IsntNull] CarEffects carEffects;
		[SerializeField, IsntNull] WheelCollider[] wheelColliders;

		[SerializeField] float torque;
		
		void Start()
		{
			foreach (WheelView wheelView in carEffects.wheelsView)
			{
				wheelView.Init();
				wheelView.trailRenderer.gameObject.SetActive(true);
			}
		}
		
		void Update()
		{ 
			for (int i = 0; i < wheelColliders.Length; i++)
			{
				var wheelView = carEffects.wheelsView[i];
				var wheelCollider = wheelColliders[i];
				wheelCollider.motorTorque = torque;

				bool isGrounded = wheelCollider.isGrounded;
				if (isGrounded != wheelView.trailRenderer.emitting)
					wheelView.trailRenderer.emitting = isGrounded;
			}
		}
	}
}