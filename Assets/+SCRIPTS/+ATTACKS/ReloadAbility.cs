using System;
using System.Collections.Generic;
using System.Linq;
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

		protected override void DoAbility()
		{
			StartReloading();
		}

		private void StartReloading()
		{
			Debug.Log("start reloading");
			Invoke(nameof(Reload), gunAttack.CurrentGun.reloadTime);
			anim.SetBool(UnitAnimations.IsBobbing, false);

			if (!gunAttack.IsUsingPrimaryGun)
			{
				Debug.Log("playing glock reload animation");
				PlayAnimationClip(ReloadUnlimitedGunAnimationClip, 1);
			}
			else
				PlayAnimationClip(ReloadPrimaryGunAnimationClip, 1);
		}

		private void Reload()
		{
			Debug.Log("reload happens");
			gunAttack.CurrentGun.Reload();
			OnReload?.Invoke();
		}

		public void OnDestroy()
		{
			player.Controller.ReloadTriangle.OnPress -= Player_Reload;
			base.Stop();
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			if (player != null) player.Controller.ReloadTriangle.OnPress -= Player_Reload;
			if (gunAttack != null) gunAttack.OnNeedsReload -= Gun_OnNeedsReload;
			player.Controller.ReloadTriangle.OnPress += Player_Reload;
			Debug.Log("set player reload");
		gunAttack.OnNeedsReload += Gun_OnNeedsReload;
		}


		private void Gun_OnNeedsReload()
		{
			Debug.Log("gun neesd reload do");
			Do();
		}

		private void Player_Reload(NewControlButton btn)
		{
			Debug.Log("player do");
			Do();
		}
	}
}
