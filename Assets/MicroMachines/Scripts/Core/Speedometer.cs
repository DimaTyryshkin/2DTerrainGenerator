using UnityEngine;

namespace MicroMachines
{
	public class Speedometer : MonoBehaviour
	{
		Transform target;
		Vector3 lastPosition;

		public Vector3 Velocity { get; private set; }
		public float Speed => Velocity.magnitude;
		public float SpeedKm => Velocity.magnitude * CarRigidbody.MToKm;

		public static Speedometer Get(Transform target)
		{
			Speedometer speedometer = target.gameObject.GetComponent<Speedometer>();
			
			if (speedometer)
			{
				return speedometer;
			}
			else
			{
				speedometer = target.gameObject.AddComponent<Speedometer>();
				speedometer.target = target;

				return speedometer;
			}
		}

		void Start()
		{
			lastPosition = target.position;
		}

		void Update()
		{
			float dt = Time.deltaTime;

			if (Mathf.Approximately(dt, 0))
				return;

			Vector3 newPosition = target.position;
			Velocity = (newPosition - lastPosition) / dt;

			lastPosition = newPosition;
		}
	}
}