using System;
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
	public class MoveController : MonoBehaviour, INeedPlayer, IPoolable
	{
		public bool CanMove { get; set; }

		public Vector2 MoveDir { get; private set; }

		private Life life;
		private Body body;
		private MoveAbility mover;
		private Player owner;
		private float damagePushMultiplier = 1;
		private UnitAnimations anim;
		private bool isWounded;
		private IMove ai;
		public void SetPlayer(Player _player)
		{
			anim = GetComponent<UnitAnimations>();

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
				mover.StopMoving();
			else
			{
				if (owner.IsPlayer()) mover.MoveInDirection(owner.Controller.MoveAxis.GetCurrentAngle(), life.MoveSpeed);
			}
		}

		private void InitializeLife()
		{
			life = GetComponent<Life>();
			life.OnWounded += Life_OnWounded;
			life.OnDying += Life_OnDead;

			if (owner.IsPlayer())
			{
				owner.Controller.MoveAxis.OnChange += Player_MoveInDirection;
				owner.Controller.MoveAxis.OnInactive += Player_StopMoving;
				CanMove = true;
			}
			else
			{
				Debug.Log("AI move controller initialized");
				ai = GetComponent<IMove>();
				if (ai == null)
				{
					Debug.Log("no ai component");
					return;
				}

				ai.OnMoveInDirection += AI_MoveInDirection;
				ai.OnStopMoving += AI_StopMoving;
				CanMove = true;
				Debug.Log("registered ai move controller");
			}
		}

		private void OnDisable()
		{
			if (life == null) return;
			if (life.IsHuman)
			{
				Debug.Log("disable here");
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
			if (Services.pauseManager.IsPaused) return;
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
			if (Services.pauseManager.IsPaused) return;
			if (!CanMove)
			{
				Debug.Log("can't move");
				return;
			}

			if (body.arms.isActive) return;

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
			MoveDir = direction;
			if (direction.x != 0) body.BottomFaceDirection(direction.x > 0);
			mover.MoveInDirection(direction, life.MoveSpeed);

			anim.SetBool(UnitAnimations.IsMoving, true);
		}

		private void StopMoving()
		{
			if (Services.pauseManager.IsPaused) return;
			anim.SetBool(UnitAnimations.IsMoving, false);
			mover?.StopMoving();
		}

		public void Push(Vector2 moveMoveDir, float statsDashSpeed)
		{
			mover.Push(moveMoveDir, statsDashSpeed);
		}

		public bool IsIdle() => mover.IsIdle();

		public void OnPoolSpawn()
		{
			CanMove = true;
			isWounded = false;
			MoveDir = Vector2.zero;
			if (life != null && !life.IsHuman) InitializeLife();
		}

		public void OnPoolDespawn()
		{
			CanMove = false;
			MoveDir = Vector2.zero;
		}
	}
}
