using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
	public sealed class FlatNoiseGenerator : NoiseGeneratorAbstract
	{  
		public override float GetPoint(float x, float y)
		{
			return -y;
		} 
	}
}