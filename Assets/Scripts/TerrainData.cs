using System.Collections.Generic;
using Terraria.NoiseGeneration;
using UnityEngine;
using UnityEngine.Assertions;

namespace Terraria
{
    public class TerrainCell : GridCell
    {
        public float density;

        public TerrainCell(Cell c, float density) : base(c)
        {
            this.density = density;
        }
    }

    public class TerrainData : Grid<TerrainCell>
    {
	    readonly Terrain terrain;
	    readonly NoiseGeneratorAbstract noiseGenerator;

	    public Cell  MinDensityCell { get; private set; }  
	    public Cell  MaxDensityCell { get; private set; }  
	    public float MinDensity { get; private set; }  
	    public float MaxDensity { get; private set; }  
	    
	    public TerrainData(Terrain terrain, NoiseGeneratorAbstract noiseGenerator) : base(terrain.Width, terrain.Height)
	    {
		    Assert.IsNotNull(terrain);
		    Assert.IsNotNull(noiseGenerator);
		    
		    this.terrain = terrain;
		    this.noiseGenerator = noiseGenerator;
	    }
        
        public void GenerateDensityField()
        {
            float offset = 0.5f;
        	int squaresOnWidth = terrain.Width;
        	int squaresOnHeight = terrain.Height;

        	Cell minDensityCell = new Cell(0, 0);
        	Cell maxDensityCell = minDensityCell;
        	float minDensity = float.MaxValue;
        	float maxDensity = float.MinValue;
        	for (int x = 0; x < squaresOnWidth; x++)
        	{
        		for (int y = 0; y < squaresOnHeight; y++)
        		{
                    Vector2 noisePoint = terrain.TerrainPointToNoisePoint(x + offset, y + offset);
        			float density = noiseGenerator.GetPoint(noisePoint.x, noisePoint.y);

        			Cell cell = new Cell(x, y);
        			if (density > maxDensity)
        			{
        				maxDensityCell = cell;
        				maxDensity = density;
        			}
        	
        			if (density < minDensity)
        			{
        				minDensityCell = cell;
        				minDensity = density;
        			}

        			SetCell(cell, new TerrainCell(cell, density));
        		}
        	}

            MinDensity = minDensity;
            MinDensityCell = minDensityCell;
            MaxDensity = maxDensity;
            MaxDensityCell = maxDensityCell;
        }
        
        public List<TerrainCell> FindGras()
        { 
            List<TerrainCell> result = new List<TerrainCell>(1024);
            foreach (TerrainCell cell in AllCells())
            {
                if (cell.density > 0)
                {
                    var up = GetUpCell(cell);

                    if (up != null && up.density <= 0)
                        result.Add(cell);
                }
            }

            return result;
        }
         
		Grid<WaveCell> GetNewGrid()
		{
			Grid<WaveCell> grid = new Grid<WaveCell>(width, width);

			foreach (TerrainCell terrainCell in AllCells())
				grid.SetCell(terrainCell.cell, new WaveCell(terrainCell.cell, int.MaxValue, -1));

			return grid;
		}

		public List<TerrainVoid>  FindVoids()
		{
			Grid<WaveCell> grid  = GetNewGrid(); 
			List<TerrainVoid> terrainVoids = new List<TerrainVoid>(32);
			foreach (TerrainCell terrainCell in AllCells())
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
		
		public List<TerrainCell> Wave(Cell cell)
		{
			Grid<WaveCell> waveGrid = GetNewGrid();

			List<TerrainCell> result = new List<TerrainCell>(waveGrid.CellCount);
			Queue<Cell> queue = new Queue<Cell>(waveGrid.CellCount);
			queue.Enqueue(cell);
			waveGrid[cell].distance = 0;

			while (queue.Count > 0)
			{
				Cell c = queue.Dequeue();
				TerrainCell item = this[c];
				if (item.density <= 0)
				{
					TerrainCell terrainCell = GetCell(c);
					result.Add(terrainCell);
 
					int distance = waveGrid[c].distance + 1;
					foreach (TerrainCell near in GetNearCells(terrainCell))
					{  
						if ( near.density <= 0 &&
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