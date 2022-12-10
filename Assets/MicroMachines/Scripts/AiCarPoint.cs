using System;
using NaughtyAttributes;
using UnityEngine;

namespace MicroMachines
{
	public class AiCarPoint : MonoBehaviour
	{
		float d;

		[SerializeField] Transform testTransform;
		
		void Start()
		{
			d = -Vector3.Dot(transform.forward, transform.position);
		}

#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			//Vector3 right = transform.right * 10;
			//Gizmos.DrawLine(transform.position + right, transform.position - right);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + transform.forward *5);
		}
		
		void OnDrawGizmosSelected()
		{
			//Vector3 right = transform.right * 10;
			//Gizmos.DrawLine(transform.position + right, transform.position - right);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + transform.forward *50);
		}
#endif

		public float GetDistanceToPlane(Vector3 point)
		{
			float distance = Vector3.Dot(transform.forward, point) + d;
			return distance;
		}

		[Button()]
		void Test()
		{
			Start();
			float distance = GetDistanceToPlane(testTransform.position);
			Debug.Log(distance);
		}
	}
}