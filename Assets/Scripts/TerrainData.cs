namespace Terraria
{
    public class TerrainCell : GridCell
    {
        public float density;
        public Square view;

        public TerrainCell(Cell c, float density, Square view) : base(c)
        {
            this.density = density;
            this.view = view;
        }
    }

    public class TerrainData : Grid<TerrainCell>
    {
        public TerrainData(int width, int height) : base(width, height)
        {
        }
    }
}