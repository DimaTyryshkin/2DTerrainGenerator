using System.Collections.Generic;
using UnityEngine;

namespace FieldGenerator
{
    public class GridCell
    {
        public readonly Vector3Int cell;

        public static implicit operator Vector3Int(GridCell cell) => cell.cell;

        public GridCell(Vector3Int c)
        {
            cell = c;
        }
    }



    public class Grid<T>
        where T : GridCell
    {
        public readonly int ySize;
        public readonly int xSize;
        public readonly int zSize;
        private readonly int xySize;

        T[] cells;

        public int CellCount => ySize * xSize * zSize;

        public T this[Vector3Int c]
        {
            get { return GetCell(c); }
            set { SetCell(c, value); }
        }

        public Grid(Vector3Int size)
        {
            xSize = size.x;
            ySize = size.y;
            zSize = size.z;
            xySize = xSize * ySize;

            cells = new T[CellCount];
        }

        public int CellToIndex(Vector3Int c)
        {
            return c.x + c.y * xSize + c.z * (xySize);
        }

        public int CellToIndex(int x, int y, int z)
        {
            return x + y * xSize + z * xySize;
        }

        public bool IsCellExist(Vector3Int cell)
        {
            if (cell.x >= 0 &&
                cell.y >= 0 &&
                cell.z >= 0 &&
                cell.x < xSize &&
                cell.y < ySize &&
                cell.z < zSize)
            {
                return true;
            }

            return false;
        }

        public T GetCellIfExist(Vector3Int cell)
        {
            if (IsCellExist(cell))
                return GetCell(cell);

            return null;
        }

        public T GetCell(Vector3Int cell)
        {
            return cells[CellToIndex(cell)];
        }

        public T GetCell(int x, int y, int z)
        {
            return cells[CellToIndex(x, y, z)];
        }

        public void SetCell(Vector3Int c, T value)
        {
            cells[CellToIndex(c)] = value;
        }

        public void SetCell(int x, int y, int z, T value)
        {
            cells[CellToIndex(x, y, z)] = value;
        }

        public IEnumerable<Vector3Int> GetAllCells()
        {
            //TODO надо сделать кеш
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    for (int z = 0; z < zSize; z++)
                        yield return new Vector3Int(x, y, z);
                }
            }
        }

        public T[] GetAllObjects() => cells;

        public List<T> GetNearCells(Vector3Int c)
        {
            Vector3Int c1 = new Vector3Int(c.x + 1, c.y + 0, c.z + 0);
            Vector3Int c2 = new Vector3Int(c.x + 0, c.y + 1, c.z + 0);
            Vector3Int c3 = new Vector3Int(c.x - 1, c.y + 0, c.z + 0);
            Vector3Int c4 = new Vector3Int(c.x + 0, c.y - 1, c.z + 0);
            Vector3Int c5 = new Vector3Int(c.x + 0, c.y + 0, c.z + 1);
            Vector3Int c6 = new Vector3Int(c.x + 0, c.y + 0, c.z - 1);
            List<T> cells = new List<T>(6); //TODO надо НЕ выделять память

            if (IsCellExist(c1))
                cells.Add(GetCell(c1));

            if (IsCellExist(c2))
                cells.Add(GetCell(c2));

            if (IsCellExist(c3))
                cells.Add(GetCell(c3));

            if (IsCellExist(c4))
                cells.Add(GetCell(c4));

            if (IsCellExist(c5))
                cells.Add(GetCell(c5));

            if (IsCellExist(c6))
                cells.Add(GetCell(c6));

            return cells;
        }

        public T GetUpCell(Vector3Int c)
        {
            return GetCellIfExist(new Vector3Int(c.x, c.y + 1, c.z));
        }
    }
}