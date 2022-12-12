using System;
using System.Diagnostics.Contracts;
using System.Linq;
using NaughtyAttributes;
using SiberianWellness.NotNullValidation;
using UnityEngine;
 

namespace MicroMachines
{
	[Serializable]
	public class CarEngineParams
	{
		public float forwardInputScale;
		public float turnRadius;
		public float maxSpeedKmPerHour;
		public float tangentVelocityToFrequencyFactor;
		public AnimationCurve tangentVelocityToFrequencyCurve;
		public AnimationCurve forwardForceFromSpeedCurve;
		public AnimationCurve speedToRotationCurve;
		public float noAccelerationSpeedFactor;
	}


	public class CarRigidbody : MonoBehaviour
	{
		[SerializeField, IsntNull] Rigidbody thisRigidbody;
		[SerializeField, IsntNull] Transform centerOfMass;
		[SerializeField, IsntNull] Transform pointOfRotationForce;
		[SerializeField, IsntNull] CarEngineParams carEngineParams;
		[SerializeField] float wheelTorque;
		[SerializeField] float minK;
		[SerializeField] float maxK;
		[SerializeField, IsntNull] WheelCollider leftForward;
		[SerializeField, IsntNull] WheelCollider leftBack;
		[SerializeField, IsntNull] WheelCollider[] wheelColliders;
 
		public const float MToKm = 3.6f; 
		public const float KmToM = 1f/MToKm; 

		Vector2 lastInput;
		float lastWheelAngle;
		float distanceBetweenAxle;
		
		public float ForwardSpeed { get; private set; }
		public float TangentSpeed { get; private set; }
		public bool IsBake  { get; private set; }
		public Vector3 Velocity { get; private set; }
		public float WheelAngle => lastWheelAngle;
		public float MaxSpeed => carEngineParams.maxSpeedKmPerHour * KmToM;
		public float WheelRadius => leftForward.radius;

		void Start()
		{
			thisRigidbody.centerOfMass = centerOfMass.position - thisRigidbody.transform.position;
			distanceBetweenAxle = Vector3.Distance(leftForward.transform.position, leftBack.transform.position);
		}
 
		void FixedUpdate()
		{
			foreach (var wheelCollider in wheelColliders)
				wheelCollider.motorTorque = wheelTorque;
			
			if(wheelColliders.Any(x=>x.isGrounded))
				SpeedFixedUpdate2(lastInput);
			
			lastInput = Vector2.zero;
		}

