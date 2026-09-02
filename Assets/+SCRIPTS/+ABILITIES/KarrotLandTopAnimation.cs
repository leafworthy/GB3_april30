using UnityEngine;

namespace __SCRIPTS
{
	// On landing, plays the weapon-appropriate top-sprite land animation on the Top layer (layer 1),
	// mirroring ReloadAbility's primary/unlimited-gun split. The Bottom layer plays the shared
	// leg/cape land clip; this only drives the Top sprite (arms) so it differs per weapon.
	public class KarrotLandTopAnimation : MonoBehaviour
	{
		public AnimationClip LandPrimaryGunAnimationClip;    // awp
		public AnimationClip LandUnlimitedGunAnimationClip;  // spray

		UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
		UnitAnimations _anim;
		GunAttack gunAttack => _gunAttack ??= GetComponent<GunAttack>();
		GunAttack _gunAttack;
		JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		JumpAbility _jumps;

		void OnEnable()
		{
			if (jumps != null) jumps.OnLand += JumpsOnLand;
		}

		void OnDisable()
		{
			if (jumps != null) jumps.OnLand -= JumpsOnLand;
		}

		void JumpsOnLand(Vector2 position)
		{
			var clip = (gunAttack != null && !gunAttack.isActive)
				? LandUnlimitedGunAnimationClip
				: LandPrimaryGunAnimationClip;
			if (clip != null && anim != null)
			{
				anim.Play(clip.name, 0, 0);
				anim.Play(clip.name, 1, 0);
			}
		}
	}
}
