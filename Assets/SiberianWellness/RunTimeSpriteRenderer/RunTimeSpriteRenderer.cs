using UnityEngine;
using UnityEngine.Assertions;

namespace SiberianWellness.RunTimeSpriteRenderer
{ 
	public class RunTimeSpriteRenderer
	{
		readonly Camera     camera;
		readonly int        width;
		readonly int        height;
		readonly Vector3    originCameraPos;
		readonly Quaternion originCameraRot;

		public RunTimeSpriteRenderer(Camera camera, int width, int height)
		{
			Assert.IsNotNull(camera); 
			
			this.camera = camera;
			this.width = width;
			this.height = height;

			originCameraPos = camera.transform.position;
			originCameraRot = camera.transform.rotation;
		}

		public void SetCameraPos(Transform cameraPos)
		{
			Assert.IsNotNull(cameraPos);
			camera.transform.position = cameraPos.position;
			camera.transform.rotation = cameraPos.rotation;
		}
		
		public void SetCameraPos(Vector3 pos)
		{
			camera.transform.position = pos;
		}

		public void ResetCameraPos()
		{
			camera.transform.position = originCameraPos;
			camera.transform.rotation = originCameraRot;
		}

		public Sprite RenderSprite()
		{ 
			//place.PlaceView(furniture.gameObject);
			//var txt    = CaptureScreenshot.SimpleCaptureTransparentScreenshot(camera, 419, 512);
			var sprite = CaptureScreenshot.RenderSimpleSprite(camera, width, height);
			return sprite;
		}
	}
}