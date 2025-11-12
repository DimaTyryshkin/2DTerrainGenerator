using System.Collections.Generic;
using FieldGenerator.Terraria.NoiseGeneration;
using UnityEngine;
using UnityEngine.Assertions;

namespace FieldGenerator
{
    public class TerrainCell : GridCell
    {
        public float density;
        public GameObject block;

        public TerrainCell(Vector3Int c, float density) : base(c)
        {
            this.density = density;
        }
    }

    public class TerrainGrid : Grid<TerrainCell>
    {
        readonly FieldSettings fieldSettings;
        readonly Function3D dencityfunction;

        public Vector3Int MinDensityCell { get; private set; }
        public Vector3Int MaxDensityCell { get; private set; }
        public float MinDensity { get; private set; }
        public float MaxDensity { get; private set; }

        public TerrainGrid(FieldSettings fieldSettings, Function3D dencityfunction) : base(fieldSettings.Size)
        {
            Assert.IsNotNull(fieldSettings);
            Assert.IsNotNull(dencityfunction);

            this.fieldSettings = fieldSettings;
            this.dencityfunction = dencityfunction;
        }

        public void GenerateDensityField()
        {
            float offset = 0.5f;

            Vector3Int minDensityCell = new Vector3Int(0, 0, 0);
            Vector3Int maxDensityCell = minDensityCell;
            float minDensity = float.MaxValue;
            float maxDensity = float.MinValue;

            foreach (Vector3Int c in GetAllCells())
            {
                Vector3 p = new Vector3(c.x + offset, c.y + offset, c.z + offset);
                Vector3 noisePoint = fieldSettings.TerrainPointToNoisePoint(p);
                float density = dencityfunction.GetValue(noisePoint);

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

                SetCell(c, new TerrainCell(c, density));
            }


            MinDensity = minDensity;
            MinDensityCell = minDensityCell;
            MaxDensity = maxDensity;
            MaxDensityCell = maxDensityCell;
        }

        public List<TerrainCell> FindGras()
        {
            List<TerrainCell> result = new List<TerrainCell>(1024);
            foreach (TerrainCell terrainCell in GetAllObjects())
            {
                if (terrainCell.density > 0)
                {
                    var up = GetUpCell(terrainCell.cell);

                    if (up != null && up.density <= 0)
                        result.Add(terrainCell);
                }
            }

            return result;
        }

        Grid<WaveCell> GetNewGrid()
        {
            Grid<WaveCell> grid = new Grid<WaveCell>(fieldSettings.Size);

            foreach (TerrainCell terrainCell in GetAllObjects())
                grid.SetCell(terrainCell.cell, new WaveCell(terrainCell.cell, int.MaxValue, -1));

            return grid;
        }

        public List<TerrainVoid> FindVoids()
        {
            Grid<WaveCell> grid = GetNewGrid();
            List<TerrainVoid> terrainVoids = new List<TerrainVoid>(32);
            foreach (TerrainCell terrainCell in GetAllObjects())
            {
                //var c = grid[terrainCell.Cell];
                //c.
                if (terrainCell.density <= 0 && grid[terrainCell.cell].group < 0)
                {
                    List<TerrainCell> newVoidCells = Wave(terrainCell.cell);
                    foreach (TerrainCell voidCell in newVoidCells)
                        grid[voidCell.cell].group = terrainVoids.Count;

                    TerrainVoid newVoid = new TerrainVoid(newVoidCells, terrainVoids.Count);
                    terrainVoids.Add(newVoid);
                }
            }

            return terrainVoids;
        }

        public List<TerrainCell> Wave(Vector3Int cell)
        {
            Grid<WaveCell> waveGrid = GetNewGrid();

            List<TerrainCell> result = new List<TerrainCell>(waveGrid.CellCount);
            Queue<Vector3Int> queue = new Queue<Vector3Int>(waveGrid.CellCount);
            queue.Enqueue(cell);
            waveGrid[cell].distance = 0;

            while (queue.Count > 0)
            {
                Vector3Int c = queue.Dequeue();
                TerrainCell item = this[c];
                if (item.density <= 0)
                {
                    TerrainCell terrainCell = GetCell(c);
                    result.Add(terrainCell);

                    int distance = waveGrid[c].distance + 1;
                    foreach (TerrainCell near in GetNearCells(terrainCell))
                    {
                        if (near.density <= 0 &&
                             distance < waveGrid[near].distance)
                        {
                            waveGrid[near].distance = distance;
                            queue.Enqueue(near);
                        }
                    }
                }
            }

            return result;
        }
    }
}