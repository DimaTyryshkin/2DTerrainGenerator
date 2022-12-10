using System.Collections;
using System.Linq;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MicroMachines
{
	public class AiCarInput : MonoBehaviour
	{
		[SerializeField, IsntNull] CarRigidbody carRigidbody;
		[SerializeField, IsntNull] Transform pointsRoot;
		[SerializeField, IsntNull] GameObject debugMarker;

		[SerializeField] float stopTimer;
		[SerializeField] float speedLimitDuration;
		[SerializeField] float backwardDistance;
		
		[SerializeField] float predictionFactor;

		[SerializeField] bool debug;

		AiCarPoint[] points;

		float forwardAxisFactor;
		float timeSpeedZero;
		float timeToResetSpeedLimit;
		float speedLimit;
		bool isZeroSpeedTimerEnable;
		Vector3 stopPosition;

		IEnumerator Start()
		{
			yield return new WaitForSeconds(0.3f);
			Play(); 
		}
		 
		void Play()
		{
			speedLimit = float.MaxValue;
			debugVectors = new Vector3[2];
			debugVectorsRed = new Vector3[2];
			points = pointsRoot.GetComponentsInChildren<AiCarPoint>();
			forwardAxisFactor = 1;
		}

		void Update()
		{
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

			AiCarPoint[] nearPoints = points
				.OrderBy(x =>Vector3.Distance(GetSelfPoint() ,x.transform.position))
				.Take(2)
				.ToArray();

			float[] distances = nearPoints
				.Select(x => Vector3.Distance(GetSelfPoint() ,x.transform.position))
				.ToArray();

			float sumDistance = distances.Sum();
			Vector3 dir = Vector3.zero;

			int n = 0;
			foreach (AiCarPoint point in nearPoints)
			{
				float weight = sumDistance - distances[n];
				Vector3 vector = point.transform.forward * weight;
				dir += vector;
				debugVectors[n] = vector;
				debugVectorsRed[n] = point.transform.position - transform.position;
				n++;
			}

			dir = dir.normalized;
			
		  
			Vector3 forward = Vector3.Project(dir, transform.forward);
			Vector3 tangent = Vector3.Project(dir, transform.right);

			float horizontalAxis = Vector3.Dot(dir, transform.right);
			//float verticalAxis = Mathf.Clamp01(forward.magnitude / (tangent.magnitude + 0.1f));
			float verticalAxis =carRigidbody.ForwardSpeed > speedLimit ? 0: 1f;
			float horizontalFactor = Mathf.Sign(carRigidbody.ForwardSpeed);
			Vector2 input = new Vector2(verticalAxis * forwardAxisFactor, horizontalAxis * horizontalFactor);
			carRigidbody.Input(input); 
		}

		Vector3 GetSelfPoint()
		{
			return transform.position + carRigidbody.Velocity * predictionFactor;
		}

		AiCarPoint GetPoint(int index)
		{
			index = Mathf.Clamp(0, points.Length - 1,index);
			return points[index];
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
				Gizmos.color = Color.green;
				Gizmos.DrawLine(transform.position, GetSelfPoint());
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

				float min = Mathf.Clamp(speedLimit - 20, 0, speedLimit);
				this.speedLimit = Mathf.Lerp(min , carRigidbody.MaxSpeed, Random.Range(0f, 1f));
				debugMarker.SetActive(debug);
			}
		}
	}
}