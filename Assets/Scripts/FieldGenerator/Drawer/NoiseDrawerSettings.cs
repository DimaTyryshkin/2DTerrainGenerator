using SiberianWellness.NotNullValidation;
using FieldGenerator.Terraria.NoiseGeneration;
using UnityEngine;

namespace FieldGenerator
{
	[CreateAssetMenu]
	public class NoiseDrawerSettings : ScriptableObject
	{
		[IsntNull] public NoiseField noiseField;
		[IsntNull] public ColorSchema colorSchema;
		[IsntNull] public Square squarePrefab;
		
		[Space] 
		[IsntNull] public GameObject marker;
	}
}