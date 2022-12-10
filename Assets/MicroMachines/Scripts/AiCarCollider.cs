using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace MicroMachines
{
	public class AiCarCollider : MonoBehaviour
	{
		[IsntNull] public AiCarInput aiCarInput; 
	}
}