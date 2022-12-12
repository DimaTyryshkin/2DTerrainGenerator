using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SiberianWellness.RunTimeSpriteRenderer
{
	/// <summary>
	/// Буфер отрендеренных картинок/
	/// Позволяет наделать картинок для переодевалки и хранить их, а потом все разом удалить когда надо.
	/// </summary>
	public class RunTimeSpriteBuffer
	{
		readonly RunTimeSpriteRenderer renderer;
		readonly List<Sprite> renderedSprites = new List<Sprite>();

		public RunTimeSpriteBuffer(RunTimeSpriteRenderer renderer)
		{
			Assert.IsNotNull(renderer);
            
			this.renderer = renderer;
		}

		public Sprite Render()
		{
			//TODO было бы круто сделать глобальный счетчик отрендереных текстур и в редакторе чекать что они все удалились
			var sprite = renderer.RenderSprite();
			renderedSprites.Add(sprite);

			var mainCamera = Camera.main;
			mainCamera.enabled = false;
			mainCamera.enabled = true;
			return sprite;
		}

		public void DeleteRenderedSprites()
		{
			foreach (var sprite in renderedSprites)
			{
				if (sprite)
				{
					if (sprite.texture)
					{
						Object.Destroy(sprite.texture);
						Object.Destroy(sprite);
					}
				}
			}
			
			renderedSprites.Clear();
		}
	}
}