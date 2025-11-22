using UnityEngine;

namespace FieldGenerator
{
    [CreateAssetMenu]
    public class BlockCollection : ScriptableObject
    {
        [SerializeField]
        BlockInfo[] blocks;

        public BlockInfo GetBlockByName(BlockName name)
        {
            foreach (var block in blocks)
            {
                if (block.name == name)
                    return block;
            }

            throw new System.Exception($"Block '{name}' not found");
        }
    }

    [System.Serializable]
    public class BlockInfo
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
        Stone,
        Coal
    }

}