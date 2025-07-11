using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GangstaBean.Core;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class Tint_FX : MonoBehaviour, IPoolable
	{
		[Header("Color Options")] public float TintFadeSpeed = 6f;

		private List<Renderer> renderersToTint = new();
		private Color materialTintColor;
		private static readonly int Tint = Shader.PropertyToID("_Tint");

		public void OnEnable()
		{
			renderersToTint = GetComponentsInChildren<Renderer>(true).ToList();
		}

		public void StartTint(Color tintColor)
		{
			materialTintColor = new Color();
			materialTintColor = tintColor;
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
		}

		public void TintIt()
		{
			renderersToTint = GetComponentsInChildren<Renderer>(true).ToList();
			Color tintColor = new Color(1, 0, 0, 1);
			materialTintColor = new Color();
			materialTintColor = tintColor;
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
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

		public void OnPoolSpawn()
		{
			// Reset tint to no tint (transparent) when spawning from pool
			materialTintColor = new Color(1, 1, 1, 1);
			OnEnable();
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
		}

		public void OnPoolDespawn()
		{
			// Nothing needed when despawning
		}
	}
}
