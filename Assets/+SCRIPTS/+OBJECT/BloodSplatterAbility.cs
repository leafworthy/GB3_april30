using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways, RequireComponent(typeof(MoveAbility)), RequireComponent(typeof(JumpAndRotateAbility))]
	public class BloodSplatterAbility : MoveJumpAndRotateAbility
	{
		public List<GameObject> BloodFlying = new();
		public List<GameObject> HitWallFlatAnimation = new();
		public List<GameObject> HitWallAngledAnimation = new();
		public List<GameObject> HitFloorAnimation = new();
		public List<GameObject> HitFloorMovingAnimation = new();
		public GameObject rotationObject;

		Vector2 heightCorrectionForDepthInFrontOfWall = new(0, -1.5f);
		float fastEnoughSpeed = 5;
		bool isMovingQuickly;
		Vector2 moveDir;

		void Start()
		{
			moveAbility.OnHitWall += SplatOnHitWall;
		}



		void HideAllAnimations()
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

		public override void Fire(Vector2 shootAngle, float height, float verticalSpeed = 0, float pushSpeed = 120)
		{
			var randScale = Random.Range(.6f, 1.5f);
			transform.localScale = new Vector3(randScale, randScale, randScale);
			base.Fire(shootAngle, height, verticalSpeed, pushSpeed );
			moveDir = shootAngle.normalized;
			isMovingQuickly = moveAbility.GetTotalVelocity().magnitude > fastEnoughSpeed;
			HideAllAnimations();
			BloodFlying.GetRandom().SetActive(true);
		}

		void SplatOnHitWall(RaycastHit2D raycastHit2D, EffectSurface.SurfaceAngle surfaceAngle)
		{
			Debug.Log("trying to splat on wall");
			HideAllAnimations();
			if (surfaceAngle == EffectSurface.SurfaceAngle.Horizontal)
			{
				var anim = HitWallFlatAnimation.GetRandom();
				anim.SetActive(true);
			}
			else if (surfaceAngle == EffectSurface.SurfaceAngle.DiagonalFacingRight)
			{
				var anim = HitWallAngledAnimation.GetRandom();
				anim.SetActive(true);
			}
			else if (surfaceAngle == EffectSurface.SurfaceAngle.DiagonalFacingLeft)
			{
				var anim = HitWallAngledAnimation.GetRandom();
				HeightObject.transform.localScale = new Vector3(-1, 1, 1);
				anim.SetActive(true);
			}

			transform.position += (Vector3) heightCorrectionForDepthInFrontOfWall;
			SetHeight(GetHeight() - heightCorrectionForDepthInFrontOfWall.y);
			FreezeHeight();
			StopMoving();
		}

		protected override void Land()
		{
			base.Land();
			moveAbility.OnHitWall -= SplatOnHitWall;
			HideAllAnimations();
			FreezeRotationAtIdentity();

			if (isMovingQuickly)
			{
				var anim = HitFloorMovingAnimation.GetRandom();
				RotateToDirection(moveDir, rotationObject);
				//Rotate2DUtility.RotateTowardDirection2D(HeightObject.transform, moveAbility.GetTotalVelocity(), -35);
				anim.SetActive(true);
			}
			else
			{
				var anim = HitFloorAnimation.GetRandom();
				anim.SetActive(true);
			}

			StopMoving();
		}


		protected override void StartResting()
		{
			base.StartResting();
			StopMoving();
		}

		void StopMoving()
		{
			moveAbility.OnHitWall -= SplatOnHitWall;
			moveAbility.StopMoving();
			moveAbility.StopPush();
		}
	}
}
