using System;
using __SCRIPTS;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LineBar : MonoBehaviour
{
	public Color slowBarColor = Color.white;
	public Image FastBar;
	public Image SlowBar;
	Gradient barGradient;
	float targetFill;
	const float smoothingFactor = .25f;
	const float showBarFraction = .9f;


	public void UpdateBar(float fraction)
	{
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
