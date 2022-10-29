using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Terraria
{
	public class TerrainVoid
	{
		public readonly int index;
		public List<TerrainCell> cells;

		public TerrainVoid(List<TerrainCell> cells, int index)
		{
			this.cells = cells;
			this.index = index;
		}
	}
	
	public class TerrainDataOperations
	{
		class WaveCell : GridCell
		{
			public int distance;
			public int group;

			public WaveCell(Cell c, int distance, int group) : base(c)
			{
				this.distance = distance;
				this.group = group;
			}
		}


		TerrainData terrainData; 

		
		//--methods
		
		public TerrainDataOperations(TerrainData terrainData)
		{
			Assert.IsNotNull(terrainData);
			this.terrainData = terrainData;
		}

		Grid<WaveCell> GetNewGrid()
		{
			Grid<WaveCell> grid = new Grid<WaveCell>(terrainData.width, terrainData.width);

			foreach (TerrainCell terrainCell in terrainData.AllCells())
				grid.SetCell(terrainCell.cell, new WaveCell(terrainCell.cell, int.MaxValue, -1));

			return grid;
		}

		public List<TerrainVoid>  FindVoids()
		{
			Grid<WaveCell> grid  = GetNewGrid(); 
			List<TerrainVoid> terrainVoids = new List<TerrainVoid>(32);
			foreach (TerrainCell terrainCell in terrainData.AllCells())
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
				TerrainCell item = terrainData[c];
				if (item.density <= 0)
				{
					TerrainCell terrainCell = terrainData.GetCell(c);
					result.Add(terrainCell);
 
					int distance = waveGrid[c].distance + 1;
					foreach (TerrainCell near in terrainData.GetNearCells(terrainCell))
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