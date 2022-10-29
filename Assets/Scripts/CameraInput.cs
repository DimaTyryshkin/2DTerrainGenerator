using FieldGenerator;
using SiberianWellness.Common;
using UnityEngine;

namespace Terraria
{
	public class CameraInput : MonoBehaviour
	{
		[SerializeField] Camera thisCamera;
		[SerializeField] NoiseDrawer noiseDrawer;
        
		void Update()
		{ 
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 point = thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XY);
				Cell c= noiseDrawer.PointToCell(point);
				Debug.Log(c);

				if (noiseDrawer.TerrainData.IsCellExist(c))
					noiseDrawer.MarkSky(c);
			} 
		}
	}
}