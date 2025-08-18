using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
	public class GunAimAbility_Simple : AimAbility
	{
		private bool isAttacking;
		private IAimableGun aimableGun;
		private bool IsAimingRight() => aimDir.normalized.x >= 0;

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

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
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
			if (player.isUsingMouse) return CursorManager.GetMousePosition() - body.AimCenter.transform.position;
			return player.Controller.GetAimAxisAngle();
		}

		private void Anim_OnComplete()
		{
			isAttacking = false;
		}

		private void AimableGunOnShoot(Attack attack, Vector2 vector2)
		{
			if (attack.Owner != player) return;
			if (body.doableArms.isActive) return;
			isAttacking = true;
			anim.Play("Shoot", 1, 0);
		}

		private float GetDegrees()
		{
			var radians = Mathf.Atan2(-aimDir.y, aimDir.x); // NEGATE y-axis
			var degrees = radians * Mathf.Rad2Deg;
			if (degrees < 0)
				degrees += 360f;
			return degrees;
		}

		protected override void Update()
		{
			base.Update();
			body.TopFaceDirection(IsAimingRight());

			if (!isAttacking && !aimableGun.isReloading && !body.doableArms.isActive) anim.Play(GetAnimationClipNameFromDegrees(GetDegrees()), 1, 0);
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
			var normalizedDegrees = degrees % 360;
			if (normalizedDegrees < 0) normalizedDegrees += 360;

			// Offset by 10° (half of a portion) to make east straddle a portion
			// rather than being at a boundary
			var offsetDegrees = (normalizedDegrees + 10) % 360;

			// Each portion is 20° wide (360° ÷ 18)
			const float portionWidth = 20f;

			// Calculate the portion number (1-18)
			var portionNumber = Mathf.FloorToInt(offsetDegrees / portionWidth);
			portionNumber = Mathf.Clamp(portionNumber, 0, AnimationClips.Length - 1);

			return portionNumber;
		}


	}
}
