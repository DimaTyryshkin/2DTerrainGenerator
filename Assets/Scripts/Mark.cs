using TMPro;
using UnityEngine;

namespace Terraria
{
	public class Mark : MonoBehaviour
	{
		[SerializeField] TextMeshPro msgText; 
		[SerializeField] SpriteRenderer thisRenderer;

		public Mark SetColor(Color color)
		{
			gameObject.SetActive(true);
			thisRenderer.color = color;
			return this;
		}

		public Mark SetText(string text)
		{
			msgText.text = text;
			msgText.gameObject.SetActive(!string.IsNullOrEmpty(text));

			return this;
		}
	}
}