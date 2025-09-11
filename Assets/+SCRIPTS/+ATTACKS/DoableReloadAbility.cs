using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class DoableReloadAbility : Ability
	{
		public event Action OnReload;
		public AnimationClip ReloadAKAnimationClip;
		public AnimationClip ReloadGlockAnimationClip;
		private GunAttack gunAttack => _gunAttack ??= GetComponent<GunAttack>();
		private GunAttack _gunAttack;

		private float reloadTime = .5f;

		private List<Gun> guns => _guns??= GetComponents<Gun>().ToList();
		private List<Gun> _guns;
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
			Invoke(nameof(Reload), reloadTime);
			anim.SetBool(UnitAnimations.IsBobbing, false);

			if (!gunAttack.IsUsingPrimaryGun)
			{
				Debug.Log("playing glock reload animation");
				PlayAnimationClip(ReloadGlockAnimationClip, 1);
			}
			else
				PlayAnimationClip(ReloadAKAnimationClip, 1);
		}

		private void Reload()
		{
			Debug.Log("reload happens");
			gunAttack.CurrentGun.Reload();
			OnReload?.Invoke();
		}

		public override void Stop()
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
