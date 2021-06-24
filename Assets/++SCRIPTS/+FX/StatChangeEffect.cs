using System.Collections.Generic;
using System.Linq;
using _SCRIPTS;
using UnityEngine;

public class StatChangeEffect : PickupEffect
{
	private StatType statType;
	private bool hasFlashingEffect;
	private float originalValue;
	private UnitStats stats;
	private float changeFactor = 1;
	private float changeFloat;
	private float rate = .3f;
	private float counter = 0;
	private List<HitTintingFX> hitTintingFX;
	private Color tintColor;

	public override void StartEffect(UnitStats _stats)
	{
		base.StartEffect(_stats);
		hitTintingFX = _stats.GetComponents<HitTintingFX>().ToList();

		stats = _stats;
		var stat = stats.GetStat(statType);
		stats.SetStat(statType, stat *changeFactor+changeFloat);
	}

	public override bool CanUpdateEffect()
	{
		if (!hasFlashingEffect) return false;
		if (counter <= 0)
		{
			Debug.Log("tint");
			if (hitTintingFX.Count > 0)
			{
				foreach (var fx in hitTintingFX)
				{
					Debug.Log("tinting");
					fx.SetTintColor(tintColor);

				}

			}

			counter = rate;
		}
		else
		{
			counter -= Time.deltaTime;
		}

		return base.CanUpdateEffect();
	}

	protected override void StopEffect()
	{
		stats.ResetStat(statType);
	}

	public StatChangeEffect(float _effectDuration, float _changeFactor, Color tint, StatType statType, bool hasFlashingEffect, float _floatChange = 0f) : base(_effectDuration)
	{
		tintColor = tint;
		this.statType = statType;
		this.hasFlashingEffect = hasFlashingEffect;
		changeFactor = _changeFactor;
		effectDuration = _effectDuration;
		changeFloat = _floatChange;
	}
}
