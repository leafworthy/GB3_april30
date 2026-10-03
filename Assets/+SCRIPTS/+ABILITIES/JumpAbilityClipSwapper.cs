using UnityEngine;

namespace __SCRIPTS
{
	public class JumpAbilityClipSwapper : MonoBehaviour
	{
		public JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
		JumpAbility _jumpAbility;

		public TwoWeaponSwitchAbility twoWeaponSwitchAbility => _twoWeaponSwitchAbility ??= GetComponent<TwoWeaponSwitchAbility>();
		TwoWeaponSwitchAbility _twoWeaponSwitchAbility;

		public AnimationClip jumpingAnimationClip_1;
		public AnimationClip fallingAnimationClip_1;
		public AnimationClip landingAnimationClip_1;
		public AnimationClip flyingAnimationClip_1;

		public AnimationClip jumpingAnimationClip_2;
		public AnimationClip fallingAnimationClip_2;
		public AnimationClip landingAnimationClip_2;
		public AnimationClip flyingAnimationClip_2;

		void Start()
		{
			twoWeaponSwitchAbility.OnSwitchWeapon += TwoWeaponSwitchAbility_OnSwitchWeapon;
		}

		void OnDisable()
		{
			twoWeaponSwitchAbility.OnSwitchWeapon -= TwoWeaponSwitchAbility_OnSwitchWeapon;
		}

		void TwoWeaponSwitchAbility_OnSwitchWeapon(int set)
		{
			SetClipSet(set);
		}

		void SetClipSet(int setNumber)
		{
			switch (setNumber)
			{
				case 1:
					jumpAbility.jumpingAnimationClip = jumpingAnimationClip_1;
					jumpAbility.fallingAnimationClip = fallingAnimationClip_1;
					jumpAbility.landingAnimationClip = landingAnimationClip_1;
					jumpAbility.flyingAnimationClip = flyingAnimationClip_1;
					break;
				case 2:
					jumpAbility.jumpingAnimationClip = jumpingAnimationClip_2;
					jumpAbility.fallingAnimationClip = fallingAnimationClip_2;
					jumpAbility.landingAnimationClip = landingAnimationClip_2;
					jumpAbility.flyingAnimationClip = flyingAnimationClip_2;
					break;
			}
		}
	}
}
