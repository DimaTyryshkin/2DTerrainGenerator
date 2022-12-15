using System;
using System.Collections;
using System.Linq;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MicroMachines.CarAi
{ 
	public class NavigationFieldAiCarInput : BaseCarInput
	{
		[SerializeField, IsntNull] CarRigidbody carRigidbody;
		[SerializeField, IsntNull] Transform navigationFieldPipette;
		[SerializeField, IsntNull] NavigationField navigationField;
		[SerializeField, IsntNull] GameObject debugMarker;

		[SerializeField] float stopTimer;
		[SerializeField] float speedLimitDuration;
		[SerializeField] float backwardDistance;
		[SerializeField] float predictionFactor;

		[SerializeField] bool debug;
		[SerializeField] bool oldAlg;

		[Header("Bake")] 
		[SerializeField] [Range(0,1) ] float bakeSpeed = 0.5f; 
		[SerializeField] [Range(0, 1)] float randomizeBake = 0; 
		[SerializeField] float bakePredictionFactor = 1; 

		[Header("Debug")]
		[SerializeField] bool old;
		[SerializeField] float t;

		float forwardAxisFactor;
		float timeSpeedZero;
		float timeToResetSpeedLimit;
		float speedLimit;
		bool isZeroSpeedTimerEnable;
		bool backwardAfterWall;
		Vector3 stopPosition;
		float rndBakeFactor;
		
		[SerializeField] bool inputEnable = true;

		public override bool InputEnable
		{
			get => inputEnable;
			set
			{
				inputEnable = value;
				carRigidbody.Input(Vector2.zero);
				
				if (inputEnable)
				{
					speedLimit = float.MaxValue;
					debugVectors = new Vector3[2];
					debugVectorsRed = new Vector3[2];
					forwardAxisFactor = 1;
				}
			}
		}

		void Update()
		{
			if(!inputEnable)
				return;
			
			if (!navigationField)
				return;

			CheckStop();
			OnBackwardMovement();

			MoveVersion_02();

			if (Time.time > timeToResetSpeedLimit)
			{
				speedLimit = float.MaxValue;
				debugMarker.SetActive(false);
			}
		}

		void MoveVersion_02()
		{
			NavigationFieldCastResult result = navigationField.GetValue(GetPredictionPoint(predictionFactor), 1);
			Vector3 dir = result.moveDirection;

			float verticalAxis = 0;
			if (carRigidbody.ForwardSpeed < speedLimit)
			{
				if (!backwardAfterWall)
				{
					if (Vector3.Dot(dir, transform.forward) < -0.7f)
					{
						//Едем не в ту сторону
						forwardAxisFactor = -1;
					}
					else
					{
						forwardAxisFactor = 1;
					}
				}

				float speedToBake = carRigidbody.MaxSpeed * bakeSpeed;
				float normalizedSpeed = carRigidbody.ForwardSpeed / speedToBake;
				if (normalizedSpeed < 1)
				{
					rndBakeFactor = Random.Range(-randomizeBake, randomizeBake);
					verticalAxis = 1;
					debugMarker.SetActive(false);
				}
				else
				{
					Vector3 predictedNavigationFieldValue = navigationField.GetValue(GetPredictionPoint(bakePredictionFactor), 1).moveDirection * 1f;
					verticalAxis = Mathf.Clamp01(Vector3.Dot(predictedNavigationFieldValue * 1.1f, carRigidbody.Velocity.normalized));

					verticalAxis = Mathf.Lerp(1, verticalAxis, normalizedSpeed);

					// random bake
					{
						float delta = 1 - verticalAxis;
						verticalAxis += delta * rndBakeFactor;
						verticalAxis = Mathf.Clamp01(verticalAxis);
					}

					verticalAxis = verticalAxis > 0 ? 1 : 0;
				}
			}

			// 

			float horizontalAxis = Vector3.Dot(dir, transform.right);
			horizontalAxis = Mathf.Clamp(horizontalAxis, -1, 1);
			float horizontalFactor = Mathf.Sign(carRigidbody.ForwardSpeed);

			Vector2 input = new Vector2(verticalAxis * forwardAxisFactor, horizontalAxis * horizontalFactor);
			carRigidbody.Input(input);
		}


		Vector3 GetPredictionPoint(float predictionFactor)
		{
			return navigationFieldPipette.position + transform.forward * (carRigidbody.ForwardSpeed * predictionFactor);
		}

		Vector3[] debugVectors;
		Vector3[] debugVectorsRed;
#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			if (debugVectors != null)
			{
				foreach (var vector in debugVectors)
				{
					Gizmos.DrawLine(transform.position, transform.position + vector);
				}
				
				Gizmos.color = Color.red;
				foreach (var vector in debugVectorsRed)
				{
					Gizmos.DrawLine(transform.position, transform.position + vector);
				}
			}

			if (carRigidbody)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(transform.position, GetPredictionPoint(predictionFactor));
				Gizmos.DrawWireSphere(GetPredictionPoint(predictionFactor),0.3f);
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(GetPredictionPoint(bakePredictionFactor),0.3f);
			}
		}
#endif

		void CheckStop()
		{
			if (Mathf.Abs(carRigidbody.ForwardSpeed) < 0.3f)
			{
				if (!isZeroSpeedTimerEnable)
				{
					//Начали стоть, видимо, уперлись в стену.
					stopPosition = transform.position;
					isZeroSpeedTimerEnable = true;
					timeSpeedZero = Time.time;
				}

				if (Time.time - timeSpeedZero > stopTimer)
				{
					// Проотсояли у тены нужное время, можно ехать назад
					backwardAfterWall = true;
					forwardAxisFactor *=-1;
					isZeroSpeedTimerEnable = false;
				}
			}
			else
			{
				isZeroSpeedTimerEnable = false;
			}
		}

		void StartBackwardMovement()
		{
		}

		void OnBackwardMovement()
		{
			if (forwardAxisFactor < 1)
			{
				if (Vector3.Distance(transform.position, stopPosition) > backwardDistance)
				{
					backwardAfterWall = false;
					forwardAxisFactor = 1;
				}
			}
		}

		public void OnBakeTriggerEnter(float speedLimit)
		{
			if (speedLimit < carRigidbody.MaxSpeed)
			{
				timeToResetSpeedLimit = Time.time + speedLimitDuration;
				this.speedLimit = speedLimit;
				debugMarker.SetActive(debug);
			}
		}
	}
}