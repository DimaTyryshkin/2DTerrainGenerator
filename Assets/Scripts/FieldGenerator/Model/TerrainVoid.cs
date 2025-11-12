using System.Collections.Generic;
using UnityEngine;

namespace FieldGenerator
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

        public WaveCell(Vector3Int c, int distance, int group) : base(c)
        {
            this.distance = distance;
            this.group = group;
        }
    }
}