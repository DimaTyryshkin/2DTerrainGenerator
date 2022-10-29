namespace FieldGenerator
{
	public struct Cell
	{
		public int x;
		public int y;

		public Cell(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return x + " " + y;
		}
	}
}