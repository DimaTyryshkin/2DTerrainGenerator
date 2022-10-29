using UnityEngine;

namespace Terraria
{
	[CreateAssetMenu(fileName = "FlatNoiseGenerator")]
	public sealed class FlatNoiseGenerator : NoiseGeneratorAbstract
	{  
		public override float GetPoint(float x, float y)
		{
			return -y;
		} 
	}
}