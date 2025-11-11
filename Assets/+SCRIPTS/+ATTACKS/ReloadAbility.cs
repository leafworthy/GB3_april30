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
		private GunAttack gunAttack => _gunAttack ??= GetComponent<GunAttack>();
		private GunAttack _gunAttack;

		private float reloadTime = .5f;
		public override string AbilityName => "Reloading";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && gunAttack.CurrentGun.CanReload();

		public override bool canStop(IDoableAbility abilityToStopFor) => abilityToStopFor is DashAbility or JumpAbility;

		protected override void DoAbility()
		{
			StartReloading();
		}

		private void StartReloading()
		{
			Debug.Log("Starting Reload");
			Invoke(nameof(Reload), gunAttack.CurrentGun.reloadTime);
			anim.SetBool(UnitAnimations.IsBobbing, false);

			if (!gunAttack.IsUsingPrimaryGun)
				PlayAnimationClip(ReloadUnlimitedGunAnimationClip, 1);
			else
				PlayAnimationClip(ReloadPrimaryGunAnimationClip, 1);
		}

		public override void Stop()
		{
			base.Stop();
			Debug.Log("Reload Stopped, doing gun offence");
			gunAttack.Resume();
		}

		private void Reload()
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
			base.Stop();
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			if (player != null) player.Controller.ReloadTriangle.OnPress -= Player_Reload;
			if (gunAttack != null) gunAttack.OnNeedsReload -= Gun_OnNeedsReload;
			player.Controller.ReloadTriangle.OnPress += Player_Reload;
			gunAttack.OnNeedsReload += Gun_OnNeedsReload;
		}

		private void Gun_OnNeedsReload()
		{
			Do();
		}

		private void Player_Reload(NewControlButton btn)
		{
			Do();
		}
	}
}
