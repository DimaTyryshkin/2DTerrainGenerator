using GamePackages.Core;
using System.Collections.Generic;
using UnityEngine;

namespace FieldGenerator
{
    class Chunk
    {
        public const int chunkSize = 16;
        public const int chunkHeight = 256;
        const float blockOffset = 0.5f;

        public readonly Vector3 position3D;
        public readonly Vector2Int position2D;
        public TerrainGrid grid;
        readonly GpuInstancer gpuInstancer;

        public Chunk(Vector2Int offest)
        {
            position2D = offest;
            position3D = new Vector3(offest.x, 0, offest.y);
            gpuInstancer = new GpuInstancer();
            grid = new TerrainGrid(new Vector3Int(chunkSize, chunkHeight, chunkSize));
        }

        public void InitGpuInstancer(BlockCollection blockCollection, ChunkGenerator chunkGenerator)
        {
            InitGpuInstancerForGame(blockCollection, chunkGenerator.showUnderground);
            //grid = null;
        }

        public void Update()
        {
            gpuInstancer.Draw();
        }

        void InitGpuInstancerForGame(BlockCollection blockCollection, bool showUnderground)
        {
            foreach (TerrainItem terrainCell in grid.items)
            {
                if (terrainCell.blockName != BlockName.Air)
                {
                    if (IsCellInvisible(terrainCell, showUnderground))
                        continue;

                    GpuInstancerUnit prefab = blockCollection.GetGpuInstancerUnitByName(terrainCell.blockName);
                    InstatniateBlock(terrainCell, prefab);
                }
            }
        }

        void InstatniateBlock(Vector3Int cell, GpuInstancerUnit prefab)
        {
            Vector3 pos = CellToWorldPos(cell);
            gpuInstancer.Add(prefab, pos);
        }

        List<int> nearIndexes = new();
        bool IsCellInvisible(Vector3Int cell, bool showUnderground)
        {
            grid.GetNearIndexes(cell, ref nearIndexes);

            if (showUnderground)
            {
                if (nearIndexes.Count != 6)
                    return false;

                foreach (int i in nearIndexes)
                {
                    if (grid.items[i].density <= 0)
                        return false;
                }

                return true;

            }
            else
            {
                int n = 0;
                foreach (int i in nearIndexes)
                {
                    if (grid.items[i].density > 0)
                        n++;
                }

                return nearIndexes.Count == n;
            }
        }

        Vector3 CellToWorldPos(Vector3Int c)
        {
            return new Vector3(
                c.x + blockOffset + position3D.x,
                c.y + blockOffset,
                c.z + blockOffset + position3D.z);
        }

        void Clear()
        {
            gpuInstancer.RemoveAll();
        }
    }
}