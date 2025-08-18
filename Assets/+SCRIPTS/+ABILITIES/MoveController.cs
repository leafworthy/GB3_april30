using System;
using __SCRIPTS._ENEMYAI;
using __SCRIPTS.Cursor;
using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

public interface IControlMove
{
	event Action<Vector2> OnMoveInDirection;
	event Action OnStopMoving;
}

public interface IMove
{
	bool IsMoving();
}

namespace __SCRIPTS
{
	public class MoveController : AbilityController
	{
		public Vector2 MoveDir { get; private set; }
		private DoableMoveAbility ability  => _ability ??= GetComponent<DoableMoveAbility>();
		public bool CanMove=> ability.CanMove;
		private DoableMoveAbility _ability;

		private bool isWounded;
		private IControlMove mover;

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			mover = _player.SpawnedPlayerGO.GetComponent<IControlMove>();
			life.OnDying += Life_OnDying;
		}

		private void Life_OnDying(Player arg1, Life arg2)
		{
			ability.CanMove = false;
			ability.StopMoving();
			ability.StopPushing();
		}

		protected override void ListenToPlayer()
		{
			mover.OnMoveInDirection += Mover_MoveInDirection;
			anim.animEvents.OnUseLegs += Anim_UseLegs;
			anim.animEvents.OnStopUsingLegs += Anim_StopUsingLegs;
			anim.animEvents.OnDashStop += Anim_DashStop;
			anim.animEvents.OnRecovered += Anim_Recovered;
		}

		private void Mover_MoveInDirection(Vector2 newDirection)
		{
			if (pauseManager.IsPaused) return;
			if (isWounded) return;
			if (!ability.CanMove) return;

			if (newDirection.magnitude < .5f)
			{
				StopMoving();
				return;
			}

			StartMoving(newDirection);
		}

		protected override void StopListeningToPlayer()
		{
			player.Controller.OnMoveAxis_Change -= Player_MoveInDirection;
			player.Controller.OnMoveAxis_Inactive -= Player_StopMoving;

			mover.OnMoveInDirection -= AI_MoveInDirection;
			mover.OnStopMoving -= AI_StopMoving;

			life.OnDamaged -= Life_OnWounded;
			life.OnDying -= Life_OnDead;
			anim.animEvents.OnUseLegs -= Anim_UseLegs;
			anim.animEvents.OnStopUsingLegs -= Anim_StopUsingLegs;
			anim.animEvents.OnDashStop -= Anim_DashStop;
			anim.animEvents.OnRecovered -= Anim_Recovered;
		}






		private void Anim_Recovered()
		{
			isWounded = false;
		}

		private void Anim_DashStop()
		{
			ability.StopPushing();
		}

		private void Reset()
		{
			ability.CanMove = true;
		}

		private void Anim_StopUsingLegs()
		{
			ability.CanMove = true;
			body.doableLegs.Stop(ability);
		}

		private void Anim_UseLegs()
		{
			ability.CanMove = false;
			body.doableLegs.DoActivity(ability);
		}

		private void Life_OnWounded(Attack attack)
		{
			ability.Push(attack.Direction, attack.DamageAmount );
			body.BottomFaceDirection(attack.Direction.x < 0);
			isWounded = true;
		}

		private void Life_OnDead(Player player, Life life1)
		{
			ability.CanMove = false;
		}

		private void Player_MoveInDirection(Vector2 direction)
		{
			if (pauseManager.IsPaused) return;
			if (isWounded) return;
			if (!ability.CanMove) return;

			if (body.doableLegs.isActive)
			{
				StopMoving();
				return;
			}

			if (direction.magnitude < .5f)
			{
				StopMoving();
				return;
			}

			StartMoving(direction);
		}

		private void AI_MoveInDirection(Vector2 direction)
		{
			Debug.Log("ai try move in direction");
			if (pauseManager.IsPaused) return;
			if (!ability.CanMove)
			{
				Debug.Log("can't move");
				return;
			}

			if (body.doableArms.isActive)
			{
				Debug.Log("ai doableArms busy");
				return;
			}

			Debug.Log("ai move in direction");
			StartMoving(direction);
		}

		private void Player_StopMoving(IControlAxis controlAxis)
		{
			StopMoving();
		}

		private void AI_StopMoving()
		{
			Debug.Log("ai stop moving");
			StopMoving();
		}

		private void StartMoving(Vector2 direction)
		{
			if (!life.IsPlayer) Debug.Log("start moving: " + life.MoveSpeed);
			MoveDir = direction;
			if (direction.x != 0) body.BottomFaceDirection(direction.x > 0);
			ability.MoveInDirection(direction, life.MoveSpeed);

			anim.SetBool(UnitAnimations.IsMoving, true);
		}

		private void StopMoving()
		{
			if (pauseManager.IsPaused) return;
			anim.SetBool(UnitAnimations.IsMoving, false);
			ability.StopMoving();
		}

		public void Push(Vector2 moveMoveDir, float statsDashSpeed)
		{
			ability.Push(moveMoveDir, statsDashSpeed);
		}

		public bool IsIdle() => ability.IsIdle();

		public void OnPoolSpawn()
		{
			ability.CanMove = true;
			isWounded = false;
			MoveDir = Vector2.zero;
		}

		public void OnPoolDespawn()
		{
			StopMoving();
			ability.CanMove = false;
			MoveDir = Vector2.zero;
		}

		public void SetCanMove(bool on)
		{
			ability.CanMove = on;
			 if (!ability.CanMove)
				 ability.StopMoving();
			 else
			 {
				 if (player.IsPlayer()) ability.MoveInDirection(player.Controller.GetMoveAxisAngle(), life.MoveSpeed);
			 }
		}
	}
}
