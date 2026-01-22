using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class ReloadAbility : Ability
	{
		public event Action OnReload;
		public AnimationClip ReloadPrimaryGunAnimationClip;
		public AnimationClip ReloadUnlimitedGunAnimationClip;
		GunAttackSingle gunAttack => _gunAttack ??= GetComponent<GunAttackSingle>();
		GunAttackSingle _gunAttack;

		public override string AbilityName => "Reloading";
		public override bool requiresArms() => true;

		public override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && gunAttack.CurrentGun.CanReload();

		public override bool canStop(Ability abilityToStopFor) => abilityToStopFor is DashAbility or JumpAbility;

		protected override void DoAbility()
		{
			StartReloading();
		}

		void StartReloading()
		{
			Invoke(nameof(Reload), gunAttack.CurrentGun.reloadTime);
			anim.SetBool(UnitAnimations.IsBobbing, false);

			if (gunAttack.CurrentGun.CanReload())
				PlayAnimationClip(gunAttack.CurrentGun.ReloadAnimationClip);
		}

		public override void StopAbilityBody()
		{
			base.StopAbilityBody();
			gunAttack.Resume();
		}

		void Reload()
		{
			gunAttack.CurrentGun.Reload();
			OnReload?.Invoke();
		}

		public void OnDestroy()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			if (player.Controller.ReloadTriangle == null) return;
			player.Controller.ReloadTriangle.OnPress -= Player_Reload;
			base.StopAbilityBody();
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			if (player != null) player.Controller.ReloadTriangle.OnPress -= Player_Reload;
			if (gunAttack != null) gunAttack.OnNeedsReload -= Gun_OnNeedsReload;
			player.Controller.ReloadTriangle.OnPress += Player_Reload;
			gunAttack.OnNeedsReload += Gun_OnNeedsReload;
		}

		void Gun_OnNeedsReload()
		{
			TryToDoAbility();
		}

		void Player_Reload(NewControlButton btn)
		{
			TryToDoAbility();
		}
	}
}
