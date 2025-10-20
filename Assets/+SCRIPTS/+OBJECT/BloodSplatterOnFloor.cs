using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS
{
	[RequireComponent(typeof(MoveAbility)), RequireComponent(typeof(SimpleJumpAbility))]
	public class BloodSplatterOnFloor : FallToFloor
	{
		private float jumpSpeed = .5f;
		public List<GameObject> BloodFlying = new();
		public List<GameObject> HitWallFlatAnimation  = new();
		public List<GameObject> HitWallAngledAnimation = new();
		public List<GameObject> HitFloorAnimation = new();
		public List<GameObject> HitFloorMovingAnimation = new();

		private float fastEnoughSpeed = 3f;


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

		[Button]
		private void SimpleJumpAbilityOnResting(Vector2 obj)
		{
			Debug.Log("Landed, changing sprite");
			HideAllAnimations();
			StopMoving();
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
		}

		public override IDebree Fire(Vector2 angle, float height)
		{
			Splatter(angle, height);
			return this;
		}

		public void Splatter(Vector3 shootAngle, float height)
		{
			simpleJumpAbility.OnLand += SplatOnResting;
			simpleJumpAbility.Jump(height, jumpSpeed, bounceSpeed);

			HideAllAnimations();
			BloodFlying.GetRandom().SetActive(true);

			moveAbility.OnHitWall += SplatOnHitWall;
			moveAbility.SetDragging(false);
			moveAbility.Push(shootAngle, Random.Range(0, PushSpeed));
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

		private void SplatOnResting(Vector2 obj)
		{
			StopMoving();
		}

		private void StopMoving()
		{
			simpleJumpAbility.OnResting -= SplatOnResting;
			moveAbility.OnHitWall -= SplatOnHitWall;
			moveAbility.StopMoving();
		}
	}
}
