using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace Terraria
{
	[CreateAssetMenu]
	public class NoiseDrawerSettings : ScriptableObject
	{
		[IsntNull] public Terrain terrain;
		[IsntNull] public ColorSchema colorSchema;
		[IsntNull] public Square squarePrefab;
		
		[Space] 
		[IsntNull] public GameObject marker;
	}
}