using SiberianWellness.NotNullValidation;
using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
	[CreateAssetMenu(fileName = "Clip")]
	public sealed class Clip : NoiseGeneratorAbstract
	{
		[SerializeField,IsntNull] NoiseGeneratorAbstract noise;
		[SerializeField,IsntNull] NoiseGeneratorAbstract clipNoise;
        
		public override float GetPoint(float x, float y)
		{
			if ( clipNoise.GetPoint(x, y) < 0)
				return 0;
			else
				return  noise.GetPoint(x, y);
		}
	}
}