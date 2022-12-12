using System;
using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines
{
	public class CarEffects : MonoBehaviour
	{

		[SerializeField, IsntNull] CarRigidbody carRigidbody;
		[IsntNull] public WheelView[] wheelsView;
		
		[Header("StopLight")]
		[SerializeField] Material stopLightMaterial;
		[SerializeField] MeshRenderer meshRenderer;
		[SerializeField] int materialIndex;

		float lastWheelRotationAngle;
		Material originalStopLightMaterial;
		bool stopLightEnable;

		void Start()
		{
			originalStopLightMaterial = meshRenderer.sharedMaterials[materialIndex];
			stopLightEnable = false;
			
			foreach (WheelView wheelView in wheelsView)
			{
				wheelView.Init();
				wheelView.trailRenderer.gameObject.SetActive(true);
			}
		}

		void Update()
		{
			//  
			var mats = meshRenderer.sharedMaterials;
			if (carRigidbody.IsBake && carRigidbody.Velocity.magnitude > 1)
			{
				mats[materialIndex] = stopLightMaterial;
			}
			else
			{
				mats[materialIndex] = originalStopLightMaterial;
			}

			meshRenderer.materials = mats;

			// steer wheels
			float angle = carRigidbody.WheelAngle;
			Quaternion steerRotation = Quaternion.Euler(0, angle, 0);

			// forward rotation wheels
			float rotationPerSecond = carRigidbody.ForwardSpeed / (carRigidbody.WheelRadius * 2 * Mathf.PI);
			lastWheelRotationAngle += rotationPerSecond * 360 * Time.deltaTime;
			Quaternion forwardRotation = Quaternion.Euler(lastWheelRotationAngle, 0, 0);
 
			for (int i = 0; i < wheelsView.Length; i++)
			{
				var wheelView = wheelsView[i];
				var wheelCollider = carRigidbody.GetWheel(i);
				
				wheelView.SetForwardRotation(forwardRotation);
				wheelView.SetSteerRotation(steerRotation);
				wheelView.ApplySpringMovement(wheelCollider);

				bool isGrounded = wheelCollider.isGrounded;
				if (isGrounded != wheelView.trailRenderer.emitting)
					wheelView.trailRenderer.emitting = isGrounded;
			}
		}
	}
}