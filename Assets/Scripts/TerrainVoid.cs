using System.Collections.Generic;

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
}