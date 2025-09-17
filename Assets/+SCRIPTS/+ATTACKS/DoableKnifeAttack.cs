using System;
using System.Linq;
using __SCRIPTS;
using UnityEngine;

public class DoableKnifeAttack : Ability
{
	public override string AbilityName => "KnifeAttack";

	private bool isAttacking;
	private bool isPressing;
	public GameObject attackPoint;

	public event Action OnMiss;
	public event Action<Vector2> OnHit;
	private CharacterJumpAbility jumpAbility => _jumpAbility ??= GetComponent<CharacterJumpAbility>();
	private CharacterJumpAbility _jumpAbility;

	[SerializeField] private AnimationClip animationClip;

	protected override bool requiresArms() => true;
	protected override bool requiresLegs() => false;

	public override bool canDo() => jumpAbility.IsResting && base.canDo();

	protected override void DoAbility()
	{
		StartAttack();
	}

	public override void SetPlayer(Player _player)
	{
		base.SetPlayer(_player);
		player = _player;

		ListenToPlayer();
	}

	private void ListenToPlayer()
	{
		player.Controller.Attack3Circle.OnPress += PlayerKnifePress;
		player.Controller.Attack3Circle.OnRelease += PlayerKnifeRelease;
	}

	private void StopListeningToPlayer()
	{
		player.Controller.Attack3Circle.OnPress -= PlayerKnifePress;
		player.Controller.Attack3Circle.OnRelease -= PlayerKnifeRelease;
	}
	private void OnDisable()
	{
		if (player == null) return;
		if (anim == null) return;
		StopListeningToPlayer();
	}



	protected override void AnimationComplete()
	{
		base.AnimationComplete();
		anim.SetBool(UnitAnimations.IsBobbing, true);
		isAttacking = false;
		if (!isPressing) return;

		PlayerKnifePress(null);
	}

	private void PlayerKnifeRelease(NewControlButton newControlButton)
	{
		isPressing = false;
	}

	private void PlayerKnifePress(NewControlButton newControlButton)
	{
		Debug.Log("[Knife] Player pressed knife button");
		isPressing = true;
		Do();
	}

	public override void Stop()
	{
		base.Stop();
		isAttacking = false;
	}

	private void StartAttack()
	{
		Debug.Log("starting attack");
		if (isAttacking) return;
		isAttacking = true;
		PlayAnimationClip(animationClip, 1);
		Invoke(nameof(Anim_AttackHit), .1f);
		anim.SetBool(UnitAnimations.IsBobbing, false);
	}



	private void Anim_AttackHit()
	{
		Debug.Log("knife attack hit");
		var targetHit = AttackUtilities.FindClosestHit( life,attackPoint.transform.position, life.TertiaryAttackRange, life.EnemyLayer);
		if (targetHit == null)
		{
			OnMiss?.Invoke();
			return;
		}

		var targetLife = targetHit.transform.gameObject.GetComponentInParent<Life>();
		if (targetLife == null)
		{
			Debug.Log("no life");
			return;
		}
		Debug.Log("attack hit");
		AttackUtilities.HitTarget( life, targetLife, life.TertiaryAttackDamageWithExtra);
		OnHit?.Invoke(targetHit.transform.position);
	}
}
