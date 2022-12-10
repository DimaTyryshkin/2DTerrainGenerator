using NaughtyAttributes;
using UnityEngine;

namespace MicroMachines.RoadBuilder
{
	public class QuaternionTest : MonoBehaviour
	{
		[SerializeField] Transform from;
		[SerializeField] Transform to;
		[SerializeField] Transform target;
		[SerializeField] float speed;

		bool run;


		[Button()]
		void Start()
		{
			run = false;
			target.rotation = from.rotation;
		}

		void Update()
		{
			if (!run)
				return;

			Quaternion q = Quaternion.Inverse(from.rotation) * to.rotation; 
			q = Quaternion.Lerp(Quaternion.identity, q,speed * Time.deltaTime);
			target.rotation *= q;
		}

		[Button()]
		void Run()
		{
			run = true;
		}

		[Button()]
		void Jump()
		{
			target.rotation = from.rotation;

			Quaternion q = from.rotation * Quaternion.Inverse(to.rotation);
			target.rotation *= q;
		}

		[Button()]
		void Jump2()
		{
			target.rotation = from.rotation;

			Quaternion q = Quaternion.Inverse(to.rotation) * from.rotation;
			target.rotation *= q;
		}

		[Button()]
		void Jump3()
		{
			target.rotation = from.rotation;

			Quaternion q = to.rotation * Quaternion.Inverse(from.rotation);
			target.rotation *= q;
		}

		[Button()]
		void Jump4()
		{
			target.rotation = from.rotation;

			Quaternion q = Quaternion.Inverse(from.rotation) * to.rotation; // Единственнов ерный вариант
			target.rotation *= q;
		}
	}
}