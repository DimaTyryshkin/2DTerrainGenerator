using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines
{
	public class CarInput : BaseCarInput
	{
		[SerializeField, IsntNull] CarRigidbody carRigidbody;


		public override bool InputEnable { get; set; } = true;
		public bool DisableEngine { get; set; }

		void Update()
		{
			if(!InputEnable)
				return;
			
			Vector2 input = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

			if (DisableEngine)
				input.x = 0;
			
			carRigidbody.Input(input);
		}

	}
}