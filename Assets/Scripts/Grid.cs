using System.Collections.Generic;

namespace Terraria
{
	public class GridCell
	{
		public readonly Cell cell;

		public static implicit operator Cell(GridCell cell) => cell.cell;
 
		public GridCell(Cell c)
		{
			cell = c;
		}
	}



	public class Grid<T>
		where T : GridCell
	{
		public readonly int height;
		public readonly int width;
		T[] cells;

		public int CellCount => height * width;
		
		public T this[Cell c]
		{
			get { return GetCell(c); }
			set { SetCell(c, value); }
		}

		public T this[int x, int y]
		{
			get { return GetCell(x, y); }
			set { SetCell(x, y, value); }
		}

		public Grid(int width, int height)
		{
			this.width = width;
			this.height = height;
			cells = new T[width * height];
		}

		public int CellToIndex(Cell c)
		{
			return c.x + c.y * width;
		}

		public int CellToIndex(int x, int y)
		{
			return x + y * width;
		}

		public bool IsCellExist(Cell cell)
		{
			if (cell.x >= 0 &&
			    cell.y >= 0 &&
			    cell.x < width &&
			    cell.y < height)
			{
				return true;
			}

			return false;
		}

		public T GetCellIfExist(Cell cell)
		{
			if (IsCellExist(cell))
				return GetCell(cell);
			
			return null;
		}

		public T GetCell(Cell cell)
		{
			return cells[CellToIndex(cell)];
		}

		public T GetCell(int x, int y)
		{
			return cells[CellToIndex(x, y)];
		}

		public void SetCell(Cell c, T value)
		{
			cells[CellToIndex(c)] = value;
		}

		public void SetCell(int x, int y, T value)
		{
			cells[CellToIndex(x, y)] = value;
		}
		
		public IEnumerable<T> AllCells()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					yield return GetCell(x, y);
				}
			}
		}
		
		public List<T> GetNearCells(Cell c)
		{
			Cell c1 = new Cell(c.x + 1, c.y + 0);
			Cell c2 = new Cell(c.x + 0, c.y + 1);
			Cell c3 = new Cell(c.x - 1, c.y + 0);
			Cell c4 = new Cell(c.x + 0, c.y - 1);
			List<T> cells = new List<T>(4);

			if (IsCellExist(c1))
				cells.Add(GetCell(c1));

			if (IsCellExist(c2))
				cells.Add(GetCell(c2));

			if (IsCellExist(c3))
				cells.Add(GetCell(c3));

			if (IsCellExist(c4))
				cells.Add(GetCell(c4));

			return cells;
		}

		public T GetUpCell(Cell c)
		{
			return GetCellIfExist(new Cell(c.x, c.y + 1));
		} 
	}
}