using FieldGenerator.Terraria.NoiseGeneration;
using GamePackages.Core.Validation;
using GamePackages.GamePackagesMath;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FieldGenerator
{
    struct WorldPoint2D
    {
        public float h;
        public BlockName block;
    }

    [CreateAssetMenu(menuName = "Game/ChunkGenerator")]
    class ChunkGenerator : ScriptableObject
    {
        public Vector3Int MinDensityCell { get; private set; }
        public Vector3Int MaxDensityCell { get; private set; }
        public float MinDensity { get; private set; }
        public float MaxDensity { get; private set; }

        [SerializeField, IsntNull] TerrainCommon fieldSettings;
        [SerializeField, IsntNull] BlockCollection blockCollection;



        [Header("MainPerlin")]
        [SerializeField] int perlinSeed = 1;
        [SerializeField] int octaves = 4;
        [SerializeField, Range(-1, 1)] float perlinOffset = 0;
        [SerializeField] float perlinScale = 1;
        [SerializeField] float waterHeight = 15;
        [SerializeField] float waterRadius = 30;

        [Header("NoizePerlin")]
        [SerializeField] float noizePerlinScale = 10;
        [SerializeField] float noizePerlinClip = 0.8f;

        [Header("Drawing")]
        public bool showUnderground;

        Vector2Int position2D;

        public void Generate(Vector2Int position2D, TerrainGrid grid, WorldPoint2D[,] heightMap)
        {
            this.position2D = position2D;
            Assert.IsNotNull(grid);

            GenerateFromHeightMap(grid, heightMap);

            //GenerateDencity(grid);
            //FillAir();

            ReplaceGras(grid);
            //InsertCoal(grid);
        }

        void GenerateFromHeightMap(TerrainGrid grid, WorldPoint2D[,] heightMap)
        {
            BlockInfo groundInfo = blockCollection.GetBlockByName(BlockName.Ground);
            BlockInfo stoneInfo = blockCollection.GetBlockByName(BlockName.Stone);

            int groundWidth = 3;

            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                for (int z = 0; z < Chunk.chunkSize; z++)
                {
                    WorldPoint2D point = heightMap[x, z];
                    int groundHeight = (int)(point.h * Chunk.chunkHeight);

                    for (int y = 0; y < Chunk.chunkHeight; y++)
                    {
                        Vector3Int cell = new Vector3Int(x, y, z);
                        TerrainItem item = new TerrainItem(cell, 1);

                        if (y < groundHeight - groundWidth)
                        {
                            item.blockName = BlockName.Stone;
                            item.density = 1;
                        }
                        else if (y < groundHeight)
                        {
                            item.blockName = BlockName.Ground;
                            item.density = 1;
                        }
                        else
                        {
                            item.blockName = BlockName.Air;
                            item.density = 0;
                        }

                        grid[cell] = item;
                    }
                }
            }
        }

        void GenerateDencity(TerrainGrid grid)
        {
            Vector3Int minDensityCell = new Vector3Int(0, 0, 0);
            Vector3Int maxDensityCell = minDensityCell;
            float minDensity = float.MaxValue;
            float maxDensity = float.MinValue;
            float offset = 0.5f;

            BlockInfo groundInfo = blockCollection.GetBlockByName(BlockName.Ground);
            BlockInfo stoneInfo = blockCollection.GetBlockByName(BlockName.Stone);

            PerlinNoise3D mainPerlin = new PerlinNoise3D(perlinSeed);


            foreach (Vector3Int c in grid.GetAllCells())
            {
                Vector3 p = new Vector3(c.x + offset, c.y + offset, c.z + offset);
                float density = ScaleFractalPerlin(mainPerlin, p, perlinScale) + perlinOffset; // [-1, 1]

                float toWater = (c.y - waterHeight);

                // 1
                //if (toWater > 0)
                //{
                //    density -= toWater / waterRadius;
                //}
                //else
                //{
                //    if (density > 0)
                //        density = 1;
                //}

                // 2 
                density -= toWater / waterRadius;

                if (density > maxDensity)
                {
                    maxDensityCell = c;
                    maxDensity = density;
                }

                if (density < minDensity)
                {
                    minDensityCell = c;
                    minDensity = density;
                }

                TerrainItem item = new TerrainItem(c, density);
                if (density <= 0)
                {
                    item.blockName = BlockName.Air;
                }
                else
                {
                    if (density >= groundInfo.density)
                        item.blockName = groundInfo.name;

                    if (density >= stoneInfo.density)
                        item.blockName = stoneInfo.name;
                }

                grid[c] = item;
            }

            MinDensity = minDensity;
            MinDensityCell = minDensityCell;
            MaxDensity = maxDensity;
            MaxDensityCell = maxDensityCell;
        }

        void ReplaceGras(TerrainGrid grid)
        {
            foreach (int index in FindGras(grid))
                grid.items[index].blockName = BlockName.Grass;
        }

        void InsertCoal(TerrainGrid grid)
        {
            PerlinNoise3D noizePerlin = new PerlinNoise3D(perlinSeed + 1);

            for (int i = 0; i < grid.CellCount; i++)
            {
                TerrainItem item = grid.items[i];
                float noize = ScalePerlin(noizePerlin, item.cell, noizePerlinScale);
                if (noize > noizePerlinClip)
                {
                    if (item.blockName == BlockName.Grass)
                    {
                        item.blockName = BlockName.Air;
                        item.density = 0;
                    }

                    if (item.blockName == BlockName.Ground)
                    {
                        item.blockName = BlockName.Stone;

                        if (noize > noizePerlinClip + 0.1f)
                            item.blockName = BlockName.Coal;
                    }

                    grid.items[i] = item;
                }

            }
        }

        void FillAir(TerrainGrid grid)
        {
            for (int index = 0; index < grid.CellCount; index++)
                if (grid.items[index].blockName == BlockName.Unknown)
                    grid.items[index].blockName = BlockName.Air;
        }

        public float ScalePerlin(PerlinNoise3D perlin, Vector3 p, float scale)
        {
            return perlin.Noise((p.x + position2D.x) * scale, p.y * scale, (p.z + position2D.y) * scale);
        }

        public float ScaleFractalPerlin(PerlinNoise3D perlin, Vector3 p, float scale)
        {
            return perlin.FractalNoise((p.x + position2D.x) * scale, p.y * scale, (p.z + position2D.y) * scale, octaves: octaves);
        }

        public List<int> FindGras(TerrainGrid grid)
        {
            List<int> result = new(2048);
            foreach (TerrainItem terrainCell in grid.items)
            {
                if (terrainCell.blockName == BlockName.Ground)
                {
                    Vector3Int up = grid.GetUpCell(terrainCell.cell);

                    if (grid.IsCellExist(up) && grid[up].blockName == BlockName.Air)
                        result.Add(grid.CellToIndex(terrainCell));
                }
            }

            return result;
        }

        public List<TerrainVoid> FindVoids(TerrainGrid grid)
        {
            Grid<WaveItem> waveGrid = GetWaveGrid(grid);
            List<TerrainVoid> terrainVoids = new List<TerrainVoid>(32);
            foreach (TerrainItem terrainCell in grid.items)
            {
                if (terrainCell.density <= 0 && waveGrid[terrainCell.cell].group < 0)
                {
                    List<TerrainItem> newVoidCells = Wave(terrainCell.cell, grid);
                    foreach (TerrainItem voidCell in newVoidCells)
                    {
                        waveGrid.items[grid.CellToIndex(voidCell)].group = terrainVoids.Count;
                    }

                    TerrainVoid newVoid = new TerrainVoid(newVoidCells, terrainVoids.Count);
                    terrainVoids.Add(newVoid);
                }
            }

            return terrainVoids;
        }




        Grid<WaveItem> GetWaveGrid(TerrainGrid grid)
        {
            Grid<WaveItem> waveGrid = new Grid<WaveItem>(grid.Size);

            foreach (Vector3Int c in grid.GetAllCells())
                waveGrid[c] = new WaveItem(c, int.MaxValue, -1);

            return waveGrid;
        }

        public List<TerrainItem> Wave(Vector3Int cell, TerrainGrid grid)
        {
            Grid<WaveItem> waveGrid = GetWaveGrid(grid);

            var result = new List<TerrainItem>(waveGrid.CellCount);
            var nearIndex = new List<int>(6);
            var queue = new Queue<int>(waveGrid.CellCount);
            int index = grid.CellToIndex(cell);
            queue.Enqueue(index);
            waveGrid.items[index].distance = 0;

            while (queue.Count > 0)
            {
                index = queue.Dequeue();
                TerrainItem item = grid.items[index];
                if (item.density <= 0)
                {
                    result.Add(item);

                    int distance = waveGrid.items[index].distance + 1;
                    grid.GetNearIndexes(item, ref nearIndex);
                    foreach (int nearId in nearIndex)
                    {
                        if (grid.items[nearId].density <= 0 && distance < waveGrid.items[nearId].distance)
                        {
                            waveGrid.items[nearId].distance = distance;
                            queue.Enqueue(nearId);
                        }
                    }
                }
            }

            return result;
        }

#if UNITY_EDITOR
        //[Button]
        void Draw()
        {
            //FindObjectOfType<TerrainDrawer>().DrawTerrain();
        }
#endif
    }
}