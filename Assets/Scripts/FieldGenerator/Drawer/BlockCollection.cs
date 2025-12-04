using GamePackages.Core;
using System;
using System.Linq;
using UnityEngine;

namespace FieldGenerator
{
    [CreateAssetMenu]
    public class BlockCollection : ScriptableObject
    {
        [SerializeField] BlockInfo[] blocks;
        [NonSerialized] GpuInstancerUnit[] gpuInstancerUnits;

        public void Init()
        {
            gpuInstancerUnits = blocks.Select(block =>
            {
                return new GpuInstancerUnit()
                {
                    prefab = block.prefab,
                    prefabRotation = block.prefab.transform.rotation,
                    prefabScale = block.prefab.transform.lossyScale,
                    material = block.prefab.GetComponent<MeshRenderer>().sharedMaterial,
                    mesh = block.prefab.GetComponent<MeshFilter>().sharedMesh,

                };
            }).ToArray();
        }

        public BlockInfo GetBlockByName(BlockName name)
        {
            foreach (var block in blocks)
            {
                if (block.name == name)
                    return block;
            }

            throw new System.Exception($"Block '{name}' not found");
        }

        public GpuInstancerUnit GetGpuInstancerUnitByName(BlockName name)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i].name == name)
                    return gpuInstancerUnits[i];
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