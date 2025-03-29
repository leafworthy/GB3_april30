using __SCRIPTS._ENEMYAI;
using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
	public class MoveController : MonoBehaviour
	{
		public bool CanMove { get; set; }

		public Vector2 MoveDir { get; private set; }

		private Vector2 targetPosition;
		private Life life;
		private Body body;
		private MoveAbility mover;
		private Player owner;
		private float damagePushMultiplier = 1;
		private Animations anim;
		private bool isDamaged;
		private EnemyAI ai;

		private void Start()
		{
			anim = GetComponent<Animations>();
			life = GetComponent<Life>();
			mover = GetComponent<MoveAbility>();
			life = GetComponent<Life>();
			body = GetComponent<Body>();
			owner = life.player;

			life.OnDamaged += Life_OnDamaged;
			life.OnDying += Life_OnDead;
			if (life.IsPlayer)
			{
				owner = life.player;
				var _controller = owner.Controller;
				_controller.MoveAxis.OnChange += Player_MoveInDirection;
				_controller.MoveAxis.OnInactive += Player_StopMoving;
				CanMove = true;
			}
			else
			{
				ai = GetComponent<EnemyAI>();
				ai.OnMoveInDirection += AI_MoveInDirection;
				ai.OnStopMoving += AI_StopMoving;
				if (ai.BornOnAggro)
					CanMove = false;
				else
					CanMove = true;
			}

			anim.animEvents.OnStep += Anim_OnStep;
			anim.animEvents.OnUseLegs += Anim_UseLegs;
			anim.animEvents.OnStopUsingLegs += Anim_StopUsingLegs;
			anim.animEvents.OnDashStop += Anim_DashStop;
			anim.animEvents.OnRecovered += Anim_Recovered;
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

			life.OnDamaged -= Life_OnDamaged;
			life.OnDying -= Life_OnDead;
			anim.animEvents.OnStep -= Anim_OnStep;
			anim.animEvents.OnUseLegs -= Anim_UseLegs;
			anim.animEvents.OnStopUsingLegs -= Anim_StopUsingLegs;
			anim.animEvents.OnDashStop -= Anim_DashStop;
			anim.animEvents.OnRecovered -= Anim_Recovered;
		}

		private void Anim_Recovered()
		{
			isDamaged = false;
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
			body.legs.Stop("On use legs");
		}

		private void Anim_UseLegs()
		{
			CanMove = false;
			body.legs.Do("On use legs");
		}

		private void Anim_OnStep()
		{
			var dust = ObjectMaker.I.Make(ASSETS.FX.dust1_ground, body.FootPoint.transform.position);
			if (mover.moveDir.x > 0)
			{
				dust.transform.localScale = new Vector3(-Mathf.Abs(dust.transform.localScale.x),
					dust.transform.localScale.y, dust.transform.localScale.z);
			}
			else
				dust.transform.localScale = new Vector3(Mathf.Abs(dust.transform.localScale.x), dust.transform.localScale.y, dust.transform.localScale.z);
		}

		public Vector2 GetMovePoint()
		{
			if (owner.isUsingMouse) return CursorManager.GetMousePosition();
			return (Vector2) body.AimCenter.transform.position + MoveDir * AimAbility.aimDistanceFactor;
		}

		private void Life_OnDamaged(Attack attack)
		{
			mover.Push(attack.Direction, attack.DamageAmount * damagePushMultiplier);
			body.BottomFaceDirection(attack.Direction.x < 0);
			isDamaged = true;
		}

		private void Life_OnDead(Player player, Life life1)
		{
			CanMove = false;
		}

		private void Player_MoveInDirection(IControlAxis controlAxis, Vector2 direction)
		{
			if (PauseManager.IsPaused) return;
			if (isDamaged) return;
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

		private void StartMoving(Vector2 direction)
		{
			MoveDir = direction;
			if (direction.x != 0) body.BottomFaceDirection(direction.x > 0);
			mover.MoveInDirection(direction, life.MoveSpeed);

			anim.SetBool(Animations.IsMoving, true);
		}

		private void StopMoving()
		{
			if (PauseManager.IsPaused) return;
			anim.SetBool(Animations.IsMoving, false);
			mover.StopMoving();
		}

		private void Player_StopMoving(IControlAxis controlAxis)
		{
			StopMoving();
		}

		private void AI_StopMoving()
		{
			StopMoving();
		}

		private void AI_MoveInDirection(Vector2 direction)
		{
			if (PauseManager.IsPaused) return;
			if (!CanMove) return;
			if (body.arms.isActive) return;
			StartMoving(direction);
		}

		public void Push(Vector2 moveMoveDir, float statsDashSpeed)
		{
			mover.Push(moveMoveDir, statsDashSpeed);
		}
	}
}