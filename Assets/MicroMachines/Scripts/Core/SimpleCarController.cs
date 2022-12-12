using System;
using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines
{
	public class SimpleCarController : MonoBehaviour
	{
		[System.Serializable]
		public class AxleInfo
		{
			public WheelCollider leftWheel;
			public WheelCollider rightWheel;
			public Transform leftView;
			public Transform rightView;
			public bool motor; // is this wheel attached to motor?
			public bool steering; // does this wheel apply steer angle?
		}

		public const float MToKm = 3.6f; 
		public const float KmToM = 1f/MToKm; 
		
		[SerializeField, IsntNull] AxleInfo[] axleInfos; // the information about each individual axle
		[SerializeField, IsntNull] Transform centerOfMass;  
		[SerializeField, IsntNull] Rigidbody thisRigidbody;  
		[SerializeField, IsntNull] AnimationCurve speedToRotationCurve;  
		
		public float maxMotorTorque; // maximum torque the motor can apply to wheel
		public float defaultBrakeTorque;  
		public float maxBrakeTorque;  
		public float maxSteeringAngle; // maximum steer angle the wheel can have

		[SerializeField] bool motorRpm;
		
		public float MotorSpeedMeterPerSecond
		{	
			get
			{
				float sum = 0;
				int count = 0;
				foreach (AxleInfo axleInfo in axleInfos)
				{
					if (!(axleInfo.motor ^ motorRpm)) 
					{
						sum += axleInfo.leftWheel.rpm;
						sum += axleInfo.rightWheel.rpm;
						count += 2;
					}
				}

				if (count == 0)
					return 0;

				float rpm = sum / count;

				float rps = rpm / 60f;
				float speed = rps * Mathf.PI * 2 * axleInfos[0].leftWheel.radius;
				return speed;
			}
		}

		void Start()
		{ 
			thisRigidbody.centerOfMass = centerOfMass.position - thisRigidbody.transform.position;
		}

		public void FixedUpdate()
		{
			Vector3 velocity = thisRigidbody.velocity;
			// Если больше нуля, значит машина едет вперед.
			float forwardSpeed = Vector3.Dot(velocity, transform.forward);
			
			float motor = maxMotorTorque * Input.GetAxis("Vertical");
			float steering = maxSteeringAngle * Input.GetAxis("Horizontal") * speedToRotationCurve.Evaluate(forwardSpeed * MToKm);

			float brake = defaultBrakeTorque;
			if (Input.GetKey(KeyCode.Space))
				brake = maxBrakeTorque;

			foreach (AxleInfo axleInfo in axleInfos)
			{
				if (axleInfo.steering)
				{ 
					axleInfo.leftWheel.steerAngle = steering;
					axleInfo.rightWheel.steerAngle = steering;
				}

				if (axleInfo.motor)
				{ 
					axleInfo.leftWheel.motorTorque = motor;
					axleInfo.rightWheel.motorTorque = motor;
				}
				
				axleInfo.leftWheel.brakeTorque = brake;
				axleInfo.rightWheel.brakeTorque = brake;
				
				ApplyLocalPositionToVisuals(axleInfo.leftWheel, axleInfo.leftView);
				ApplyLocalPositionToVisuals(axleInfo.rightWheel, axleInfo.rightView);
			}
		}
		
		void ApplyLocalPositionToVisuals(WheelCollider collider, Transform view)
		{
			if(!view)
				return;
			
			collider.GetWorldPose(out var position, out var rotation);
     
			view.transform.position = position;
			view.transform.rotation = rotation;
		}
	}
}