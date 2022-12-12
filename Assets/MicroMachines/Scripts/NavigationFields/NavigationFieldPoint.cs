using NaughtyAttributes;
using UnityEngine;

namespace MicroMachines.CarAi
{
	public class NavigationFieldPoint : MonoBehaviour
	{
		float d;

		[SerializeField] Transform testTransform;
		[SerializeField] float distance;
		[SerializeField] float k;
		public bool isRightRadialPoint;
		public bool isLeftRadialPoint;
		[SerializeField] bool isFixedRadius;

		static bool hideGizmo;
		
		void Start()
		{
			d = -Vector3.Dot(transform.forward, transform.position);
		}

#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			if(hideGizmo)
				return;
			
			//Vector3 right = transform.right * 10;
			//Gizmos.DrawLine(transform.position + right, transform.position - right);
			bool isRadial = isLeftRadialPoint || isRightRadialPoint;
			if (isRadial)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(transform.position, distance);
			}
			else
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position, GetPoint(Vector3.zero));
			}
		} 
#endif

		public Vector3 GetPoint(Vector3 p)
		{
			if (isLeftRadialPoint || isRightRadialPoint)
			{
				//Vector3 dir = p - transform.position;
				
				Vector3 dir = p - transform.position;
				
				if(isFixedRadius)
					dir = dir.normalized * distance;
					
				Vector3 rotatedDir = new Vector3(-dir.z, dir.y, dir.x);
				
				if (isRightRadialPoint)
					rotatedDir = -rotatedDir;
				
				return transform.position + dir + rotatedDir * k;
			}

			return transform.position + transform.forward * distance;
		}

		public float GetDistanceToPlane(Vector3 point)
		{
			float distanceToPlane = Vector3.Dot(transform.forward, point) + d;
			return distanceToPlane;
		}

		[Button()]
		void Test()
		{
			Start();
			float distanceToPlane = GetDistanceToPlane(testTransform.position);
			Debug.Log(distanceToPlane);
		}
		
		[Button()]
		void SwitchGizmoDrawing()
		{
			hideGizmo = !hideGizmo;
		}
	}
}