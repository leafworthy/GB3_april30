using System;
using System.Linq;
using __SCRIPTS;
using UnityEngine;

public class DoableKnifeAttack : Ability
{
	public override string VerbName => "Knife-Attack";

	private bool isAttacking;
	private bool isPressing;
	public GameObject attackPoint;
	public event Action OnMiss;
	public event Action<Vector2> OnHit;
	private DoableJumpAbility jumpAbility => _jumpAbility ??=GetComponent<DoableJumpAbility>();
	private DoableJumpAbility _jumpAbility;
	private UnitAttackManager unitAttackManager => _unitAttackManager ??= GetComponent<UnitAttackManager>();
	private UnitAttackManager _unitAttackManager;



	[SerializeField]private AnimationClip animationClip;

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

	private void OnDisable()
	{
		if (player == null) return;
		if (anim == null) return;
		StopListeningToPlayer();
	}

	private void StopListeningToPlayer()
	{
		player.Controller.Attack3Circle.OnPress -= PlayerKnifePress;
		player.Controller.Attack3Circle.OnRelease -= PlayerKnifeRelease;
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

	private void StartAttack()
	{
		Debug.Log("starting attack");
		if (isAttacking) return;
		isAttacking = true;
		PlayAnimationClip(animationClip,1);
		Invoke(nameof(Anim_AttackHit), 0);
		anim.SetBool(UnitAnimations.IsBobbing, false);
	}

	private GameObject FindClosestHit()
	{
		var circleCast = Physics2D.OverlapCircleAll(attackPoint.transform.position, unitAttackManager.life.TertiaryAttackRange, assetManager.LevelAssets.EnemyLayer).ToList();
		if (circleCast.Count <= 0) return null;

		var closest = circleCast[0];
		foreach (var col in circleCast)
		{
			var colStats = col.GetComponentInChildren<Life>();
			if (colStats.IsObstacle || !colStats.IsPlayerAttackable) continue;
			if (colStats.cantDie) continue;
			if (Vector2.Distance(col.gameObject.transform.position, transform.position) < Vector2.Distance(closest.transform.position, transform.position))
				closest = col;
		}

		return closest.gameObject;
	}

	private void Anim_AttackHit()
	{

		var enemyHit = FindClosestHit();
		if (enemyHit == null)
		{
			Debug.Log("attack miss");
			OnMiss?.Invoke();
			return;
		}


		var enemy = enemyHit.transform.gameObject.GetComponent<Life>();
		if (enemy == null) enemy = enemyHit.transform.gameObject.GetComponentInParent<Life>();
		if (enemy == null || enemy.IsPlayer || enemy.cantDie || enemy.IsObstacle)
		{
			if (enemy == null ) Debug.Log(" attack hit null enemy");
			if(enemy.IsPlayer) Debug.Log(" attack hit player");
			if(enemy.cantDie) Debug.Log(" attack hit cant die", enemy);
			if(enemy.IsObstacle) Debug.Log(" attack hit obstacle");
			Debug.Log("attack miss");
			OnMiss?.Invoke();
			return;
		}

		Debug.Log("attack hit");
		OnHit?.Invoke(enemyHit.transform.position);

		unitAttackManager.HitTarget(unitAttackManager.life.TertiaryAttackDamageWithExtra, enemy, 2);
	}
}
