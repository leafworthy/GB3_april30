using UnityEngine;

namespace __SCRIPTS
{
	public class WeaponSwitchAbility : Ability
	{
		public bool IsUsingPrimaryWeapon = true;

		public override string AbilityName => "Swapping Weapon";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public AnimationClip GlockPullOutAnimationClip;
		public AnimationClip AK47PullOutAnimationClip;

		private GunAttack gunAttack => _gunAttack ??= GetComponent<GunAttack>();
		private GunAttack _gunAttack;
		protected override void DoAbility()
		{
			SwitchGuns();
			PlayAnimationClip(IsUsingPrimaryWeapon ? AK47PullOutAnimationClip : GlockPullOutAnimationClip, 1);
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			player.Controller.SwapWeaponSquare.OnPress -= Player_SwapWeapon;
			IsUsingPrimaryWeapon = true;
			player.Controller.SwapWeaponSquare.OnPress += Player_SwapWeapon;
		}
		private void SwitchGuns()
		{
			IsUsingPrimaryWeapon = !IsUsingPrimaryWeapon;
			gunAttack.SwitchGuns(IsUsingPrimaryWeapon ? 0: 1);
		}

		private void OnDestroy()
		{
			if (player == null) return;
			player.Controller.SwapWeaponSquare.OnPress -= Player_SwapWeapon;
		}

		private void Player_SwapWeapon(NewControlButton obj)
		{
			Do();
		}
	}
}
