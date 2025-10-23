using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways, RequireComponent(typeof(MoveAbility)), RequireComponent(typeof(SimpleJumpAbility))]
	public class BloodSplatterOnFloor : FallToFloor
	{
		private float jumpSpeed = .5f;
		public List<GameObject> BloodFlying = new();
		public List<GameObject> HitWallFlatAnimation = new();
		public List<GameObject> HitWallAngledAnimation = new();
		public List<GameObject> HitFloorAnimation = new();
		public List<GameObject> HitFloorMovingAnimation = new();

		private float fastEnoughSpeed = 100;

		private void HideAllAnimations()
		{
			foreach (var go in BloodFlying)
			{
				go.SetActive(false);
			}

			foreach (var go in HitWallFlatAnimation)
			{
				go.SetActive(false);
			}

			foreach (var go in HitWallAngledAnimation)
			{
				go.SetActive(false);
			}

			foreach (var go in HitFloorAnimation)
			{
				go.SetActive(false);
			}

			foreach (var go in HitFloorMovingAnimation)
			{
				go.SetActive(false);
			}
		}

		public override IDebree Fire(Vector2 shootAngle, float height, float verticalSpeed = 0, float pushSpeed = 120)
		{
			Debug.Log("Firing blood splatter" + shootAngle + " height " + height + " vertical speed " + verticalSpeed);
			base.Fire(shootAngle, height, verticalSpeed, pushSpeed*8);

			HideAllAnimations();
			BloodFlying.GetRandom().SetActive(true);

			simpleJumpAbility.OnLand += SplatOnLand;
			moveAbility.OnHitWall += SplatOnHitWall;
			return this;
		}

		private void SplatOnHitWall(RaycastHit2D obj, EffectSurface.SurfaceAngle surfaceAngle)
		{
			HideAllAnimations();
			if (surfaceAngle == EffectSurface.SurfaceAngle.Horizontal)
			{
				var anim = HitWallFlatAnimation.GetRandom();
				anim.SetActive(true);
			}
			else
			{
				var anim = HitWallAngledAnimation.GetRandom();
				anim.SetActive(true);
			}

			StopMoving();
		}

		private void SplatOnLand(Vector2 obj)
		{
			simpleJumpAbility.OnLand -= SplatOnLand;
			moveAbility.OnHitWall -= SplatOnHitWall;
			Debug.Log("Landed, changing sprite");
			HideAllAnimations();
			simpleJumpAbility.FreezeRotationAtIdentity();

			Debug.Log( "fastenoughspeed " + fastEnoughSpeed + " current speed " + moveAbility.GetTotalVelocity().magnitude);
			;
			if (moveAbility.IsMovingQuickly(fastEnoughSpeed))
			{
				var anim = HitFloorMovingAnimation.GetRandom();
				anim.SetActive(true);
			}
			else
			{
				var anim = HitFloorAnimation.GetRandom();
				anim.SetActive(true);
			}

			StopMoving();
		}

		private void StopMoving()
		{
			simpleJumpAbility.OnResting -= SplatOnLand;
			moveAbility.OnHitWall -= SplatOnHitWall;
			moveAbility.StopMoving();
			moveAbility.StopPush();
		}
	}
}
