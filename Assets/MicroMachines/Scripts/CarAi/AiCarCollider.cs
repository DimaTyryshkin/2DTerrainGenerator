using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines.CarAi
{
	public class AiCarCollider : MonoBehaviour
	{
		[IsntNull] public NavigationFieldAiCarInput aiCarInput; 
	}
}