using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Life_FX : IHaveInspectorColor
{
	public override Color GetBackgroundColor() => Colors.Red;
	public override string GetIconPath() => "Assets/Skull_Icon.png";

	public Image slowBarImage;
	public Image fastBarImage;
	public Color DebreeTint = Color.white;
	private List<Renderer> renderersToTint = new();
	private Color materialTintColor;
	private const float tintFadeSpeed = 6f;
	private static readonly int Tint = Shader.PropertyToID("_Tint");

	public enum ColorMode
	{
		Single,
		Gradient
	}

	public ColorMode colorMode;
	public Color slowBarColor = Color.white;
	public Gradient barGradient = new();

	private float targetFill;
	private float smoothingFactor = .25f;
	private Life _life;
	private Color PlayerColor;
	public GameObject healthBar;

	public void OnEnable()
	{
		renderersToTint = GetComponentsInChildren<Renderer>().ToList();
		_life = GetComponentInParent<Life>();
		if (_life == null) return;
		_life.OnDamaged += Life_Damaged;
		_life.OnFractionChanged += DefenceOnDefenceChanged;
		_life.OnDying += DefenceOnDead;
		if (_life.unitData.ShowLifeBar) return;
		if (healthBar != null) healthBar.SetActive(false);
	}



	public void OnDisable()
	{
		if (_life == null) return;
		_life.OnDamaged -= Life_Damaged;
		_life.OnFractionChanged -= DefenceOnDefenceChanged;
		_life.OnDying -= DefenceOnDead;
	}

	public void StartTint(Color tintColor)
	{
		Debug.Log("tint started ", this);
		materialTintColor = new Color();
		materialTintColor = tintColor;
		foreach (var r in renderersToTint)
		{
			r.material.SetColor(Tint, materialTintColor);
		}
	}

	[Button]
	public void StartTintRed()
	{
		Debug.Log("tint started ", this);
		materialTintColor = Color.red;
		foreach (var r in renderersToTint)
		{
			r.material.SetColor(Tint, materialTintColor);
		}
	}

	private void Life_Damaged(Attack attack)
	{
		StartTint(attack.color);
	}

	private void DefenceOnDefenceChanged(float newAmount)
	{
		UpdateBarFill();
	}

	private void DefenceOnDead(Player player, Life life)
	{
		_life.OnFractionChanged -= DefenceOnDefenceChanged;
		_life.OnDying -= DefenceOnDead;
	}

	private void UpdateGradient()
	{
		var time = _life == null ? targetFill : _life.GetFraction();
		if (colorMode == ColorMode.Gradient)
			fastBarImage.color = barGradient.Evaluate(time);
	}

	#region PUBLIC FUNCTIONS

	public void UpdateBar(float currentValue, float maxValue)
	{
		if (slowBarImage == null)
			return;
		targetFill = currentValue / maxValue;
		UpdateGradient();
		UpdateBarFill();
	}

	private void UpdateBarFill()
	{
		if (_life == null) return;
		if (healthBar == null) return;

		if (!_life.unitData.ShowLifeBar) return;
		targetFill = _life.GetFraction();
		if (targetFill > .9f)
			healthBar.SetActive(false);
		else
		{
			healthBar.SetActive(true);
			if (slowBarImage != null) slowBarImage.fillAmount = Mathf.Lerp(slowBarImage.fillAmount, targetFill, smoothingFactor);
			if (fastBarImage != null) fastBarImage.fillAmount = targetFill;
		}
	}

	private void Update()
	{
		UpdateColor(slowBarColor);
		UpdateColor(barGradient);
		UpdateBarFill();
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

	private void UpdateColor(Color targetColor)
	{
		if (colorMode != ColorMode.Single || slowBarImage == null)
			return;
		slowBarColor = targetColor;
		slowBarImage.color = slowBarColor;
	}

	private void UpdateColor(Gradient targetGradient)
	{
		if (colorMode != ColorMode.Gradient || slowBarImage == null)
			return;

		barGradient = targetGradient;
		UpdateGradient();
	}

	#endregion
}