		void SpeedFixedUpdate2(Vector2 input)
		{
			input.x *= carEngineParams.forwardInputScale;
			Vector3 forward = thisRigidbody.transform.forward.normalized;
			Vector3 velocity = thisRigidbody.velocity;
			Velocity = velocity;
			// Если больше нуля, значит машина едет вперед.
			float forwardSpeed = Vector3.Dot(velocity, forward);
			ForwardSpeed = forwardSpeed;
			Vector3 forwardVelocity = Vector3.Project(velocity, forward);
			Vector3 tangentVelocity = velocity - forwardVelocity;

			TangentSpeed = tangentVelocity.magnitude;

			// True, если скорость машины и ввод игрока не совпадают по направлению. Тоесть игрок хочет тормозить
			IsBake = (forwardSpeed * input.x) > 0 ;

			bool canApplyForce = false;

			if (!IsBake)
			{
				if (Mathf.Abs(forwardSpeed) < (carEngineParams.maxSpeedKmPerHour * KmToM))
					canApplyForce = true; // Разгоянемся
			}
			else
			{
				canApplyForce = true; // тормозим
			}

			if (canApplyForce)
			{
				float force = input.x * carEngineParams.forwardForceFromSpeedCurve.Evaluate(Mathf.Abs(forwardSpeed / (carEngineParams.maxSpeedKmPerHour * KmToM)));
				thisRigidbody.AddForce(force * Time.fixedDeltaTime * forward.normalized, ForceMode.VelocityChange);
			}

			// Тормождение на хололстом ходу
			if (Mathf.Approximately(input.x, 0))
			{
				float stopAcceleration = carEngineParams.noAccelerationSpeedFactor * (-Mathf.Sign(forwardSpeed));
				if (Mathf.Abs(forwardSpeed) < Mathf.Abs(stopAcceleration))
					stopAcceleration = -forwardSpeed;

				thisRigidbody.AddForce(forward.normalized * (stopAcceleration * Time.fixedDeltaTime), ForceMode.VelocityChange);
			}


			// Сила трения движения в бок
			float tangentFrequency = carEngineParams.tangentVelocityToFrequencyFactor * carEngineParams.tangentVelocityToFrequencyCurve.Evaluate(TangentSpeed * MToKm);
			thisRigidbody.AddForce(tangentVelocity * (tangentFrequency * Time.fixedDeltaTime), ForceMode.VelocityChange);

			// rotaton 
			//1)
			// float yInput = input.y * carEngineParams.speedToRotationCurve.Evaluate(forwardSpeed * MToKm);
			// lastWheelAngle = Mathf.Asin(distanceBetweenAxle/carEngineParams.turnRadius) * yInput * Mathf.Rad2Deg;
			//
			// float angleSpeed = yInput * (forwardSpeed/ carEngineParams.turnRadius) ;
			// float delta = angleSpeed - thisRigidbody.angularVelocity.y;
			//
			// delta *= deltaCurve.Evaluate(Mathf.Abs(delta));
			// float kk = forwardVelocity.magnitude / (velocity.magnitude + 0.1f);
			//
			// delta *= Mathf.Clamp(minK,maxK, kk);
			// thisRigidbody.AddTorque(new Vector3(0, delta , 0), ForceMode.VelocityChange);


			//2)
			// float speed = velocity.magnitude * Mathf.Sign(forwardSpeed);
			//
			// float yInput = input.y * carEngineParams.speedToRotationCurve.Evaluate( speed* MToKm);
			// lastWheelAngle = Mathf.Asin(distanceBetweenAxle/carEngineParams.turnRadius) * yInput * Mathf.Rad2Deg;
			//
			// float angleSpeed = yInput * (speed/ carEngineParams.turnRadius) ;
			// float delta = angleSpeed - thisRigidbody.angularVelocity.y;
			//
			// if (Mathf.Abs(forwardSpeed) < tangentVelocity.magnitude)
			// 	delta = Mathf.Sign(delta) * Time.deltaTime;
			//
			// thisRigidbody.AddTorque(new Vector3(0, delta , 0), ForceMode.VelocityChange);

			//3)
			// float speed = velocity.magnitude * Mathf.Sign(forwardSpeed);
			//
			// float yInput = input.y * carEngineParams.speedToRotationCurve.Evaluate( speed* MToKm);
			// lastWheelAngle = Mathf.Asin(distanceBetweenAxle/carEngineParams.turnRadius) * yInput * Mathf.Rad2Deg;
			//
			// float angleSpeed = yInput * (speed/ carEngineParams.turnRadius) ;
			// float delta = angleSpeed - thisRigidbody.angularVelocity.y;

			// // delta *= deltaCurve.Evaluate(Mathf.Abs(thisRigidbody.angularVelocity.y*Mathf.Rad2Deg));
			// //  if (Mathf.Abs(forwardSpeed) < tangentVelocity.magnitude)
			// //  	 delta = Mathf.Sign(delta);

			//thisRigidbody.AddTorque(new Vector3(0, delta * Time.fixedDeltaTime , 0), ForceMode.VelocityChange);

			//4)
			float yInput = input.y * carEngineParams.speedToRotationCurve.Evaluate(forwardSpeed * MToKm);
			lastWheelAngle = Mathf.Asin(distanceBetweenAxle / carEngineParams.turnRadius) * yInput * Mathf.Rad2Deg;

			float angleSpeed = yInput * (forwardSpeed / carEngineParams.turnRadius);
			float delta = angleSpeed - thisRigidbody.angularVelocity.y;

			float kk1 = forwardVelocity.magnitude / (velocity.magnitude + 0.1f);
			kk1 = Mathf.Lerp(minK, maxK, kk1);

			float kk2 = maxK / (velocity.magnitude + 1f);

			float kk = Mathf.Max(kk1, kk2);
			
			float resultDelta = delta *  Mathf.Clamp01(kk * Time.deltaTime);
			thisRigidbody.AddTorque(new Vector3(0, resultDelta, 0), ForceMode.VelocityChange);
		}
 
		/// <summary>
		/// Input axis
		/// x: 1=forward -1=back
		/// y: 1=right -1=left
		/// </summary>
		public void Input(Vector2 input)
		{
			lastInput = input;
		}

		public WheelCollider GetWheel(int index)
		{
			return wheelColliders[index];
		}



		[Button()]
		void Test()
		{
			// thisRigidbody.AddTorque(Vector3.up * k, ForceMode.VelocityChange);
		}
		
	}
}