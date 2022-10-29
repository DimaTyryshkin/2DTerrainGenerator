using System.Collections.Generic;
using System.Diagnostics; 
using NaughtyAttributes;
using SiberianWellness.Common;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using Debug = UnityEngine.Debug;

using Terraria.NoiseGeneration;
using UnityEngine.UIElements;

namespace Terraria
{
	public class NoiseDrawer : MonoBehaviour
	{  
		[SerializeField, IsntNull] NoiseGeneratorAbstract noiseGenerator;  
		[SerializeField, IsntNull] bool markVoids;
		
		[Space]
		[SerializeField, IsntNull] NoiseDrawerSettings settings;

		Transform squaresRoot;
		Transform markersRoot;
		TerrainData terrainData; 
		Vector3 offset;
		Rect viewFieldRect;

		public TerrainData TerrainData => terrainData;
		public Terrain Terrain => settings.terrain;
		public Vector2 Size =>  new Vector3(settings.terrain.Width, settings.terrain.Height);
		public Vector2 Center => (Vector2)transform.position + Size * 0.5f;
		

		private void Start()
		{
			Draw();
		}

		[Button]
		public void Draw()
		{
			Clear();
			
			Stopwatch watch = new Stopwatch();
			
			watch.Start();
			viewFieldRect = new Rect(transform.position, new Vector2(settings.terrain.Width, settings.terrain.Height));
			
			GenerateDensityField();
			FindGras();
			//MarkSky(new Cell(0, terrainData.height - 1));
			
			if(markVoids)
				MarkVoids();
			
			watch.Stop();
			Debug.Log($"Draw '{watch.Elapsed.TotalSeconds}' sec");
		}

		public void MarkSky(Cell cell)
		{
			markersRoot = CreateRoot(markersRoot, "markerRoot");


			TerrainDataOperations operations = new TerrainDataOperations(terrainData);
			List<GridCell<TerrainCell>> skyCells = operations.Wave(cell);

			foreach (GridCell<TerrainCell> c in skyCells)
				AddMark(c.Cell);
		}

		public void MarkVoids()
		{
			TerrainDataOperations operations = new TerrainDataOperations(terrainData);
			List<TerrainVoid> voids = operations.FindVoids();

			markersRoot = CreateRoot(markersRoot, "markerRoot");
			foreach (TerrainVoid terrainVoid in voids)
			{
				Color color = Random.ColorHSV();
				foreach (GridCell<TerrainCell> cell in terrainVoid.cells)
				{
					AddMark(cell.Cell)
						.SetText(terrainVoid.index.ToString())
						.SetColor(color);
				}
			}
		}

		
		Mark AddMark(Cell cell)
		{
			return AddMark(CellToWorldPos(cell));
		}
		
		Mark AddMark(Vector3 pos)
		{
			GameObject newMarker = Instantiate(settings.marker, markersRoot);
			newMarker.transform.position = pos;
			newMarker.transform.localScale = Vector3.one;

			return newMarker.GetComponent<Mark>();
		}


		void Clear()
		{ 
			if (squaresRoot)
				Destroy(squaresRoot.gameObject);
			
			if (markersRoot)
				Destroy(markersRoot.gameObject);
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

		private void FindGras()
		{
			foreach (GridCell<TerrainCell> cell in FindGras(terrainData.AllCells()))
			{
				cell.value.view.SetHeadColor(settings.colorSchema.grassColor);
			}
		}

		List<GridCell<TerrainCell>> FindGras(IEnumerable<GridCell<TerrainCell>> collection)
		{
			List<GridCell<TerrainCell>> result = new List<GridCell<TerrainCell>>(1024);
			foreach (GridCell<TerrainCell> cell in collection)
			{
				if (cell.value.density > 0)
				{
					var up = cell.UpCell;

					if (up != null && up.value.density <= 0)
					{
						result.Add(cell);
					}
				}
			}

			return result;
		}

		private void GenerateDensityField()
		{
			squaresRoot = CreateRoot(squaresRoot, "root");
			float offset = 0.5f;
			this.offset = new Vector3(offset, offset, 0);

			int squaresOnWidth = settings.terrain.Width;
			int squaresOnHeight = settings.terrain.Height;
			terrainData = new TerrainData(squaresOnWidth, squaresOnHeight);

			Cell minCell = new Cell(0, 0);
			Cell maxCell = minCell;
			float minDensity = float.MaxValue;
			float maxDensity = float.MinValue;
			for (int x = 0; x < squaresOnWidth; x++)
			{
				for (int y = 0; y < squaresOnHeight; y++)
				{
					Vector2 noisePoint = settings.terrain.TerrainPointToNoisePoint(x + offset, y + offset);
					float density = noiseGenerator.GetPoint(noisePoint.x, noisePoint.y);

					Cell cell = new Cell(x, y);
					if (density > maxDensity)
					{
						maxCell = cell;
						maxDensity = density;
					}
			
					if (density < minDensity)
					{
						minCell = cell;
						minDensity = density;
					}

					Square square = squaresRoot.InstantiateAsChild(settings.squarePrefab);
					square.transform.position = CellToWorldPos(x, y); 
					terrainData.SetCell(x, y, new TerrainCell(density, square));
				}
			}
			
			// SetColor
			foreach (GridCell<TerrainCell> gridCell in terrainData.AllCells())
			{
				TerrainCell terrainCell = gridCell.value;
				float density = terrainCell.density;
				if (density > 0)
					terrainCell.view.SetColor(settings.colorSchema.ColorFromDensity(density, 0, maxDensity));
				else
					terrainCell.view.Hide();
			}

			Debug.Log($"maxDensity='{maxDensity}'");
		}

		Vector3 CellToWorldPos(int x, int y)
		{
			return new Vector3(x , y , 0) + transform.position + offset;
		}
		
		Vector3 CellToWorldPos(Cell c)
		{
			return new Vector3(c.x , c.y , 0) + transform.position + offset;
		}

		public bool IsPointInRect(Vector2 point)
		{
			return viewFieldRect.Contains(point);
		}

		public Cell PointToCell(Vector2 point)
		{
			Vector2 relativePosition = point - (Vector2)transform.position;
			
			return new Cell(Mathf.FloorToInt(relativePosition.x), Mathf.FloorToInt(relativePosition.y));
		}

#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			if (settings.terrain)
				Gizmos.DrawWireCube(Center, Size);
		}
#endif
	}
}