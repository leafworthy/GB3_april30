using UnityEngine;

public class StatChangePickupEffect : PickupEffect
{
	private StatType statType;
	private bool hasFlashingEffect;
	private UnitStats stats;
	private float changeFactor = 1;
	private float changeFloat;
	private float rate = .3f;
	private float counter;
	private TintHandler tintHandler;
	private Color tintColor;

	public override void StartEffect(UnitStats _stats)
	{
		tintHandler = _stats.GetComponent<TintHandler>();
		stats = _stats;
		var stat = stats.GetStatValue(statType);
		stats.SetStat(statType, stat * changeFactor + changeFloat);
		base.StartEffect(_stats);
	}

	public override void UpdateEffect()
	{
		ApplyFlashingEffect();
		base.UpdateEffect();
	}

	private void ApplyFlashingEffect()
	{
		if (!isRunning) return;
		if (!hasFlashingEffect) return;
		if (counter <= 0)
		{

					tintHandler.StartTint(tintColor);

			counter = rate;
		}
		else
			counter -= Time.deltaTime;
	}

	protected override void StopEffect()
	{
		stats.ResetStat(statType);
		base.StopEffect();
	}

	public StatChangePickupEffect(float _effectDuration, float _changeFactor, Color tint, StatType statType,
	                        bool hasFlashingEffect, float _floatChange = 0f) : base(_effectDuration)
	{
		tintColor = tint;
		this.statType = statType;
		this.hasFlashingEffect = hasFlashingEffect;
		changeFactor = _changeFactor;
		effectDuration = _effectDuration;
		changeFloat = _floatChange;
	}
}
