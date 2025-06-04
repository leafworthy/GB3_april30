using System;
using System.Collections.Generic;
using GangstaBean.Core;
using GangstaBean.UI;
using UnityEngine;


namespace GangstaBean.Abilities
{
	public class GunAimAbility_Simple : AimAbility
	{
		private Animations anim;
		private bool isAttacking;
		private IAimableGun aimableGun;

		private static  string[] AnimationClips =
		{

			"E",
			"EES",
			"ES",
			"SE",
			"SSE",
			"SSE",
			"SE",
			"ES",
			"EES",
			"E",
			"EEN",
			"EN",
			"NE",
			"NNE",
			"NNE",
			"NE",
			"EN",
			"EEN",
		};


		public override void SetPlayer(Player player)
		{
			base.SetPlayer(player);
			anim = GetComponent<Animations>();
			aimableGun = GetComponent<IAimableGun>();
			anim.animEvents.OnAnimationComplete += Anim_OnComplete;
			aimableGun.OnShotHitTarget += AimableGunOnShoot;
			aimableGun.OnShotMissed += AimableGunOnShoot;
		}

		private void OnDisable()
		{
			if (aimableGun == null) return;
			aimableGun.OnShotHitTarget -= AimableGunOnShoot;
			aimableGun.OnShotMissed -= AimableGunOnShoot;
			if (anim == null) return;
			if (anim.animEvents != null) anim.animEvents.OnAnimationComplete -= Anim_OnComplete;
		}

		protected override Vector3 GetRealAimDir()
		{
			if (owner.isUsingMouse) return CursorManager.GetMousePosition() - body.AimCenter.transform.position;
			return owner.Controller.AimAxis.GetCurrentAngle();
		}

		private void Anim_OnComplete()
		{
			isAttacking = false;
		}

		private void AimableGunOnShoot(Attack attack, Vector2 vector2)
		{
			if (attack.Owner != owner) return;
			if (body.arms.isActive) return;
			isAttacking = true;
			anim.Play("Shoot", 1, 0);
		}

		private float GetDegrees()
		{
			float radians = Mathf.Atan2(-AimDir.y, AimDir.x); // NEGATE y-axis
			float degrees = radians * Mathf.Rad2Deg;
			if (degrees < 0)
				degrees += 360f;
			return degrees;
		}

		protected override void Update()
		{
			base.Update();
			body.TopFaceDirection(GetClampedAimDir().x >= 0);


			if (!isAttacking && !aimableGun.isReloading && !body.arms.isActive)
			{
				anim.Play(GetAnimationClipNameFromDegrees(GetDegrees()), 1, 0);

			}
		}

		private string GetAnimationClipNameFromDegrees(float degrees)
		{
			var whichOne = GetDirectionPortion(degrees);

			if (whichOne > AnimationClips.Length) whichOne = 0;
			return AnimationClips[whichOne];
		}

		private static int GetDirectionPortion(float degrees)
		{
			// Normalize the degrees to be within [0, 360)
			float normalizedDegrees = degrees % 360;
			if (normalizedDegrees < 0)
			{
				normalizedDegrees += 360;
			}

			// Offset by 10° (half of a portion) to make east straddle a portion
			// rather than being at a boundary
			float offsetDegrees = (normalizedDegrees + 10) % 360;

			// Each portion is 20° wide (360° ÷ 18)
			const float portionWidth = 20f;

			// Calculate the portion number (1-18)
			int portionNumber = Mathf.FloorToInt(offsetDegrees / portionWidth);
			portionNumber = Mathf.Clamp(portionNumber, 0, AnimationClips.Length - 1);

			return portionNumber;
		}


		private Vector2 GetClampedAimDir()
		{
			var clampedDir = AimDir.normalized;
			if (clampedDir.x <= .25f && clampedDir.x >= 0)
			{
				clampedDir.x = .25f;
				if (AimDir.y > 0)
					clampedDir.y = 1f;
				else
					clampedDir.y = -1;
			}

			if (clampedDir.x < 0 && clampedDir.x > -.25) clampedDir.x = -.25f;
			return clampedDir;
		}
	}
}
