using System.Collections.Generic;
using UnityEngine;

public class TintHandler : MonoBehaviour
{
	public List<Renderer> renderersToTint = new List<Renderer>();
	private Color materialTintColor;
	private float tintFadeSpeed = 6f;
	private static readonly int Tint = Shader.PropertyToID("_Tint");

	public void StartTint(Color tintColor)
	{
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
		if (!(materialTintColor.a > 0)) return;
		materialTintColor.a = Mathf.Clamp01(materialTintColor.a - tintFadeSpeed * Time.deltaTime);
		foreach (var r in renderersToTint)
		{
			r.material.SetColor(Tint, materialTintColor);
		}
	}
}
