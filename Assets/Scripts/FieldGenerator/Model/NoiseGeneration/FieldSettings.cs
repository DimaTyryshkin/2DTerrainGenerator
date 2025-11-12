using GamePackages.Core.Validation;
using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public class FieldSettings : ScriptableObject
    {
        [Space, IsntNull]
        public GameObject marker;

        [SerializeField] private Vector3Int size;

        [Space, SerializeField]

        private int groundOffset = 0;
        [SerializeField] private float globalScale = 1;

        [SerializeField, Range(0, 0.1f)] private float heightScale = 0.1f;

        public float GroundHeight => size.y * 0.5f + groundOffset;
        public Vector3Int Size => size;
        public float GlobalScale => globalScale;
        public float HeightScale => heightScale;

        public Vector3 TerrainPointToNoisePoint(Vector3 p)
        {
            Vector3 noisePoint = new Vector3(p.x * globalScale, (p.y - GroundHeight) * globalScale, p.z * globalScale);
            return noisePoint;
        }
    }
}