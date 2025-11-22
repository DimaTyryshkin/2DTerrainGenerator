using System.Collections.Generic;
using UnityEngine;

namespace FieldGenerator
{
    class TerrainVoid
    {
        public readonly int index;
        public List<TerrainItem> cells;

        public TerrainVoid(List<TerrainItem> cells, int index)
        {
            this.cells = cells;
            this.index = index;
        }
    }

    struct WaveItem
    {
        public Vector3Int cell;
        public int distance;
        public int group;

        public static implicit operator Vector3Int(WaveItem cell) => cell.cell;

        public WaveItem(Vector3Int cell, int distance, int group)
        {
            this.cell = cell;
            this.distance = distance;
            this.group = group;
        }
    }
}