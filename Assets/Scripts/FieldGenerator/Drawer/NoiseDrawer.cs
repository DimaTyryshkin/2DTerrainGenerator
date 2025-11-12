using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FieldGenerator.Terraria.NoiseGeneration;
using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FieldGenerator
{
    public class NoiseDrawer : MonoBehaviour
    {
        [SerializeField, IsntNull] Function3D noiseGenerator;
        [SerializeField, IsntNull] BlockCollection blockCollection;
        [SerializeField, IsntNull] bool markVoids;
        [SerializeField, IsntNull] DynamicObjectPool pool;

        [Space, SerializeField, IsntNull] FieldSettings fieldSettings;

        Transform squaresRoot;
        Transform markersRoot;
        TerrainGrid terrainGrid;
        Vector3 offset;
        Rect viewFieldRect;

        public TerrainGrid TerrainGrid => terrainGrid;
        public FieldSettings FieldSettings => fieldSettings;
        public Vector3 Center => transform.position + (Vector3)fieldSettings.Size * 0.5f;


        private void Start()
        {
            Draw();
        }

        [Button]
        public void Draw()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Clear();
            Debug.Log($"Clear '{watch.Elapsed.TotalSeconds}' sec");
            viewFieldRect = new Rect(transform.position, new Vector2(fieldSettings.Size.x, fieldSettings.Size.y));

            GenerateDensityField();
            FindGras();
            //MarkSky(new Cell(0, terrainData.height - 1));

            if (markVoids)
            {
                MarkVoids();
                Debug.Log($"MarkVoids '{watch.Elapsed.TotalSeconds}' sec");
            }

            HideInvisible();
            watch.Stop();
            Debug.Log($"Draw '{watch.Elapsed.TotalSeconds}' sec");
        }

        public void MarkSky(Vector3Int cell)
        {
            markersRoot = CreateRoot(markersRoot, "markerRoot");
            List<TerrainCell> skyCells = terrainGrid.Wave(cell);

            foreach (TerrainCell c in skyCells)
                AddMark(c);
        }


        public void HideInvisible()
        {
            int n = 0;
            foreach (TerrainCell tCell in terrainGrid.GetAllObjects())
            {
                if (!tCell.block)
                    continue;

                List<TerrainCell> nearCells = terrainGrid.GetNearCells(tCell);
                if (nearCells.Count == 6 && nearCells.All(n => n.density > 0))
                {
                    pool.Return(tCell.block);
                    n++;
                }
            }

            Debug.Log($"'{n}' objects is hidden");
        }

        public void MarkVoids()
        {
            List<TerrainVoid> voids = terrainGrid.FindVoids();

            markersRoot = CreateRoot(markersRoot, "markerRoot");
            foreach (TerrainVoid terrainVoid in voids)
            {
                Color color = Random.ColorHSV(0, 1, 0, 1, 1, 1); //Яркий цвет
                foreach (TerrainCell cell in terrainVoid.cells)
                {
                    AddMark(cell)
                        .SetText(terrainVoid.index.ToString())
                        .SetColor(color);
                }
            }
        }


        Mark AddMark(Vector3Int cell)
        {
            return AddMark(CellToWorldPos(cell));
        }

        Mark AddMark(Vector3 pos)
        {
            GameObject newMarker = Instantiate(fieldSettings.marker, markersRoot);
            newMarker.transform.position = pos;
            newMarker.transform.localScale = Vector3.one;

            return newMarker.GetComponent<Mark>();
        }


        void Clear()
        {
            if (squaresRoot)
                DestroyImmediate(squaresRoot.gameObject);

            if (markersRoot)
                DestroyImmediate(markersRoot.gameObject);
        }

        Transform CreateRoot(Transform oldRoot, string rootName)
        {
            if (oldRoot)
                Destroy(oldRoot.gameObject);

            Transform newRoot = new GameObject(rootName).transform;
            newRoot.parent = transform;
            newRoot.localPosition = Vector3.zero;

            return newRoot;
        }

        void FindGras()
        {
            GameObject grassprefab = blockCollection.GetBlockByName(BlockName.Grass);


            foreach (TerrainCell cell in terrainGrid.FindGras())
            {
                if (terrainGrid[cell].block)
                    pool.Return(terrainGrid[cell].block);

                GameObject grass = pool.Get(grassprefab);
                terrainGrid[cell].block = grass;
                grass.transform.position = CellToWorldPos(cell.cell);
            }
        }

        void GenerateDensityField()
        {
            float offset = 0.5f;
            this.offset = new Vector3(offset, offset, 0);

            terrainGrid = new TerrainGrid(fieldSettings, noiseGenerator);
            terrainGrid.GenerateDensityField();

            squaresRoot = CreateRoot(squaresRoot, "root");

            foreach (TerrainCell terrainCell in terrainGrid.GetAllObjects())
            {
                if (terrainCell.density > 0)
                {
                    GameObject prefab = blockCollection.GetBlockByDensity(terrainCell.density);
                    GameObject block = pool.Get(prefab);
                    block.transform.position = CellToWorldPos(terrainCell);
                    terrainCell.block = block;
                }
            }
        }

        Vector3 CellToWorldPos(Vector3Int c)
        {
            return new Vector3(c.x, c.y, c.z) + transform.position + offset;
        }

        //public bool IsPointInRect(Vector3 point)
        //{
        //    return viewFieldRect.Contains(point);
        //}

        public Vector3Int PointToCell(Vector3 point)
        {
            Vector3 relativePosition = point - transform.position;

            return new Vector3Int(Mathf.FloorToInt(relativePosition.x), Mathf.FloorToInt(relativePosition.y), Mathf.FloorToInt(relativePosition.z));
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (fieldSettings)
                Gizmos.DrawWireCube(Center, fieldSettings.Size);
        }
#endif
    }
}