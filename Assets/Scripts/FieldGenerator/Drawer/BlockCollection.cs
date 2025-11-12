using UnityEngine;

namespace FieldGenerator
{
    [CreateAssetMenu]
    public class BlockCollection : ScriptableObject
    {
        [SerializeField]
        DencityToBlock[] blocks;

        public GameObject GetBlockByName(BlockName name)
        {
            foreach (var block in blocks)
            {
                if (block.name == name)
                    return block.prefab;
            }

            throw new System.Exception($"Block '{name}' not found");
        }

        public GameObject GetBlockByDensity(float density)
        {
            if (density == 0)
                return null;

            if (density > 0.5f)
                return GetBlockByName(BlockName.Stone);

            return GetBlockByName(BlockName.Ground);
        }
    }

    [System.Serializable]
    public class DencityToBlock
    {
        public BlockName name;
        public GameObject prefab;
        public float density;
    }

    public enum BlockName
    {
        Unknown = 0,
        Air = 1,
        Grass,
        Ground,
        Stone
    }

}