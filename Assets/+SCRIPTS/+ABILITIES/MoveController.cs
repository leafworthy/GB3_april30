using System;
using __SCRIPTS._ENEMYAI;
using __SCRIPTS.Cursor;
using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

public interface IMove
{
	event Action<Vector2> OnMoveInDirection;
	event Action OnStopMoving;
	bool BornOnAggro { get; set; }
}

namespace __SCRIPTS
{
	public class MoveController : ServiceUser, INeedPlayer, IPoolable
	{
		public bool CanMove { get; set; }

		public Vector2 MoveDir { get; private set; }

		private Life life;
		private Body body;
		private MoveAbility mover;
		private Player owner;
		private float damagePushMultiplier = 1;
		private Animations anim;
		private bool isWounded;
		private IMove ai;
		public void SetPlayer(Player _player)
		{
			anim = GetComponent<Animations>();

			mover = GetComponent<MoveAbility>();
			body = GetComponent<Body>();
			body.OnCanMove += Body_OnCanMove;
			owner = _player;

			InitializeLife();

			anim.animEvents.OnUseLegs += Anim_UseLegs;
			anim.animEvents.OnStopUsingLegs += Anim_StopUsingLegs;
			anim.animEvents.OnDashStop += Anim_DashStop;
			anim.animEvents.OnRecovered += Anim_Recovered;
		}

		private void Body_OnCanMove(bool canMove)
		{
			if (!canMove)
			{
				mover.StopMoving();
			}
			else
			{
				if(owner.IsPlayer()) mover.MoveInDirection(owner.Controller.MoveAxis.GetCurrentAngle(), life.MoveSpeed);
			}
		}

		private void InitializeLife()
		{
			life = GetComponent<Life>();
			life.OnWounded += Life_OnWounded;
			life.OnDying += Life_OnDead;

			if (life.IsPlayer)
			{

				owner = life.player;
				owner.Controller.MoveAxis.OnChange += Player_MoveInDirection;
				owner.Controller.MoveAxis.OnInactive += Player_StopMoving;
				CanMove = true;
			}
			else
			{
				ai = GetComponent<IMove>();
				if (ai == null)
				{
					return;
				}

				ai.OnMoveInDirection += AI_MoveInDirection;
				ai.OnStopMoving += AI_StopMoving;
				CanMove = !ai.BornOnAggro;
			}
		}

		private void OnDisable()
		{
			if (life == null) return;
			if (life.IsPlayer)
			{
				owner.Controller.MoveAxis.OnChange -= Player_MoveInDirection;
				owner.Controller.MoveAxis.OnInactive -= Player_StopMoving;
			}
			else
			{
				if (ai == null) return;
				ai.OnMoveInDirection -= AI_MoveInDirection;
				ai.OnStopMoving -= AI_StopMoving;
			}

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
			mover.StopPush();
		}

		private void Reset()
		{
			CanMove = true;
		}

		private void Anim_StopUsingLegs()
		{
			CanMove = true;
			body.legs.Stop(mover);
		}

		private void Anim_UseLegs()
		{
			CanMove = false;
			body.legs.Do(mover);
		}



		private void Life_OnWounded(Attack attack)
		{
			mover.Push(attack.Direction, attack.DamageAmount * damagePushMultiplier);
			body.BottomFaceDirection(attack.Direction.x < 0);
			isWounded = true;
		}

		private void Life_OnDead(Player player, Life life1)
		{
			CanMove = false;
		}

		private void Player_MoveInDirection(IControlAxis controlAxis, Vector2 direction)
		{
			if (pauseManager.IsPaused) return;
			if (isWounded) return;
			if (!CanMove) return;


			if (body.legs.isActive)
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
			if (pauseManager.IsPaused) return;
			if (!CanMove) return;
			if (body.arms.isActive) return;
			StartMoving(direction);
		}
		private void Player_StopMoving(IControlAxis controlAxis)
		{
			StopMoving();
		}

		private void AI_StopMoving()
		{
			StopMoving();
		}
		private void StartMoving(Vector2 direction)
		{
			MoveDir = direction;
			if (direction.x != 0) body.BottomFaceDirection(direction.x > 0);
			mover.MoveInDirection(direction, life.MoveSpeed);

			anim.SetBool(Animations.IsMoving, true);
		}

		private void StopMoving()
		{
			if (pauseManager.IsPaused) return;
			anim.SetBool(Animations.IsMoving, false);
			mover.StopMoving();
		}


		public void Push(Vector2 moveMoveDir, float statsDashSpeed)
		{
			mover.Push(moveMoveDir, statsDashSpeed);
		}

		public bool IsIdle() => mover.IsIdle();

		public void OnPoolSpawn()
		{
			// Reinitialize movement controller when spawned from pool
			CanMove = true;
			isWounded = false;
			MoveDir = Vector2.zero;
			// Reinitialize life connections if it's not a player
			if (life != null && !life.IsPlayer)
			{
				InitializeLife();
			}
		}

		public void OnPoolDespawn()
		{
			// Clean up when returning to pool
			StopMoving();
			CanMove = false;
			MoveDir = Vector2.zero;
		}
	}
}
