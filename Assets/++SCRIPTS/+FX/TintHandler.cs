using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TintHandler : MonoBehaviour
{
	private List<Renderer> renderersToTint = new List<Renderer>();
	private Color materialTintColor;
	private const float tintFadeSpeed = 6f;
	private static readonly int Tint = Shader.PropertyToID("_Tint");
	private Life life;

	
	public void OnEnable()
	{
		renderersToTint = GetComponentsInChildren<Renderer>().ToList();
		life = GetComponent<Life>();
		if (life == null) return;
		life.OnDamaged += Life_Damaged;
	}

	private void Life_Damaged(Attack attack)
	{
		StartTint(attack.color);
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
