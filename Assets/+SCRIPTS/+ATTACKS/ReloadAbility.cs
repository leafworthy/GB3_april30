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
		GunAttack gunAttack => _gunAttack ??= GetComponent<GunAttack>();
		GunAttack _gunAttack;

		public override string AbilityName => "Reloading";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && gunAttack.CurrentGun.CanReload();

		public override bool canStop(IDoableAbility abilityToStopFor) => abilityToStopFor is DashAbility or JumpAbility or ChainsawAttack or ShieldDashAbility or ThrowMineAttack;

		protected override void DoAbility()
		{
			StartReloading();
		}

		void StartReloading()
		{
			Invoke(nameof(Reload), gunAttack.CurrentGun.reloadTime);
			anim.SetBool(UnitAnimations.IsBobbing, false);

			if (!gunAttack.IsUsingPrimaryGun)
				PlayAnimationClip(ReloadUnlimitedGunAnimationClip, 1);
			else
				PlayAnimationClip(ReloadPrimaryGunAnimationClip, 1);
		}

		public override void StopAbility()
		{
			base.StopAbility();
			CancelInvoke(nameof(Reload));
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
			base.StopAbility();
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
			TryToActivate();
		}

		void Player_Reload(NewControlButton btn)
		{
			TryToActivate();
		}
	}
}
