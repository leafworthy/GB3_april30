using System;
using __SCRIPTS;
using UnityEngine;

public class KnifeAttack : Ability
{
	public override string AbilityName => "KnifeAttack";

	bool isAttacking;
	bool isPressing;
	public GameObject attackPoint;

	public event Action OnMiss;
	public event Action<Vector2> OnHit;
	JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
	JumpAbility _jumpAbility;

	[SerializeField] AnimationClip animationClip;

	public DamageOverTimeData damageOverTimeData;

	public override bool requiresArms() => true;
	public override bool requiresLegs() => false;

	public override bool canDo() => jumpAbility.IsResting && base.canDo();

	protected override void DoAbility()
	{
		StartAttack();
	}

	public override void SetPlayer(Player newPlayer)
	{
		base.SetPlayer(newPlayer);
		player = newPlayer;

		ListenToPlayer();
	}

	void ListenToPlayer()
	{
		player.Controller.Attack3Circle.OnPress += PlayerKnifePress;
		player.Controller.Attack3Circle.OnRelease += PlayerKnifeRelease;
	}

	void StopListeningToPlayer()
	{
		if (player.Controller == null) return;
		if (player.Controller.Attack3Circle == null) return;
		player.Controller.Attack3Circle.OnPress -= PlayerKnifePress;
		player.Controller.Attack3Circle.OnRelease -= PlayerKnifeRelease;
	}

	void OnDisable()
	{
		if (player == null) return;
		if (anim == null) return;
		StopListeningToPlayer();
	}

	protected override void AnimationComplete()
	{
		anim.SetBool(UnitAnimations.IsBobbing, true);
		isAttacking = false;
		base.AnimationComplete();
		if (!isPressing) return;

		PlayerKnifePress(null);
	}

	void PlayerKnifeRelease(NewControlButton newControlButton)
	{
		isPressing = false;
	}

	void PlayerKnifePress(NewControlButton newControlButton)
	{
		isPressing = true;
		TryToDoAbility();
	}

	public override void StopAbilityBody()
	{
		isAttacking = false;

		if (lastArmAbility != null)
		{
			base.StopAbilityBody();
			lastArmAbility?.Resume();
			lastArmAbility?.TryToDoAbility();
		}
		else
			base.StopAbilityBody();
	}

	void StartAttack()
	{
		if (isAttacking) return;
		isAttacking = true;
		PlayAnimationClip(animationClip, 1);
		Invoke(nameof(Anim_AttackHit), .1f);
	}

	void Anim_AttackHit()
	{
		var targetHit = MyAttackUtilities.FindClosestHit(attacker, attackPoint.transform.position, attacker.stats.Stats.Range(3), attacker.EnemyLayer);
		if (targetHit == null)
		{
			OnMiss?.Invoke();
			return;
		}

		var targetLife = targetHit.transform.gameObject.GetComponentInParent<Life>();
		if (targetLife == null) return;
		var KnifeFireEffect = targetLife.gameObject.AddComponent<DamageOverTimeEffect>();
		KnifeFireEffect.StartEffect(attacker, targetLife, damageOverTimeData.fireDuration, damageOverTimeData.fireDamageRate,
			damageOverTimeData.fireDamageAmount, damageOverTimeData.fireColor);
		MyAttackUtilities.HitTarget(attacker, targetLife, 0); //stats.TertiaryAttackDamageWithExtra
		OnHit?.Invoke(targetHit.transform.position);
	}
}
