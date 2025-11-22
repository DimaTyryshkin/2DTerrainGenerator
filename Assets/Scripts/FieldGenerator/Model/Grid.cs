using System.Collections.Generic;
using UnityEngine;

namespace FieldGenerator
{
    public class Grid<T> where T : struct
    {
        public readonly int ySize;
        public readonly int xSize;
        public readonly int zSize;
        private readonly int xySize;

        public T[] items;
        Vector3Int[] cells;

        public int CellCount { get; }
        public Vector3Int Size { get; }

        public T this[Vector3Int c]
        {
            get { return items[CellToIndex(c)]; }
            set { items[CellToIndex(c)] = value; }
        }

        public T this[int x, int y, int z]
        {
            get { return items[CellToIndex(x, y, z)]; }
            set { items[CellToIndex(x, y, z)] = value; }
        }

        public Grid(Vector3Int size)
        {
            Size = size;
            xSize = size.x;
            ySize = size.y;
            zSize = size.z;
            xySize = xSize * ySize;
            CellCount = ySize * xSize * zSize;

            items = new T[CellCount];
        }

        public int CellToIndex(Vector3Int c)
        {
            checked
            {
                return c.x + c.y * xSize + c.z * (xySize);
            }
        }

        public int CellToIndex(int x, int y, int z)
        {
            checked
            {
                return x + y * xSize + z * xySize;
            }
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

        public Vector3Int[] GetAllCells()
        {
            if (cells == null)
            {
                cells = new Vector3Int[CellCount];
                for (int x = 0; x < xSize; x++)
                {
                    for (int y = 0; y < ySize; y++)
                    {
                        for (int z = 0; z < zSize; z++)
                            cells[CellToIndex(x, y, z)] = new Vector3Int(x, y, z);
                    }
                }
            }

            return cells;
        }

        public IEnumerable<Vector3Int> GetAllColumns()
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                    yield return new Vector3Int(x, 0, z);
            }
        }

        public void GetNearIndexes(Vector3Int c, ref List<int> cells)
        {
            cells.Clear();
            Vector3Int c1 = new Vector3Int(c.x + 1, c.y + 0, c.z + 0);
            Vector3Int c2 = new Vector3Int(c.x + 0, c.y + 1, c.z + 0);
            Vector3Int c3 = new Vector3Int(c.x - 1, c.y + 0, c.z + 0);
            Vector3Int c4 = new Vector3Int(c.x + 0, c.y - 1, c.z + 0);
            Vector3Int c5 = new Vector3Int(c.x + 0, c.y + 0, c.z + 1);
            Vector3Int c6 = new Vector3Int(c.x + 0, c.y + 0, c.z - 1);

            if (IsCellExist(c1))
                cells.Add(CellToIndex(c1));

            if (IsCellExist(c2))
                cells.Add(CellToIndex(c2));

            if (IsCellExist(c3))
                cells.Add(CellToIndex(c3));

            if (IsCellExist(c4))
                cells.Add(CellToIndex(c4));

            if (IsCellExist(c5))
                cells.Add(CellToIndex(c5));

            if (IsCellExist(c6))
                cells.Add(CellToIndex(c6));
        }

        public Vector3Int GetUpCell(Vector3Int c)
        {
            return new Vector3Int(c.x, c.y + 1, c.z);
        }
    }
}