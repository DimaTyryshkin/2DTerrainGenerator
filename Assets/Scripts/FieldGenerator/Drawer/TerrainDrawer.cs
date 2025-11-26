using FieldGenerator.Terraria.NoiseGeneration;
using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using Terraria;
using UnityEngine;

namespace FieldGenerator
{


    class TerrainDrawer : MonoBehaviour
    {
        [SerializeField, IsntNull] BlockCollection blockCollection;
        [SerializeField, IsntNull] CameraStartPosition cameraStartPosition;
        [SerializeField, IsntNull] bool markVoids;

        [Space]
        [SerializeField, IsntNull] TerrainCommon fieldSettings;
        [SerializeField, IsntNull] ChunkGenerator terrainGenerator;

        [Header("Debug")]
        [SerializeField] Vector3Int cell;
        [SerializeField] BlockName blockName;
        [SerializeField] bool isGpuInstancing;
        [SerializeField] int gpuGroupSize;

        DynamicObjectPool pool;
        GpuInstancer gpuInstancer;
        Transform squaresRoot;
        Transform markersRoot;
        TerrainGrid terrainGrid;
        Vector3 offset;
        Rect viewFieldRect;

        public TerrainGrid TerrainGrid => terrainGrid;
        public TerrainCommon FieldSettings => fieldSettings;
        public Vector3 Center => transform.position + (Vector3)fieldSettings.Size * 0.5f;

        private void Start()
        {
            //Init();
        }

        private void Update()
        {
            //if (isGpuInstancing)
            //    gpuInstancer.Draw();
        }

        public void Init()
        {
            float offset = 0.5f;
            this.offset = new Vector3(offset, offset, offset);
            pool = DynamicObjectPool.GetInst();
            //gpuInstancer = GpuInstancer.GetInst(gpuGroupSize);

            //DrawTerrain();

            cameraStartPosition.UpdatePosition();
        }

        //[Button]
        //public void DrawTerrain()
        //{
        //    Stopwatch watch = new Stopwatch();
        //    watch.Start();
        //    ClearTerrain();
        //    Debug.Log($"Clear '{watch.Elapsed.TotalSeconds}' sec");
        //    viewFieldRect = new Rect(transform.position, new Vector2(fieldSettings.Size.x, fieldSettings.Size.y));

        //    terrainGrid = new TerrainGrid(fieldSettings.Size);
        //    terrainGenerator.Generate(terrainGrid);
        //    squaresRoot = CreateRoot(squaresRoot, "root");

        //    //Body
        //    {
        //        DrawForGame();
        //    }

        //    watch.Stop();
        //    Debug.Log($"Draw '{watch.Elapsed.TotalSeconds}' sec");

        //    gpuInstancer.LogObjectsAmount();
        //}

        //public void MarkSky(Vector3Int cell)
        //{
        //    markersRoot = CreateRoot(oldRoot: markersRoot, "markerRoot");
        //    List<TerrainItem> skyCells = terrainGenerator.Wave(cell);

        //    foreach (TerrainItem c in skyCells)
        //        AddMark(c);
        //}


        //public void HideInvisible()
        //{
        //    int n = 0;

        //    foreach (TerrainItem tCell in terrainGrid.items)
        //    {
        //        if (!tCell.block)
        //            continue;

        //        terrainGrid.GetNearIndexes(tCell, ref nearIndexes);
        //        if (nearIndexes.Count == 6 && nearIndexes.All(n => terrainGrid.items[n].density > 0))
        //        {
        //            pool.Return(tCell.block);
        //            n++;
        //        }
        //    }

        //    Debug.Log($"'{n}' objects is hidden");
        //}



        //public void MarkVoids()
        //{
        //    List<TerrainVoid> voids = terrainGenerator.FindVoids();

        //    markersRoot = CreateRoot(markersRoot, "markerRoot");
        //    foreach (TerrainVoid terrainVoid in voids)
        //    {
        //        Color color = Random.ColorHSV(0, 1, 0, 1, 1, 1); //Яркий цвет
        //        foreach (TerrainItem cell in terrainVoid.cells)
        //        {
        //            AddMark(cell)
        //                .SetText(terrainVoid.index.ToString())
        //                .SetColor(color);
        //        }
        //    }
        //}


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


        [Button]
        void ClearTerrain()
        {
            if (isGpuInstancing)
            {
                gpuInstancer.RemoveAll();
            }
            else
            {
                pool.ReturnAll();

                if (squaresRoot)
                    DestroyImmediate(squaresRoot.gameObject);

                if (markersRoot)
                    DestroyImmediate(markersRoot.gameObject);
            }
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

        //[Button]
        void SpawnBlock()
        {
            GameObject prefab = blockCollection.GetBlockByName(blockName).prefab;
            //InstatniateBlock(cell, prefab);
        }
#endif
    }
}