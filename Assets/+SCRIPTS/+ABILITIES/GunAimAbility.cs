using System;
using UnityEngine;

namespace __SCRIPTS
{

	public class GunAimAbility : DoableAimAbility
	{
		private IAimableGun aimableGun;
		public bool simpleShoot;

		private static string[] AnimationClips =
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
			"EEN"
		};

		public override void SetPlayer(Player player)
		{
			base.SetPlayer(player);
			aimableGun = GetComponent<IAimableGun>();
			aimableGun.OnShotHitTarget += AimableGunOnShoot;
			aimableGun.OnShotMissed += AimableGunOnShoot;
		}

		protected override void DoAbility()
		{
			base.DoAbility();
			body.TopFaceDirection(GetClampedAimDir().x >= 0);
			anim.Play(GetClipNameFromDegrees(GetDegreesFromAimDir()), 1, 0);
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
		}

		private void AimableGunOnShoot(Attack attack, Vector2 vector2)
		{
			if (body.arms.isActive) return;
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			if (simpleShoot)
				anim.Play("Shoot", 1, 0);
			else
				anim.Play(GetClipNameFromDegrees(GetDegreesFromAimDir()), 1, .25f);
		}

		private float GetDegreesFromAimDir()
		{
			var radians = Mathf.Atan2(-AimDir.y, AimDir.x); // NEGATE y-axis
			var degrees = radians * Mathf.Rad2Deg;
			if (degrees < 0)
				degrees += 360f;
			return degrees;
		}

		private string GetClipNameFromDegrees(float degrees)
		{
			var whichPortion = GetDirectionPortion(degrees);
			if (whichPortion > AnimationClips.Length) whichPortion = 0;
			return AnimationClips[whichPortion];
		}

		private static int GetDirectionPortion(float degrees)
		{
			var normalizedDegrees = degrees % 360;
			if (normalizedDegrees < 0) normalizedDegrees += 360;
			var offsetDegrees = (normalizedDegrees + 10) % 360;
			var portionWidth = 360f / AnimationClips.Length;
			var portionNumber = Mathf.FloorToInt(offsetDegrees / portionWidth);
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
