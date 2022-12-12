using System.Collections;
using System.Linq;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace MicroMachines.CarAi
{ 
	public class NavigationFieldAiCarInput : MonoBehaviour
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


		float forwardAxisFactor;
		float timeSpeedZero;
		float timeToResetSpeedLimit;
		float speedLimit;
		bool isZeroSpeedTimerEnable;
		Vector3 stopPosition;
		float rndBakeFactor;
		 
		

		IEnumerator Start()
		{
			speedLimit = 0;
			yield return new WaitForSeconds(0.3f);
			Run(); 
		}

		void Run()
		{
			speedLimit = float.MaxValue;
			debugVectors = new Vector3[2];
			debugVectorsRed = new Vector3[2];
			forwardAxisFactor = 1;
		}

		void Update()
		{
			if (!navigationField)
				return;

			CheckStop();
			OnBackwardMovement();

			MoveVersion_02();

			// if (Time.time > timeToResetSpeedLimit)
			// {
			// 	speedLimit = float.MaxValue;
			// 	debugMarker.SetActive(false);
			// }
		}

		void MoveVersion_02()
		{
			NavigationFieldCastResult result = oldAlg ? GetInputOld(navigationField.Points, GetPredictionPoint(predictionFactor)) : navigationField.GetValue(GetPredictionPoint(predictionFactor), 1);

			Vector3 dir = result.moveDirection;

			float horizontalAxis = Vector3.Dot(dir, transform.right);
			horizontalAxis = Mathf.Clamp(horizontalAxis, -1, 1);
			//float verticalAxis = Mathf.Clamp01(forward.magnitude / (tangent.magnitude + 0.1f));

			float verticalAxis = 0;

			if (carRigidbody.ForwardSpeed < speedLimit)
			{
				float speedToBake = carRigidbody.MaxSpeed * bakeSpeed;
				float normalizedSpeed = carRigidbody.ForwardSpeed / speedToBake;
				if (normalizedSpeed < 1)
				{
					rndBakeFactor =  Random.Range(-randomizeBake, randomizeBake);
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

					if (!mode1)
					{
						verticalAxis = verticalAxis > t ? 1 : 0;
					}

					debugMarker.SetActive(debug && verticalAxis < 0.9f);
				}
			}
			
			

			float horizontalFactor = Mathf.Sign(carRigidbody.ForwardSpeed);
			Vector2 input = new Vector2(verticalAxis * forwardAxisFactor, horizontalAxis * horizontalFactor);
			carRigidbody.Input(input);
		}
		[SerializeField] bool mode1;
		[SerializeField] float t;
		public static NavigationFieldCastResult GetInputOld(NavigationFieldPoint[] points, Vector3 worldPoint)
		{
			NavigationFieldPoint[] nearPoints = points
				.OrderBy(x => Vector3.Distance(worldPoint, x.transform.position))
				.Take(2)
				.ToArray();

			float[] distances = nearPoints
				.Select(x => Vector3.Distance(worldPoint, x.transform.position))
				.ToArray();

			float sumDistance = distances.Sum();
			Vector3 dir = Vector3.zero;

			int n = 0;
			foreach (NavigationFieldPoint point in nearPoints)
			{
				float weight = sumDistance - distances[n];
				Vector3 vector = point.transform.forward * weight;
				dir += vector;
				//debugVectors[n] = vector;
				//debugVectorsRed[n] = point.transform.position - transform.position;
				n++;
			}

			return new NavigationFieldCastResult()
			{
				moveDirection = dir.normalized
			};
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
					stopPosition = transform.position;
					isZeroSpeedTimerEnable = true;
					timeSpeedZero = Time.time;
				}

				if (Time.time - timeSpeedZero > stopTimer)
				{
					forwardAxisFactor *=-1;
					isZeroSpeedTimerEnable = false;
				}
			}
			else
			{
				isZeroSpeedTimerEnable = false;
			}
		}
		
		void OnBackwardMovement()
		{
			if (forwardAxisFactor < 1)
			{
				if (Vector3.Distance(transform.position, stopPosition) > backwardDistance)
					forwardAxisFactor = 1;
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