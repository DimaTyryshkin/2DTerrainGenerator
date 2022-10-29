using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Terraria
{
	public class TerrainVoid
	{
		public readonly int index;
		public List<GridCell<TerrainCell>> cells;

		public TerrainVoid(List<GridCell<TerrainCell>> cells, int index)
		{
			this.cells = cells;
			this.index = index;
		}
	}
	
	public class TerrainDataOperations
	{
		class WaveItem
		{
			public int distance;
			public int group;

			public WaveItem( int distance, int group)
			{
				this.distance = distance;
				this.group = group;
			}
		}
 
		
		TerrainData terrainData; 

		public TerrainDataOperations(TerrainData terrainData)
		{
			Assert.IsNotNull(terrainData);
			this.terrainData = terrainData;
		}

		Grid<WaveItem> GetNewGrid()
		{
			Grid<WaveItem> grid = new Grid<WaveItem>(terrainData.width, terrainData.width);

			foreach (GridCell<TerrainCell> terrainCell in terrainData.AllCells())
				grid.SetCell(terrainCell.Cell, new WaveItem(int.MaxValue, -1));

			return grid;
		}

		public List<TerrainVoid>  FindVoids()
		{
			Grid<WaveItem> grid  = GetNewGrid(); 
			List<TerrainVoid> terrainVoids = new List<TerrainVoid>(32);
			foreach (GridCell<TerrainCell> terrainCell in terrainData.AllCells())
			{
				if (terrainCell.value.density <= 0 && grid[terrainCell.Cell].group < 0)
				{
					List<GridCell<TerrainCell>> newVoidCells = Wave(terrainCell.Cell);
					foreach (GridCell<TerrainCell> voidCell in newVoidCells)
						grid[voidCell.Cell].group = terrainVoids.Count;
					
					TerrainVoid newVoid = new TerrainVoid(newVoidCells, terrainVoids.Count);
					terrainVoids.Add(newVoid);
				}
			}

			return terrainVoids;
		}
		
		public List<GridCell<TerrainCell>> Wave(Cell cell)
		{
			Grid<WaveItem> grid  = GetNewGrid(); 
			
			List<GridCell<TerrainCell>> result = new List<GridCell<TerrainCell>>(1024 * 10);
			Queue<Cell> queue = new Queue<Cell>(1024 * 10);
			queue.Enqueue(cell);
			grid[cell].distance = 0;

			while (queue.Count > 0)
			{
				Cell c = queue.Dequeue();
				TerrainCell item = terrainData[c];
				if (item.density <= 0)
				{
					GridCell<TerrainCell> terrainCell = terrainData.GetCell(c);
					result.Add(terrainCell);
 
					int distance = grid[c].distance + 1;
					foreach (GridCell<TerrainCell> near in terrainCell.GetNearCells())
					{  
						if ( near.value.density <= 0 &&
						     distance < grid[near.Cell].distance)
						{
							grid[near.Cell].distance = distance;
							queue.Enqueue(near.Cell);
						}
					}
				}
			}

			return result;
		}

	}
}