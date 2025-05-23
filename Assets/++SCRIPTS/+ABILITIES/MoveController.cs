using __SCRIPTS._ENEMYAI;
using __SCRIPTS.Cursor;
using __SCRIPTS.HUD_Displays;
using UnityEngine;

namespace __SCRIPTS
{
	public class MoveController : MonoBehaviour, INeedPlayer
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
		private bool isWounded;
		private EnemyAI ai;
		private string VerbName = "Moving";

		public void SetPlayer(Player _player)
		{
			anim = GetComponent<Animations>();
			life = GetComponent<Life>();
			mover = GetComponent<MoveAbility>();
			life = GetComponent<Life>();
			body = GetComponent<Body>();
			owner = _player;

			life.OnWounded += Life_OnWounded;
			life.OnDying += Life_OnDead;
			InitializeLife();

			anim.animEvents.OnStep += Anim_OnStep;
			anim.animEvents.OnUseLegs += Anim_UseLegs;
			anim.animEvents.OnStopUsingLegs += Anim_StopUsingLegs;
			anim.animEvents.OnDashStop += Anim_DashStop;
			anim.animEvents.OnRecovered += Anim_Recovered;
		}
		private void InitializeLife()
		{
			if (life.IsPlayer)
			{
				owner = life.player;
				owner.Controller.MoveAxis.OnChange += Player_MoveInDirection;
				owner.Controller.MoveAxis.OnInactive += Player_StopMoving;
				CanMove = true;
			}
			else
			{
				ai = GetComponent<EnemyAI>();
				if (ai == null)
				{
					Debug.LogError("No AI found on " + gameObject.name);
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
			anim.animEvents.OnStep -= Anim_OnStep;
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
			body.legs.StopSafely(mover);
		}

		private void Anim_UseLegs()
		{
			CanMove = false;
			body.legs.Do(mover);
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
			if (PauseManager.I.IsPaused) return;
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
			if (PauseManager.I.IsPaused) return;
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
			if (PauseManager.I.IsPaused) return;
			anim.SetBool(Animations.IsMoving, false);
			mover.StopMoving();
		}


		public void Push(Vector2 moveMoveDir, float statsDashSpeed)
		{
			mover.Push(moveMoveDir, statsDashSpeed);
		}

		public bool IsIdle() => mover.IsIdle();
	}
}
