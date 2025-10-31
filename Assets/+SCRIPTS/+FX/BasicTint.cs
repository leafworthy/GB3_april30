using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class BasicTint : MonoBehaviour
	{
		private float TintFadeSpeed = 6f;
		private List<Renderer> renderersToTint => _renderersToTint ??= GetComponentsInChildren<Renderer>(true).ToList();
		private List<Renderer> _renderersToTint;
		private Color materialTintColor;
		private static readonly int Tint = Shader.PropertyToID("_Tint");
		private Player player;
		public Color DebreeTint = Color.red;

		public void StartTint(Color tintColor)
		{
			materialTintColor = new Color();
			materialTintColor = tintColor;
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
		}

		public void TintDebreeColor(GameObject debreeObject)
		{

			var spriteToTint = debreeObject.GetComponentInChildren<SpriteRenderer>();
			if (spriteToTint != null)
				spriteToTint.color = DebreeTint;
		}
		private void Update()
		{
			FadeOutTintAlpha();
		}

		private void FadeOutTintAlpha()
		{
			if (materialTintColor.a < 0) return;
			materialTintColor.a = Mathf.Clamp01(materialTintColor.a - TintFadeSpeed * Time.deltaTime);
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
		}

	}
}
