using UnityEngine;

namespace FieldGenerator
{
    public struct TerrainItem
    {
        public Vector3Int cell;
        public float density;
        public GameObject block;
        public BlockName blockName;

        public static implicit operator Vector3Int(TerrainItem item) => item.cell;

        public TerrainItem(Vector3Int cell, float density)
        {
            this.cell = cell;
            this.density = density;
            block = null;
            blockName = BlockName.Unknown;
        }
    }

    class TerrainGrid : Grid<TerrainItem>
    {
        public TerrainGrid(Vector3Int size) : base(size)
        {
        }
    }
}