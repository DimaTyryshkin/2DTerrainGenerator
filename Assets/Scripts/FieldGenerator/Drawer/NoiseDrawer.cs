using System.Collections.Generic;
using System.Diagnostics;
using FieldGenerator;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

using NaughtyAttributes;
using SiberianWellness.NotNullValidation;
using SiberianWellness.Common;
using FieldGenerator.Terraria.NoiseGeneration;
using TerrainData = FieldGenerator.TerrainData;

namespace FieldGenerator
{
	class ViewCell : GridCell
	{
		public Square view;
 
		public ViewCell(Cell c, Square view) : base(c)
		{
			Assert.IsNotNull(view);
			this.view = view;
		}
	}

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
		Grid<ViewCell> viewGrid;

		public TerrainData TerrainData => terrainData;
		public NoiseField NoiseField=> settings.noiseField;
		public Vector2 Size =>  new Vector3(settings.noiseField.Width, settings.noiseField.Height);
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
			viewFieldRect = new Rect(transform.position, new Vector2(settings.noiseField.Width, settings.noiseField.Height));
			
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
			List<TerrainCell> skyCells = terrainData.Wave(cell);

			foreach (TerrainCell c in skyCells)
				AddMark(c);
		}

		public void MarkVoids()
		{
			List<TerrainVoid> voids = terrainData.FindVoids();

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
			viewGrid = null;
			
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

		void FindGras()
		{
			foreach (TerrainCell cell in terrainData.FindGras())
				viewGrid[cell].view.SetHeadColor(settings.colorSchema.grassColor);
		}

		void GenerateDensityField()
		{
			float offset = 0.5f;
			this.offset = new Vector3(offset, offset, 0);

			terrainData = new TerrainData(settings.noiseField, noiseGenerator);
			terrainData.GenerateDensityField();
			viewGrid = new Grid<ViewCell>(terrainData.width, terrainData.height);
			squaresRoot = CreateRoot(squaresRoot, "root");
			
			foreach (TerrainCell terrainCell in terrainData.AllCells())
			{
				Square square = squaresRoot.InstantiateAsChild(settings.squarePrefab);
				square.transform.position = CellToWorldPos(terrainCell);
				viewGrid.SetCell(terrainCell, new ViewCell(terrainCell, square));

				if (terrainCell.density > 0)
					square.SetColor(settings.colorSchema.ColorFromDensity(terrainCell.density, 0, terrainData.MaxDensity));
				else
					square.Hide();
			}
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
			if (settings.noiseField)
				Gizmos.DrawWireCube(Center, Size);
		}
#endif
	}
}