using System;
using UnityEngine;

namespace MicroMachines.CarAi
{
	public class AiSpeedLimitTrigger : MonoBehaviour
	{
		[SerializeField] float maxSpeed;

		void Start()
		{
			Destroy(GetComponent<MeshRenderer>());
			Destroy(GetComponent<MeshFilter>());
		}

		void OnTriggerEnter(Collider other)
		{
			var ai = other.GetComponent<AiCarCollider>();
			if (ai)
				ai.aiCarInput.OnBakeTriggerEnter(maxSpeed * CarRigidbody.KmToM);
		}
	}
}