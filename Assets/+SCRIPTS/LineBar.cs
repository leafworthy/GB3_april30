using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable, ExecuteInEditMode]
public class LineBar : MonoBehaviour
{
	public Color slowBarColor = Color.white;
	public Image FastBar;
	public Image SlowBar;
	Gradient barGradient;
	public float targetFill;
	const float smoothingFactor = .25f;
	const float showBarFraction = .9f;
	public bool HideWhenAboveFraction = true;
	public bool useGradientColor;

	void Start()
	{
		barGradient = Services.assetManager.FX.healthbarGradient;
	}

	public void UpdateBar(float fraction)
	{
		Debug.Log("Updating LineBar to fraction: " + fraction, this);
		targetFill = fraction;
		if (HideWhenAboveFraction && fraction > showBarFraction) gameObject.SetActive(false);

			gameObject.SetActive(true);
			SlowBar.color = slowBarColor;
			SlowBar.fillAmount = Mathf.Lerp(SlowBar.fillAmount, targetFill, smoothingFactor);
			FastBar.fillAmount = targetFill;


		UpdateBarFill();
	}

	public void Update()
	{
		UpdateBarFill();
	}

	void UpdateBarFill()
	{
		if (useGradientColor)
		{
			barGradient = Services.assetManager.FX.healthbarGradient;
			FastBar.color = barGradient.Evaluate(targetFill);
		}
		if (SlowBar != null && targetFill < SlowBar.fillAmount)
		{
			if (SlowBar != null) SlowBar.fillAmount = Mathf.Lerp(SlowBar.fillAmount, targetFill, smoothingFactor);
			if (FastBar != null) FastBar.fillAmount = targetFill;
		}
		else
		{
			if (SlowBar != null) FastBar.fillAmount = Mathf.Lerp(SlowBar.fillAmount, targetFill, smoothingFactor);
			if (FastBar != null) SlowBar.fillAmount = targetFill;
		}
	}
}
