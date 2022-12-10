using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines
{
	public class CarRoot : MonoBehaviour
	{
		[IsntNull] public Transform cameraTarget;
		[IsntNull] public Transform thirdPersonCameraTarget; 
	}
}