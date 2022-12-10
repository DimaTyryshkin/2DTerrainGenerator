using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
	public class NoiseField : ScriptableObject
	{
		[SerializeField] private int height;
		[SerializeField] private int width;

		[Space, SerializeField] 
		
		private int groundOffset = 0;
		[SerializeField] private float globalScale = 1;
		[SerializeField, Range(0, 0.1f)]  private float heightScale = 0.1f;

		public int Height => height;
		public int Width => width;
		public float GroundHeight => height*0.5f + groundOffset;
		public float GlobalScale => globalScale;
		public float HeightScale => heightScale;

		public Vector2 TerrainPointToNoisePoint(float x, float y)
		{
			Vector2 noisePoint = new Vector2(x * globalScale, (y - GroundHeight) * globalScale);
			return noisePoint;
		}
	}
}