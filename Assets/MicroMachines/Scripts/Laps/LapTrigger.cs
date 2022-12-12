using UnityEngine;
using UnityEngine.Events;

namespace MicroMachines.Lap
{
	public class LapTrigger : MonoBehaviour
	{
		public event UnityAction<LapTrigger, Racer> TriggerEnter;

		public int TriggerIndex { get; private set; }


		void Start()
		{
			TriggerIndex = transform.GetSiblingIndex();
			Destroy(GetComponent<MeshRenderer>());
			Destroy(GetComponent<MeshFilter>());
		}

		void OnTriggerEnter(Collider other)
		{
			Racer racer = other.GetComponentInParent<Racer>();
			if (racer)
				TriggerEnter?.Invoke(this, racer);
		}
	}
}