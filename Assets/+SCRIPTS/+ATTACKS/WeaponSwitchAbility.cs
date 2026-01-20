using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class WeaponSwitchAbility : Ability
	{

		public override string AbilityName => "Swapping Weapon";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canStop(IDoableAbility abilityToStopFor) => abilityToStopFor is GunAttack;

		private GunAttack gunAttack => _gunAttack ??= GetComponent<GunAttack>();
		private GunAttack _gunAttack;

		protected override void DoAbility()
		{
			SwitchGuns();
			Debug.Log("Switched Guns",this);
			PlayAnimationClip(gunAttack.CurrentGun.pullOutAnimationClip, 1);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			player.Controller.SwapWeaponSquare.OnPress -= Player_SwapWeapon;
			player.Controller.SwapWeaponSquare.OnPress += Player_SwapWeapon;
		}

		private void SwitchGuns()
		{
			gunAttack.SwapGuns();
		}

		private void OnDestroy()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			if (player.Controller.SwapWeaponSquare == null) return;
			player.Controller.SwapWeaponSquare.OnPress -= Player_SwapWeapon;
		}

		private void Player_SwapWeapon(NewControlButton obj)
		{
			Try();
		}
	}
}
