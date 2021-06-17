using System.Collections.Generic;
using System.Linq;
using _SCRIPTS;
using UnityEngine;

public class MoveSpeedEffect : PickupEffect
{
	private float originalSpeed;
	private UnitStats stats;
	private float speedFactor = 1.5f;
	private float rate = .3f;
	private float counter = 0;
	private List<HitTintingFX> hitTintingFX;
	private Color tintColor;

	public override void StartEffect(UnitStats _stats)
	{
		base.StartEffect(_stats);
		hitTintingFX = _stats.GetComponents<HitTintingFX>().ToList();

		stats = _stats;
		originalSpeed = stats.moveSpeed;
		stats.moveSpeed = stats.moveSpeed *speedFactor;
	}

	public override bool UpdateEffect()
	{

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

		return base.UpdateEffect();
	}

	protected override void StopEffect()
	{
		stats.ResetSpeed();
	}

	public MoveSpeedEffect(float _effectDuration, float _speedFactor, Color tint) : base(_effectDuration)
	{
		tintColor = tint;
		speedFactor = _speedFactor;
		effectDuration = _effectDuration;
	}
}
