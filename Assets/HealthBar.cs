using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HealthBar : MonoBehaviour
{
	public Color slowBarColor = Color.white;
	public Image FastBar;
	public Image SlowBar;
	[SerializeField] private Gradient barGradient;

	private float targetFill;
	private const float smoothingFactor = .25f;
	private float showBarFraction = .9f;

	public void UpdateHealthBar(float fraction)
	{
		showBarFraction = .9f;
		targetFill = fraction;
		if (fraction > showBarFraction) gameObject.SetActive(false);
		else
		{
			barGradient = Services.assetManager.FX.healthbarGradient;
			gameObject.SetActive(true);
			FastBar.color = barGradient.Evaluate(fraction);
			SlowBar.color = slowBarColor;
			SlowBar.fillAmount = Mathf.Lerp(SlowBar.fillAmount, targetFill, smoothingFactor);
			FastBar.fillAmount = targetFill;
		}
	}
}
