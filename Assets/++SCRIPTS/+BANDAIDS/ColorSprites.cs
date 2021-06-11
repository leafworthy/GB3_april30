using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _SCRIPTS
{
	[ExecuteInEditMode]
	public class ColorSprites : MonoBehaviour
	{
		[SerializeField] public float currentAlpha = 1;
		[SerializeField] private Color primaryColor;
		[SerializeField] private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
		[SerializeField] private List<Image> images = new List<Image>();

		[Button]
		private void GetSprites()
		{
			spriteRenderers.Clear();
			foreach (var sr in GetComponentsInChildren<SpriteRenderer>()) spriteRenderers.Add(sr);

			foreach (var sr in GetComponents<SpriteRenderer>()) spriteRenderers.Add(sr);
			foreach (var sr in GetComponents<Image>()) images.Add(sr);
		}


		[Button]
		public void TintAll()
		{
			Debug.Log("tint");
			GetSprites();
			foreach (var sr in spriteRenderers)
				if (sr.CompareTag("dontcolor"))
					sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, currentAlpha);
				else
					sr.color = new Color(primaryColor.r, primaryColor.g, primaryColor.b, currentAlpha);

			foreach (var sr in images)
				if (sr.CompareTag("dontcolor"))
					sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, currentAlpha);
				else
					sr.color = new Color(primaryColor.r, primaryColor.g, primaryColor.b, currentAlpha);
		}

		public void SetColor(Color newColor)
		{
			primaryColor = newColor;
			TintAll();
		}
	}
}
