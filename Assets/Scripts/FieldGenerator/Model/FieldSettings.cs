using GamePackages.Core.Validation;
using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public class TerrainCommon : ScriptableObject
    {
        [Space, IsntNull]
        public GameObject marker;

        [SerializeField] private Vector3Int size;

        public Vector3Int Size => size;
    }
}