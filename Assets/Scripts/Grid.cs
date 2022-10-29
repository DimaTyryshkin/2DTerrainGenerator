using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terraria
{
	public class GridCell<T> where T : class
	{
		protected readonly Grid<T> grid;
		public readonly int x;
		public readonly int y;
		public readonly T value;

		public Cell Cell => new Cell(x, y);

		public GridCell(GridCell<T> cell, T value)
		{
			this.grid = cell.grid;
			this.x = cell.x;
			this.y = cell.y;
			this.value = value;
		}

		public GridCell(Grid<T> grid, int x, int y, T value)
		{
			this.grid = grid;
			this.x = x;
			this.y = y;
			this.value = value;
		}

		public List<GridCell<T>> GetNearCells()
		{
			Cell c1 = new Cell(x + 1, y + 0);
			Cell c2 = new Cell(x + 0, y + 1);
			Cell c3 = new Cell(x - 1, y + 0);
			Cell c4 = new Cell(x + 0, y - 1);
			List<GridCell<T>> cells = new List<GridCell<T>>(4);

			if (grid.IsCellExist(c1))
				cells.Add(grid.GetCell(c1));

			if (grid.IsCellExist(c2))
				cells.Add(grid.GetCell(c2));

			if (grid.IsCellExist(c3))
				cells.Add(grid.GetCell(c3));

			if (grid.IsCellExist(c4))
				cells.Add(grid.GetCell(c4));

			return cells;
		}



		public GridCell<T> UpCell
		{
			get
			{
				Cell c = new Cell(this.x, y + 1);
				return grid.GetCellIfExist(c);
			}
		}
	}



	public class Grid<T>
		where T : class
	{
		public readonly int height;
		public readonly int width;
		GridCell<T>[] cells;

		public T this[Cell c]
		{
			get { return GetCell(c).value; }
			set { SetCell(c, value); }
		}

		public T this[int x, int y]
		{
			get { return GetCell(x, y).value; }
			set { SetCell(x, y, value); }
		}

		public Grid(int width, int height)
		{
			this.width = width;
			this.height = height;
			cells = new GridCell<T>[width * height];
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

		public GridCell<T> GetCellIfExist(Cell cell)
		{
			if (IsCellExist(cell))
				return GetCell(cell);
			else
				return null;
		}

		public GridCell<T> GetCell(Cell cell)
		{
			return cells[CellToIndex(cell)];
		}

		public GridCell<T> GetCell(int x, int y)
		{
			return cells[CellToIndex(x, y)];
		}

		public void SetCell(Cell c, T value)
		{
			cells[CellToIndex(c)] = new GridCell<T>(this, c.x, c.y, value);
		}

		public void SetCell(int x, int y, T value)
		{
			cells[CellToIndex(x, y)] = new GridCell<T>(this, x, y, value);
		}
		
		public IEnumerable<GridCell<T>> AllCells()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					yield return GetCell(x, y);
				}
			}
		}

		public IEnumerable<T2> SelectAllCells<T2>(Func<GridCell<T>, T2> func)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					yield return func(GetCell(x, y));
				}
			}
		}
	}
}