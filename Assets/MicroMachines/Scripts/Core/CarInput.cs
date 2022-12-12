using System.Text;
using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines
{
	public class CarInput : MonoBehaviour
	{
		[SerializeField, IsntNull] CarRigidbody carRigidbody;

		void Update()
		{
			Vector2 input = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
			carRigidbody.Input(input);
		}
	}
}