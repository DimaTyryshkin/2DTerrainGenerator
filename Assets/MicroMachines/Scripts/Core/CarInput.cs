using System.Text;
using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines
{
	public class CarInput : MonoBehaviour
	{
		[SerializeField, IsntNull] CarRigidbody carRigidbody;

		public bool DisableEngine { get; set; }

		void Update()
		{
			Vector2 input = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

			if (DisableEngine)
				input.x = 0;
			
			carRigidbody.Input(input);
		}
	}
}